using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Step;
using System.Text.RegularExpressions;



public class StepManager : MonoBehaviour
{
    // Singleton instance variable
    public static StepManager instance = null;
    public string storyAssemblerPath; // The path to the StoryAssembler step implmentation
    public string optionalScenePath; // If desired, you can specify an additional path to a file containing your current scene (e.g. "Assets/Scripts/Scenes/Maze.step")
    public string sceneName;

    private Module module;
    private State state;
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

    /// A temporary example of how to use the StepManager
    void UsageDemo() 
    {   
        string current_scene_json = Render();
        Debug.Log(current_scene_json);
        current_scene_json = Select("welcome");
        Debug.Log(current_scene_json);
        var save = SaveState();
        Debug.Log("Save: " + save);
    }

    // Should be called before anything else with a scene name as defined in the Step file: 
    // Step: Scene maze. 
    // the scene name would be "maze"
    string Initialize(string sceneName)
    {
        return ExecuteWithState($"[Initialize {sceneName}]");
    }

    // Print the choices available in the current scene
    // Unnecessary if you are using the Select or Render functions
    string PrintChoices()
    {
        return ExecuteWithState("[PrintChoices]");
    }

    // Represents a player selecting a choice
    // choice_id is the id of the choice as defined in the Step file,
    // returned by Render or PrintChoices
    string MakeChoice(string choice_id)
    { 
        return ExecuteWithState($"[MakeChoice {choice_id}]");
    }

    // Render the current scene, including the fragment content, characters, and choices.
    // TODO Question: Should we return the JSON string or the SerializedSceneRender object?
    string Render() 
    {
        // string sceneString = ExecuteWithState("[RenderScene]");
        string contentString = ExecuteWithState("[RenderFragment]");
        string currentFragment = ExecuteWithState("[CurrentFragment]");
        string speakerString = ExecuteWithState("[RenderSpeaker]");
        // string choicesString = ExecuteWithState("[RenderNextBestChoices]");
        string systemMessage = ExecuteWithState("[Error]");

        var renderedScene = new SerializedFragmentRender()
        {
            fragmentID = currentFragment, 
            content = contentString,
            choices = new SerializedChoice[] { 
                new SerializedChoice() { id = "think_twice", text = "Should you really do this?" },
                new SerializedChoice() { id = "welcome", text = "Welcome to the game. This choice should be displayed second." },
                new SerializedChoice() { id = "run", text = "Get out of there!" },
            },
            characters = new SerializedCharacter[] { 
                new SerializedCharacter() { id = "samantha", name = "Samantha", assetPath = "Assets/Scripts/Scenes/Characters/samantha.png", position = new Tuple<int, int>(0, 20) },
            },
            speakerID= "samantha", // this can be empty
            systemMessage = systemMessage // Error messages, etc. 
        };

        // Convert the object to JSON
        return JsonUtility.ToJson(renderedScene);
    }

    string parseStep(string stepOutput) 
    {
        return stepOutput; // TODO
        //     string itemDelim = ExecuteWithState("[ItemDelim]");
        //     string[] objects = stepOutput.Split(itemDelim);
        //     // for each object, call parseStep on with the type of the array
        //     // then add the result to the array
        //     var subType = typeof(T).GetElementType();
        //     subType[] subObjects = new subType[objects.Length];
        //     for (int i = 0; i < objects.Length; i++)
        //     {
        //         var subObject = parseStep(subType, objects[i]);
        //         subObjects[i] = subObject;
        //     }
        //     return subObjects;
    }

    string SaveState()
    {
        var state = new SerializedFragmentSaveState() {
            currentFragment = "fragment_id_1",
            stateVariables = new Dictionary<string, object>() {
                { "Married", false },
                {"LearningGoalProgress", 4},
            },
            characters = new SerializedCharacter[] { 
                new SerializedCharacter() { id = "samantha", name = "Samantha", assetPath = "Assets/Scripts/Scenes/Characters/samantha.png", position = new Tuple<int, int>(0, 20) },
            }
        };
        return JsonUtility.ToJson(state);
    }

    void LoadState(string state)
    {
        // TODO load a scene from a save state
    }
    
    // Identical to calling MakeChoice, followed by Render
    string Select(string choice_id)
    { 
        // return ExecuteWithState($"[Select {choice_id}]"); // This is the old way, TODO remove this from .step files
        MakeChoice(choice_id);
        return Render();
    }

    // Direct access to the Step interpreter.
    // Executes a step task and prints the result.
    // For more information on accepted syntax, see the Step Language Reference 
    // https://github.com/ianhorswill/Step/raw/master/Step%20Language%20Reference.docx
   string ExecuteWithState(string code)
    {
        (string result, State newState) = this.module.ParseAndExecute(code, this.state);
        this.state = newState;
        return result;
    }
    
    private void SetupStep()
    {
        this.module = LoadModule();
        this.state = State.Empty;

        // Load the StoryAssembler library, implemented in Step
        LoadStoryAssembler();
        Debug.Log(
            "Loaded StoryAssembler: " + storyAssemblerLoaded + 
            " and optional scene: " + optionalScenePathLoaded
        );
        Initialize(this.sceneName);
        UsageDemo();
    }

    private void LoadStoryAssembler() 
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
}