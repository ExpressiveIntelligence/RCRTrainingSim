using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Step;

public class StepManager : MonoBehaviour
{
    // Singleton instance variable
    public static StepManager instance = null;

    private Module module;
    private State state;

    public string storyAssemblerPath; // The path to the StoryAssembler step implmentation
    
    public string optionalScenePath; // If desired, you can specify an additional path to a file containing your current scene (e.g. "Assets/Scripts/Scenes/Maze.step")
    public string sceneName;
    private bool storyAssemblerLoaded = false;
    private bool optionalScenePathLoaded = false;
    
    // Awake is used to instantiate class as singleton
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        SetupStep(); // Load the objects needed to interact with the Step interpreter
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetupStep()
    {
        this.module = LoadModule();
        this.state = State.Empty;

        // Load the StoryAssembler library, implemented in Step
        LoadStoryAssembler();
        Debug.Log("Loaded StoryAssembler: " + storyAssemblerLoaded + " and optional scene: " + optionalScenePathLoaded);
        Initialize(this.sceneName);
        Demo();
    }

    void Demo() {
        Render();
        PrintChoices();
        Select("welcome");
        PrintChoices();
    }

    void LoadStoryAssembler() 
    {
        this.module.LoadDirectory(this.storyAssemblerPath);
        if (optionalScenePath != null && optionalScenePath != "")
        {
            this.module.LoadDefinitions(this.optionalScenePath);
            this.optionalScenePathLoaded = true;
        }
        this.storyAssemblerLoaded = true;
    }

    // Create a Step Module object as defined in the Step C# library
    private static Module LoadModule()
    {
        Module module = null;
        while (module == null)
        {
            try
            {
                module = new Module("StepManager");
                return module;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }
        return module;
    }

    // Execute a step task and print the result
    // For more information on accepted syntax, see the Step Language Reference 
    // https://github.com/ianhorswill/Step/raw/master/Step%20Language%20Reference.docx
    string ExecuteTask(string code)
    {
        var result = this.module.ParseAndExecute(code);
        Debug.Log(result);
        return result;
    }

    string ExecuteWithState(string code)
    {
        (string result, State newState) = this.module.ParseAndExecute(code, this.state);
        this.state = newState;
        Debug.Log(result);
        return result;
    }

    string Initialize(string sceneName)
    {
        return ExecuteWithState($"[Initialize {sceneName}]");
    }

    string PrintChoices()
    {
        return ExecuteWithState("[PrintChoices]");
    }

    string Select(string choice_id)
    { 
        return ExecuteWithState($"[Select {choice_id}]");
    }

    string MakeChoice(string choice_id)
    {
        return ExecuteWithState($"[MakeChoice {choice_id}]");
    }

    string Render() 
    {
        return ExecuteWithState("[Render]");
    }

}