# headers
[remembered]
States that when the task is called, its results (arguments and text output) should be cached.  Subsequent calls should be unified with the remembered arguments; when a match is found the remembered text, if any, should be printed and the task should complete.
[fluent]
States that the task is a [predicate] and, additionally, can be updated using now.  Methods can still be declared for the predicate, in which case they behave as defaults.
[function]
For [fluents], states that the predicate has the additional property of being a function in the sense that its last argument is unique given its previous arguments.  This means that an update like [now [F 1 2]] will not only mark [F 1 2] as true, but mark any previous [F 1 ?] as false.



# Where to find Max, other definitons

HigherOrderPrimitiveTests.cs

    FindUnique
    FindFirstNUnique
    FindAtMostNUnique
    DoAll
    Or
    Not
    ForEach
    Once
    ExactlyOnce
    Max
    Min
    PreviousCall
    UniqueCall

PrimitiveTests

    Test
    Write
    =
    <
    >

    Lists
        [add ?x List]
        [set List = empty]
        





# Scene Test old reference

# Old reference code




# How does bestPath.js work?

* retrieve all possible paths (not just paths that fulfill the storyspec)
* remove ones that don't satisfy wishlish order field (we don't have this yet)
* rank them based on number of satisfied wants
* current speaker is scored higher

* get all paths

find_path(cur_path, cur_frag)
    if cur_frag != root: expand(cur_frag)
    findunique available_frag
        find_path(cur_path + available_frag, available_frag)
    if no available_frag
        paths += cur_path

find_path(empty, root)

FindPath ?cur_path ?cur_frag:
    [case ?cur_path] [= root] : [else] [Expand ?cur_frag] [end]
    [ForEach [SelectAvailableFragment ?cur_frag ?new_node] [Add ?cur_frag ?cur_path ?new_path] [FindPath ?new_path ?new_node]]
[end]
FindPath ?cur_path ?cur_frag:
    [case ?cur_path] [= root] : [Expand ?cur_frag] [else] [end]
    [Not [SelectAvailableFragment ?cur_frag ?new_node]]
    [add ?cur_path ^AllPaths]
[end]

[case ?cur_frag] [= root] : hi [else] body [end]