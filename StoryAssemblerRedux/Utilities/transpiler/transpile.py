# Transpile a StoryAssembler.js json scene into a .step scene
# Usage: pip install -r requirements.txt
#        python transpile.py <path to json scene overview> <path to json scene fragments> <path to output step scene>

import sys
import os
import re
import jstyleson as json
import stringcase as sc
import string
from collections import Counter

want_tracker = Counter()
choice_tracker = Counter()
all_globals = {}

# Translations between StoryAssembler.js and Step conditionals
condition_translations = {
    "eq": "=",
    "neq": "Different",
    "lt": "<",
    "lte": "<=",
    "gt": ">",
    "gte": ">="
}
set_translations = {
    "set": "set",
    "incr": "inc",
    "decr": "dec"
}



step_template = \
"""# Declare the scene
Scene {scene}.

# Declare all storylets (fragments)
[randomly]
{fragment_declarations}

# Scene specific predicates
{predicates}

InitialSceneState {scene}: 
{initial_state}
[end]

# Fragments
{fragments}

# Declare the StorySpec
[randomly]
{wants}

[randomly]
{fulfillments}"""

def escape(s):
    return re.subn(r"([\|\{\}])", r"\\\1", s)[0]

def format_content(val):
    if type(val) == list:
        val = " ".join(val)
    val = escape(val)
    return val.strip().replace("\n", "")

def format_global(var):
    # snake or camel case to pascal case
    var = var.strip().replace("?", "")
    global_var = '^' + sc.pascalcase(var) 
    return global_var

def format_literal(var):
    # camel case to snake case
    var = var.replace("'", "")
    var = var.strip().replace("^", "")
    return sc.snakecase(var)

def format_local(var):
    return f"?{string.snakecase(var.strip().replace('^', ''))}"

def format_value(value):
    default = "0"
    if value in ["true", "false"]:
        value = value
        default = "false"
    elif value.isnumeric():
        value = value
        default = "0"
    elif value.replace(".", "").isnumeric():
        value = str(float(value))
        default = "0"
    else:
        value = format_literal(value)
        default = "none"
    return value, default

def parse_global(var, value=None):
    global_var = format_global(var)
    value, type = format_value(value)
    all_globals[global_var] = type
    return global_var, value, type

def format_condition(cond):
    tokens = cond.split()
    # if tokens[0] == "state:":
    #     print("condition", cond)
    #     return "TODO: conditional state set", "" 
    var, value, _ = parse_global(tokens[0], tokens[2])
    conditional = condition_translations[tokens[1]]
    return f"[{conditional} {var} {value}]", var

def make_want(want):
    want, var = format_condition(want)
    want_id = f"{format_literal(var)}_{str(want_tracker[var])}"
    return want_id, want

def format_set(text):
    tokens = text.split()
    set = set_translations[tokens[0]]
    eq = " =" if set == "set" else ""
    var, value, _ = parse_global(tokens[1], tokens[2])
    return f"[{set} {var}{eq} {value}]", var 

def initialize(var, default):
    return f"[set {var} = {default}]"

def characters(json_chars):
    step_chars = ""
    for id, char in json_chars.items():
        step_char = f"Character {id} {char['name']}.\n"
        step_chars += step_char
    return step_chars

def story_spec(scene, wishlist):
    wants = fulfillments = ""
    for want in wishlist:
        if "hoverText" in want.keys():
            continue # weird UI things
        want_id, want = make_want(want["condition"])
        wants += f"Want {scene} {want_id}.\n"
        fulfillments += f"Fulfilled {want_id}: {want}\n"
    return wants, fulfillments

def initial_scene_state(start):
    step_start = []
    initialized_vars = set()
    for text in start:
        initial, var = format_set(text)
        step_start.append("\t" + initial)
        initialized_vars.add(var)
    step_start.append("\t# The following variables have had types inferred and should be reviewed.")
    for var in set(all_globals.keys()) - initialized_vars:
        step_start.append("\t" + initialize(var, all_globals[var]))

    return "\n".join(step_start)

def format_choice(id, json_choice):
    if 'gotoId' in json_choice.keys():
        goto = json_choice['gotoId']
        return f"GoToChoice {id} {format_literal(goto)}." # TODO add speaker choice
    elif 'condition' in json_choice.keys():
        condition = json_choice['condition']
        condition, var = format_condition(condition)
        choice_name = f"{format_literal(var)}_{str(choice_tracker[var])}"
        return f"ChoiceCondition {id} {choice_name}: {condition}"
    else:
        raise Exception("Choice is not a goto or conditional choice")

def format_fragment(id, json_frag):
    frag = ""
    for var, val in json_frag.items():
        if var == "content":
            frag += f"Content {id}: {format_content(val)}.\n"
        elif var == "choiceLabel":
            frag += f"ChoiceLabel {id}: {format_content(val)}\n"
        elif var == "speaker":
            frag += f"Speaker {id} {format_literal(val)}.\n"
        elif var == "effects":
            if val:
                frag += f"Effects {id}:\n"
                for effect in val:
                    frag += f"\t{format_set(effect)[0]} \n"
                frag += "[end]\n"
        elif var == "conditions":
            if val:
                frag += f"Conditions {id}:\n"
                for cond in val:
                    frag += f"\t{format_condition(cond)[0]} \n"
                frag += "[end]\n"
        elif var == "request":
            for type, args in val.items():
                if type == "condition":
                    f_args = format_condition(args)[0]
                elif type == "gotoId":
                    type = "goto"
                    f_args = format_literal(args)
                else:
                    raise Exception(f"Request type {type} not recognized")
                frag += f"Request {id} {type} {f_args}.\n"
        elif var == "choices":
            for choice in val:
                frag += format_choice(id, choice) + "\n"

    if "conditions" not in json_frag.keys():
        frag += f"Conditions {id}.\n"
    if "effects" not in json_frag.keys():
        frag += f"Effects {id}.\n"

    return frag

def format_fragments(scene, json_fragments):
    declarations = []
    fragments = []
    for frag in json_fragments:
        id = format_literal(frag["id"])
        declarations.append(f"Fragment {id} {scene}.")
        fragments.append(format_fragment(id, frag))

    return "\n".join(declarations), "\n".join(fragments)
                   
def convert_json_to_step(json_scene, json_fragments):
    scene = json_scene["id"]
    scene = format_literal(scene)
    wants, fulfillments = story_spec(scene, json_scene["wishlist"])

    fragment_declarations, fragments = format_fragments(scene, json_fragments)

    # uses a global variable populated in format_fragments
    initial_state = initial_scene_state(json_scene["startState"])

    predicates = ""

    return step_template.format(**locals())

if __name__ == "__main__":
    # Get the path to the scene file
    js_game_path = sys.argv[1] if len(sys.argv) > 1 else None
    if js_game_path is None:
        print("No game file specified")
        exit(1)

    # Get the path to the scene file
    js_scene_path = sys.argv[2] if len(sys.argv) > 2 else None
    if js_scene_path is None:
        print("No scene file specified")
        exit(1)

    # Get the path to the output file
    step_path = sys.argv[3] if len(sys.argv) > 3 else None
    if step_path is None:
        print("No output file specified")
        exit(1)

    # Read the json file
    with open(js_game_path, "r") as js_file:
        js_game = json.load(js_file)

    js_scene = [] 
    # Detect if it is a directory (multiple libaries that need to be concatenated)
    if os.path.isdir(js_scene_path):
        for file in os.listdir(js_scene_path):
            with open(os.path.join(js_scene_path, file), "r") as js_file:
                js_scene += json.load(js_file)
    else:
        with open(js_scene_path, "r") as js_file:
            js_scene = json.load(js_file)

    # Convert the json scene to a step scene
    step_scene = convert_json_to_step(js_game, js_scene)

    with open(step_path, "w") as step_file:
        step_file.write(step_scene)

