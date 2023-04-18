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
    private string itemDelim = "";
    private string subItem = "";
    
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
        SerializedFragment fragment = Render();
        var current_scene_json = JsonUtility.ToJson(fragment);
        Debug.Log(current_scene_json);
        fragment = Select("welcome");
        current_scene_json = JsonUtility.ToJson(fragment);
        Debug.Log(current_scene_json);
        var save = SaveState();
        Debug.Log("Save: " + save);
    }

    
    /** 
     *  Initialize Step Library 
     */
    public void InitStep()
    {
        this.module = LoadModule();
        this.state = State.Empty;

        // Load the StoryAssembler library, implemented in Step
        LoadStoryAssembler();
        Debug.Log(
            "Loaded StoryAssembler: " + storyAssemblerLoaded + 
            " and optional scene: " + optionalScenePathLoaded
        );
        
        this.subItem = ExecuteWithState("[Delim]");
        this.itemDelim = ExecuteWithState("[ItemDelim]");

        Initialize(this.sceneName);
        UsageDemo();
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

    /**
     * Parse Raw Current Step Fragment into Fragment GameObject
     */
    SerializedFragment Render() 
    {
        var renderedScene = new SerializedFragment()
        {
            fragmentID =  ExecuteWithState("[CurrentFragment]"), 
            content = ExecuteWithState("[RenderFragment]"),
            choices = ExecuteWithState<SerializedChoice>("[RenderNextBestChoices]"),
            characters = ExecuteWithState<SerializedCharacter>("[RenderCharacters]"),
            speakerID=  ExecuteWithState("[RenderSpeaker]"), // this can be empty
            backgroundPath =  "Assets/PlaceholderPath.png", // TODO - implement the Step function retrieval of this
            systemMessage = ExecuteWithState("[Error]") // Error messages, etc. 
        };

        //TODO: Change to instantiating serialized objects instead

        // TODO discuss RETURN BOOL - INDICATING SUCCESSFUL PARSE OF STEP CONTENT
        return renderedScene;
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
                new SerializedCharacter() { id = "samantha", name = "Samantha", assetPath = "Assets/Scripts/Scenes/Characters/samantha.png", x=1, y=10 },
            }
        };
        return JsonUtility.ToJson(state);
    }

    void LoadState(string state)
    {
        // TODO load a scene from a save state
    }
    
    // Identical to calling MakeChoice, followed by Render
    SerializedFragment Select(string choice_id)
    { 
        // return ExecuteWithState($"[Select {choice_id}]"); // This is the old way, TODO remove this from .step files
        MakeChoice(choice_id);
        return Render();
    }

    // Direct access to the Step interpreter.
    // Executes a step task and prints the result.
    // For more information on accepted syntax, see the Step Language Reference 
    // https://github.com/ianhorswill/Step/raw/master/Step%20Language%20Reference.docx
    public string ExecuteWithState(string code)
    {
        return ParseAndExecute(code);
    }

    public T[] ExecuteWithState<T>(string code)
    {
        string result = ParseAndExecute(code);
        T[] parsed = ParseStep<T>(result);
        return parsed;
    }

    private string ParseAndExecute(string code)
    {
        (string result, State newState) = this.module.ParseAndExecute(code, this.state);
        this.state = newState;
        return result;
    }

    
    private T[] ParseStep<T>(string stepOutput) {
        string[] items = stepOutput.Trim().Split(this.itemDelim);
        // for each, parse into a choice
        var parsedItems = new List<T>();
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == "") continue;
            var item = InitStepItem<T>(items[i].Split(this.subItem));
            parsedItems.Add(item);
        }
        return parsedItems.ToArray();
    }

    private T InitStepItem<T>(object[] fields) {
        // switch statement on type of T, if it is a character create a Character, etc
        if (typeof(T) == typeof(SerializedChoice))
        {
            return (T) (object) new SerializedChoice() { id = Normalize(fields[0]).ToLower(), text = Normalize(fields[1])};
        }
        else if (typeof(T) == typeof(SerializedCharacter))
        {
            return (T) (object) new SerializedCharacter() { 
                id = Normalize(fields[0]).ToLower(), 
                name = Normalize(fields[1]),
                x = Int32.Parse(Normalize(fields[2])),
                y = Int32.Parse(Normalize(fields[3])),
                assetPath = Normalize(fields[4]),
            };
        }
        else
        {
            return default(T);
        }
    }

    private string Normalize(object o) {
        return ((string) o).Trim();
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