
# We can assume that ?source_frag has already been expanded, this will not expand it
# source_frag must be ground.... the NotAny in bestpath might give an issue
# if source_frag is request, search for satisfiable request
    # compile full request
    # request will not have choices
    # gather choice types
    # gather effects and conditions
    # requesting frag will have conditions met
    # requesting frag will have effects
    # response frag will have effects, conditions, choices, choice_label
    request (choice_label, x conditions, x effects)
    response (go_to_choices, ChoiceConditions, x conditions, x effects)

    base (choice_label, x conditions, x effects, go_to_choices, ChoiceConditions)


    # get go to if satisifies source or is a request
    # get choice_spec if satisfies source or is a request
    
    # can't have top level requests

    # will need to know the effects of  
    # check conditions of response

    # if nothing met, return a top level frag

    # Step Issues

    
    [Member [?a ?b] ^A] fails whereas
    [Member ?m ^A] [= ?m [?a ?b]] succeeds

    [Different [1 2] [1 2]] Succeeds
    [Not [= [1 2] [1 2]]] Fails (as it should)
    

    Printing in Throw 