step_template = \
"""# Declare the scene
Scene {scene}.

# Declare asset paths
{assets}

# Scene specific predicates, fluents, and tasks
{predicates}

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