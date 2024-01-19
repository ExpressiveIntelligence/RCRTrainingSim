using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Step; // Using this line gives us the debug error "The identifier ParseAndExecute is not in scope." To fix it:
// using static Step.Module; 
using System.Text.RegularExpressions;
using System.Linq;

public class StepManager : MonoBehaviour
{
    // Singleton instance variable
    public static StepManager instance = null;
    // Unity Variables
    public string storyAssemblerPath; // The path to the StoryAssembler step implmentation

    [SerializeField]
    private List<TextAsset> m_stepFiles;

    // This will be deprecated once we are using m_stepFiles
    public string optionalScenePath; // If desired, you can specify an additional path to a file containing your current scene (e.g. "Assets/Scripts/Scenes/Maze.step")


    public string sceneName;
    public GameSession gameSession;
    // Debug Variables
    public bool extraDebugLogging = true;
    // Step Variables
    private Module module;
    public State state;
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

        //Get gameSession reference
    }

    // Start is called before the first frame update
    void Start()
    {
        //safety check for gameSession
        if (!this.gameSession)
        {
            //get by tag
            GameObject[] gameSessions = GameObject.FindGameObjectsWithTag("GameSession");
            if (gameSessions.Length == 0)
            {
                Debug.Log("ERROR: GameSession GameObject not found.");
            }
            else
            {
                this.gameSession = gameSessions[0].GetComponent<GameSession>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// An example of how to use the StepManager
    void UsageDemo() 
    {   
        SerializedFragment fragment = Render();
        var currentSceneJson = JsonUtility.ToJson(fragment);
        Debug.Log(currentSceneJson);
        fragment = Select("welcome");
        currentSceneJson = JsonUtility.ToJson(fragment);
        Debug.Log(currentSceneJson);
        var save = SaveState();
        Debug.Log("Save: " + save);
    }

    /*
     * Reload story assembler and create a new Step module. 
     * Returns the new current fragment.
     * Does not reload the Step DLL itself.
    **/
    public SerializedFragment Reload()
    {
        return this.InitializeStepStoryAssembler();
    }
    
    /** 
     *  Initialize StoryAssembler and the Step library interface
     */
    public SerializedFragment InitializeStepStoryAssembler()
    {
        this.module = CreateModule();
        this.state = State.Empty;

        // Load the StoryAssembler library, implemented in Step
        LoadStoryAssembler();
        
        // Call the step Initialize function and set the delimiter variables
        Initialize(this.sceneName);

        bool doReRender = false;

        // We need to call Render once to get the first fragment
        SerializedFragment fragment = Render();
        // Check the length of choices
        if (this.extraDebugLogging)
            Debug.Log("Choices: " + fragment.choices.Length);
        if (fragment.choices.Length == 0) {
            Debug.Log("No choices found from the root fragment");
        } else if (fragment.choices.Length > 1) {
            Debug.Log("Warning: StoryAssembler returned multiple choices from the root");
            doReRender = true;
        } else {
            // There should only be one choice if StoryAssembler is working nominally
            if (this.extraDebugLogging)
                Debug.Log("Selecting choice: " + fragment.choices[0].id);
            doReRender = true;
        }

        if (doReRender) {
            string choice = fragment.choices[0].id.ToLower(); // We shouldn't have to lower case: TODO call verbatm case in Step
            fragment = Select(choice); 
        }

        return fragment;
    }

    /** 
     * This is an overloaded version of the same function, but takes a state object as an argument.
    **/
    public SerializedFragment InitializeStepStoryAssembler(State state)
    {
        this.module = CreateModule();
        this.state = state;

        LoadStoryAssembler();

        this.subItem = ExecuteStep("[Delim]"); // Normally this is accomplished in Initialize() which we don't call here
        this.itemDelim = ExecuteStep("[ItemDelim]");

        SerializedFragment fragment = Render();
        return fragment;
    }


    // Should be called before anything else with a scene name as defined in the Step file: 
    // Step syntax:
    //      Scene maze. 
    // the scene name would be "maze"
    string Initialize(string sceneName)
    {
        this.subItem = ExecuteStep("[Delim]");
        this.itemDelim = ExecuteStep("[ItemDelim]");

        return ExecuteStep($"[Initialize {sceneName}]");
    }

    // Print the choices available in the current scene
    // Unnecessary if you are using the Select or Render functions
    string PrintChoices()
    {
        return ExecuteStep("[PrintChoices]");
    }

    // Represents a player selecting a choice. 
    // This function should not be used if you plan to render a fragment after making a choice. In that case, use Select() instead.
    // <param> choiceID </param> the id of the choice as defined in the Step file, returned by Render or PrintChoices
    private string MakeChoice(string choiceID)
    { 
        return ExecuteStep($"[MakeChoice {choiceID}]");
    }

    /**
     * Parse Raw Current Step Fragment into Fragment GameObject
     * if the step parse fails for a given field, it will be null.
    **/
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
            systemMessage = ExecuteStep("[Error]"), // Error messages, etc.
            // warnings = ExecuteStep("[Warnings ^CurrentScene]"), // Warning messages, etc.
        };

        // there is a more elegant way of doing this
        KeyValuePair<string, string>[] tagsArray = ExecuteStep<KeyValuePair<string, string>>("[RenderFragmentTags]");
        renderedScene.tags = tagsArray.ToDictionary(tuple => tuple.Key.ToLower(), tuple => tuple.Value.ToLower());

        return renderedScene;
    }

    SerializedSaveState SaveState()
    {
        var save = new SerializedSaveState() {
            currentFragment = Normalize(ExecuteStep("[CurrentFragment]")),
            stepState = this.state,
            characters = new SerializedCharacter[] { 
                new SerializedCharacter() { id = "brad", name = "Brad", x=1, y=10 }, // Mock
            }
        };
        return save;
    }

    void LoadState(SerializedSaveState save)
    {
        InitializeStepStoryAssembler(save.stepState);
    }
    
    /* 
    * Represents the user selecting the choice with the given id.
    * Returns the next fragment to be rendered.
    */
    public SerializedFragment Select(string choiceID)
    {
        MakeChoice(choiceID);
        if (this.extraDebugLogging) Debug.Log("Rendering " + choiceID);
        Debug.Log("Thread " + ExecuteStep("[Thread]"));
        return Render();
    }

    /* 
    * This function provides direct access to the Step interpreter.
    * It executes a step task and returns the result.
    * For more information on accepted syntax, see the Step Language Reference https://github.com/ianhorswill/Step/raw/master/Step%20Language%20Reference.docx
    * The default implementation of this function will return a string, there is an overloaded generic version 
    * that will parse the result into a list of objects of type T.
    *
    * @param code The Step code to execute e.g. "[MyTask argument1 argument2]"
    * @return The result of the execution. 
    * Returns null and throws a debug message if the execution or parse fails.
    */
    public string ExecuteStep(string code)
    {   
        try {
            if (this.extraDebugLogging)
                Debug.Log("Executing Step: " + code);
            return ParseAndExecute(code);
        } catch (Exception e) {
            Debug.Log(e);
            return null;
        }
    }

    /* 
    * Generic overload for ExecuteStep that parses the result into a list of serializable objects
    * e.g. ExecuteStep<SerializedChoice>("[RenderNextBestChoices]")
    */
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
        if (typeof(T) == typeof(KeyValuePair<string, string>)) { // Right now we only support two item tuples
            return (T) (object) new KeyValuePair<string, string>(Normalize(fields[0]), Normalize(fields[1]));
        }
        else if (typeof(T) == typeof(SerializedChoice))
        {
            return (T) (object) new SerializedChoice() { id = Normalize(fields[0]), text = Normalize(fields[1])};
        }
        else if (typeof(T) == typeof(SerializedCharacter))
        {
            SerializedCharacter character = new SerializedCharacter() { 
                id        = Normalize(fields[0]),
                name      = Normalize(fields[1]),
                x         = Int32.Parse(Normalize(fields[2])),
                y         = Int32.Parse(Normalize(fields[3])),
            };

            KeyValuePair<string, string>[] tagsArray = ExecuteStep<KeyValuePair<string, string>>("[RenderCharacterTags ?" + character.id + "]");
            character.tags = tagsArray.ToDictionary(tuple => tuple.Key.ToLower(), tuple => tuple.Value.ToLower());

            return (T) (object) character;
        }
        else
        {
            return default(T);
        }
    }

    private string Normalize(object o) {
        return ((string) o).Trim();
    }

    /* 
    * Load the StoryAssembler library, implemented in Step
    */
    private void LoadStoryAssembler() 
    {
        this.module.LoadDirectory(this.storyAssemblerPath);
        if (optionalScenePath != null && optionalScenePath != "")
        {
            this.module.LoadDefinitions(this.optionalScenePath);
            this.optionalScenePathLoaded = true;
        }
        this.storyAssemblerLoaded = true;

        foreach (var stepFile in m_stepFiles)
        {
            this.module.AddDefinitions(stepFile.text);
        }

        string debugMessage = storyAssemblerLoaded ? "StoryAssembler Loaded." : "StoryAssembler FAILED to Load.";
        debugMessage += optionalScenePathLoaded ? " Optional scene loaded." : " Optional scene FAILED to load.";
        Debug.Log(debugMessage);
    }

    // Create a Step Module object as defined in the Step C# library
    private static Module CreateModule()
    {
        return new Module("StepManager");
    }
}