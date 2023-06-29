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

    public bool debug = false;

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

    /// An example of how to use the StepManager
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
    public void InitializeStepStoryAssembler()
    {
        this.module = CreateModule();
        this.state = State.Empty;

        // Load the StoryAssembler library, implemented in Step
        LoadStoryAssembler();
        Debug.Log(
            "Loaded StoryAssembler: " + storyAssemblerLoaded + 
            " and optional scene: " + optionalScenePathLoaded
        );
        
        this.subItem = ExecuteStep("[Delim]");
        this.itemDelim = ExecuteStep("[ItemDelim]");

        Initialize(this.sceneName);
        // We need to call Render once to get the first fragment
        SerializedFragment fragment = Render();
        // Check the length of choices
        if (this.debug)
            Debug.Log("Choices: " + fragment.choices.Length);
        if (fragment.choices.Length == 0) {
            Debug.Log("No choices found from the root fragment");
        } else if (fragment.choices.Length > 1) {
            Debug.Log("Warning: StoryAssembler returned multiple choices from the root");
            Select(fragment.choices[0].id);
        } else {
            // There should only be one choice if StoryAssembler is working nominally
            if (this.debug)
                Debug.Log("Selecting choice: " + fragment.choices[0].id);
            Select(fragment.choices[0].id);
        }
    }

    /** 
     * Reload story assembler and create a new Step module. Does not reload the Step library.
     * 
    **/
    public void Reload()
    {
        // Currently we can get away with calling the initialization function again,
        // but we may need to do more in the future, so the separation may be useful. 
        this.InitializeStepStoryAssembler();
    }

    // Should be called before anything else with a scene name as defined in the Step file: 
    // Step syntax:
    //      Scene maze. 
    // the scene name would be "maze"
    string Initialize(string sceneName)
    {
        return ExecuteStep($"[Initialize {sceneName}]");
    }

    // Print the choices available in the current scene
    // Unnecessary if you are using the Select or Render functions
    string PrintChoices()
    {
        return ExecuteStep("[PrintChoices]");
    }

    // Represents a player selecting a choice
    // choice_id is the id of the choice as defined in the Step file,
    // returned by Render or PrintChoices
    private string MakeChoice(string choice_id)
    { 
        return ExecuteStep($"[MakeChoice {choice_id}]");
    }

    /**
     * Parse Raw Current Step Fragment into Fragment GameObject
     * if the step parse fails for a given field, it will be null.
     */
    public SerializedFragment Render() 
    {
        var renderedScene = new SerializedFragment()
        {
            fragmentID =  Normalize(ExecuteStep("[CurrentFragment]")), 
            content = ExecuteStep("[RenderFragmentContent]"),
            choices = ExecuteStep<SerializedChoice>("[RenderNextBestChoices]"),
            characters = ExecuteStep<SerializedCharacter>("[RenderCharacters]"),
            speakerID =  ExecuteStep("[RenderSpeaker]"), // this can be empty
            backgroundPath = ExecuteStep("[RenderBackground]"),
            systemMessage = ExecuteStep("[Error]") // Error messages, etc.
            // warnings = ExecuteStep("[Warnings ^CurrentScene]"), // Warning messages, etc.
        };

        return renderedScene;
    }

    string SaveState()
    {
        // TODO this is a temporary example of the data format
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
    
    /* 
    * Represents the user selecting the choice with the given id.
    * Returns the next fragment to be rendered.
    */
    public SerializedFragment Select(string choice_id)
    {
        MakeChoice(choice_id);
        if (this.debug)
            Debug.Log("Rendering " + choice_id);
        return Render();
    }

    /* 
    * This function provides direct access to the Step interpreter.
    * It executes a step task and returns the result.
    * For more information on accepted syntax, see the Step Language Reference https://github.com/ianhorswill/Step/raw/master/Step%20Language%20Reference.docx
    *
    * @param code The Step code to execute e.g. "[MyTask argument1 argument2]"
    * @return The result of the execution. 
    * Returns null and throws a debug message if the execution or parse fails.
    */
    public string ExecuteStep(string code)
    {   
        try {
            if (this.debug)
                Debug.Log("Executing Step: " + code);
            return ParseAndExecute(code);
        } catch (Exception e) {
            Debug.Log(e);
            return null;
        }
    }

    // Generic overload for ExecuteStep that parses the result into a list of serializable objects
    // e.g. ExecuteStep<SerializedChoice>("[RenderNextBestChoices]")
    public T[] ExecuteStep<T>(string code)
    {   
        string result = null;
        try {
            result = ParseAndExecute(code);
        } catch (Exception e) {
            Debug.Log(e);
            return null;
        }
        try {
            return ParseStep<T>(result);
        } catch (Exception e) {
            Debug.Log(e);
            return null;
        }
    }

    private string ParseAndExecute(string code)
    {
        (string result, State newState) = this.module.ParseAndExecute(code, this.state);
        this.state = newState;
        return result;
    }

    /* 
    * Helper function that parses a step output into a list of serializable objects
    */
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
            return (T) (object) new SerializedChoice() { id = Normalize(fields[0]), text = Normalize(fields[1])};
        }
        else if (typeof(T) == typeof(SerializedCharacter))
        {
            return (T) (object) new SerializedCharacter() { 
                id = Normalize(fields[0]),
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
    private static Module CreateModule()
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