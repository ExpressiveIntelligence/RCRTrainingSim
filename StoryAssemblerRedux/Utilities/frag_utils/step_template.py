step_template = \
"""# Declare the scene
Scene {scene}.

# Declare asset paths
{assets}

# Scene specific predicates
{predicates}

# Define the initial state as required by StoryAssembler
InitialSceneState {scene}: {initial_state}

# Declare the characters and locations
{characters}

# Declare all fragments
[randomly]
{fragment_declarations}

# Fragments
{fragments}

# Declare the StorySpec
[randomly]
{wants}

[randomly]
{fulfillments}"""