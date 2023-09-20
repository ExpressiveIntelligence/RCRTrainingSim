step_template = \
"""# Declare the scene
Scene {scene}.

# Declare the characters and locations
{characters}

# Declare all fragments
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