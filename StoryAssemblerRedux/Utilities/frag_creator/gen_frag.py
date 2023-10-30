# This utility script is 

# 1. Go to the Google Developers Console: https://console.developers.google.com/
# 2. Create a new project or select an existing one.
# 3. Enable the Google Sheets API for your project.
# 4. Create credentials for your project. You'll get a JSON file which you should save securely, as it contains sensitive information.


# To run th3e script:
# 1. Install the python requirements found in the requirements.txt file
#   pip install -r requirements.txt
# 2. run the script:
#   python gen_frag.py

import gspread
from oauth2client.service_account import ServiceAccountCredentials
import pandas as pd
import re, sys

# import the file "../frag_utils/frag_utils.py"
sys.path.append("../frag_utils/")
from frag_utils import step_template


sheet_id = "10d4UvR6uY8BSDfV4k_nIjLiSm5O7sRez8NCbchAtk4s"
# sheet_id = '1cXQ4lJ-FtmhkHW1YTQkm4uiF4pbJKudSk3jL835k8cQ'
threads = ["T0001", "T0002", "T0003"]

scene = "e0001" # The scene ID to use in the .step file (casing matters)

def read_google_sheet(tab_name):
    # Define the scope for the Google Sheets API
    scope = ["https://spreadsheets.google.com/feeds",'https://www.googleapis.com/auth/drive']

    # Provide the path to your Google Sheets API credentials
    creds = ServiceAccountCredentials.from_json_keyfile_name('./secret.json', scope)

    # Authenticate and initialize the Google Sheets API client
    client = gspread.authorize(creds)

    # Open the Google Sheets file
    sheet = client.open_by_key(sheet_id)

    # Access the worksheet by its title
    worksheet = sheet.worksheet(tab_name)

    # Get all values from the worksheet and convert it to a Pandas DataFrame
    data = worksheet.get_all_values()
    df = pd.DataFrame(data[1:], columns=data[0])
    
    return df

def clean_cell(cell):
    # if the type is a string
    if not isinstance(cell, str):
        return cell
    # replace "n/a" with empty
    cell = cell.replace("n/a", "").replace("N/A", "")
    if cell == "":
        return None
    # turn Request and Reusable into booleans, replacing their string values
    return cell

def clean_row(row):
    if not row.id:
        return None
    # if row.request:
    #     row.request = True
    if row.reusable and (row.reusable.lower() == "true"):
        row.reusable = True
    else:
        row.reusable = False
    # for each cell
    for key in row.keys():
        row[key] = clean_cell(row[key])
    return row

def split_data(string_to_split):
    return re.split(r'[ ,]+', string_to_split)

def split_task_call(string_to_split):
    # [this is a task] [another] -> ['[this is a task]', '[another]']
    # however, [Text [task1] [task2]] -> ['[Text [task1] [task2]]']
    tasks = []
    current_task = ""
    open_brackets = 0
    for char in string_to_split:
        if char == '[':
            open_brackets += 1
        if char == ']':
            open_brackets -= 1
        current_task += char
        if open_brackets == 0:
            tasks.append(current_task)
            current_task = ""
    return tasks


def multi(content):
    """
    Format content for step methods which have multiple lines.
    Content         vent_irb:
        Brad looks frustrated. "We're still waiting! It's a minimal-risk proposal, too. I mean, we're literally just recording interviews. I don't get why it's taking so long. I've been waiting for a month and I still haven't heard back."
    [end]
    """
    content = content.strip()
    if "\n" in content:
        content = re.sub(r'\n', r'\n\t', content) # each new line should be indented
        content = f"\n\t{content}\n[end]"
    return content

def create_frag(row):
    if not row.id:
        return None, None
    frag_name = row.id
    
    if(row.step_code):
        return frag_name, row.step_code

    code = ""
    if row.content:
        content = multi(row.content)
        code += f"Content {row.id}: {content}\n"
    if row.speaker:
        code += f"Speaker {row.id} {row.speaker}.\n"
    if row.choice_label:
        code += f"ChoiceLabel {row.id}: {multi(row.choice_label)}\n"
    if row.gotochoices:
        for token in split_data(row.gotochoices):
            code += f"GoToChoice {row.id} {token}.\n"
    if row.conditionalchoice:
        for i, token in enumerate(split_task_call(row.conditionalchoice)):
            code += f"ChoiceCondition {row.id} {row.id + '_' + 'c' * (i + 1)}: {multi(token)}\n"
    if row.effects:
        code += f"Effects {row.id}: {multi(row.effects)}\n"
    else:
        code += f"Effects {row.id}.\n"
    if row.conditions:
        code += f"Conditions {row.id}: {row.conditions}\n"
    else: 
        code += f"Conditions {row.id}.\n"
    if row.reusable:
        code += f"Reusable {row.id} {scene}.\n"
    # TODO tags
    code += "\n"

    return frag_name, code

# Read the Google Sheets data and print it
df = pd.DataFrame()
for thread in threads:
    tab_name = f'{thread}_Fragments'
    thread_df = read_google_sheet(tab_name)
    # add the thread to the running df
    df = pd.concat([df, thread_df], ignore_index=True)

# snake case the labels in pandas
# clean the rows
df.columns = df.columns.str.replace(' ', '_').str.lower()
# print(df.keys())

# clean the rows
df = df.apply(clean_row, axis=1)

fragment_declarations = ""
fragments = ""

# Create an entry fragment
first_frag_name = df.iloc[0].id
fragment_declarations += f"Fragment entry {scene}.\n"
fragments += f"""Content entry: Welcome to Academical!
Conditions  entry.
Effects     entry.
GoToChoice  entry {first_frag_name}.\n
"""

frag_ids = set()
for index, row in df.iterrows():
    frag_id, frag_code = create_frag(row)
    if frag_id:
        fragment_declarations += f"Fragment {frag_id} {scene}.\n"
        if frag_id in frag_ids:
            raise Exception(f"Duplicate fragment ID: {frag_id}")
        frag_ids.add(frag_id)
    if frag_code:
        fragments += frag_code
    

code = fragment_declarations + "\n\n" + fragments

# predicates = "# No Predicates"
predicates ="""fluent PleasantriesOver ?scene.
[function]
fluent Check ?thread ?frag."""

# initial_state = "# No Initial State"
initial_state = multi("""[Not [PleasantriesOver e0001]]
[set BradInsecurityToNed = 0]
[set Thread = none]""")

characters = f"""Character brad {scene} |Brad|.
CharacterAsset brad {scene} |./brad.png|.
CharacterLocation brad {scene} [c0, 0].

Character ned {scene} |Ned|.
CharacterAsset ned {scene} |./ned.png|.
CharacterLocation ned {scene} [0, 0]."""
assets = f"BackgroundAsset {scene}: |./scene_name_background.png|."
wants = f"""Want {scene} entry.
Want {scene} three."""
fulfillments = """Fulfilled entry: [Expanded entry CurrentScene]
Fulfilled three: [= Thread insecurity]"""
code = step_template.format(**locals())

# Write to a file which is specified in the command line, if none is specified, write to a default file
file_name = sys.argv[1] if len(sys.argv) > 1 else "GeneratedScene.step"
with open(file_name, "w") as f:
    f.write(code)
    f.close()

print(f"Successfully wrote to {file_name}")