using System;
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
    string Render() 
    {
        return ExecuteWithState("[Render]");
    }
    
    // Identical to calling MakeChoice, followed by Render
    string Select(string choice_id)
    { 
        return ExecuteWithState($"[Select {choice_id}]");
    }

    // Direct access to the Step interpreter.
    // Executes a step task and prints the result.
    // For more information on accepted syntax, see the Step Language Reference 
    // https://github.com/ianhorswill/Step/raw/master/Step%20Language%20Reference.docx
   string ExecuteWithState(string code)
    {
        (string result, State newState) = this.module.ParseAndExecute(code, this.state);
        this.state = newState;
        string render = StepManager.FormatHTML(result);
        return render;
    }
    
    private void SetupStep()
    {
        this.module = LoadModule();
        this.state = State.Empty;

        // Load the StoryAssembler library, implemented in Step
        LoadStoryAssembler();
        Debug.Log("Loaded StoryAssembler: " + storyAssemblerLoaded + " and optional scene: " + optionalScenePathLoaded);
        Initialize(this.sceneName);
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

    private static string FormatHTML(string html)
    {
        // Replace all newlines and tabs with a single space
        html = Regex.Replace(html, @"[\r\n\t]+", " ");

        // Lowercase all element text
        html = Regex.Replace(html, @"<([^>]*)>", match =>
        {
            string tag = match.Groups[1].Value;
            return $"<{tag.ToLower()}>";
        });

        // Indent the HTML
        string[] lines = html.Split('<');
        int indentLevel = 0;
        string result = "";
        foreach (string line in lines)
        {
            if (line.StartsWith("/"))
            {
                indentLevel--;
            }
            string indent = "";
            for (int i = 0; i < indentLevel; i++)
            {
                indent += "    ";
            }
            result += indent + "<" + line.ToLower() + "\n";
            if (!line.StartsWith("/") && !line.EndsWith(">"))
            {
                indentLevel++;
            }
        }

        return result.TrimEnd();
    }
    
}