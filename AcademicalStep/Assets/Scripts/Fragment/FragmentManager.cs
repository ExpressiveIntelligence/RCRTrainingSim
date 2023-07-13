using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Manager for loading, parsing, instantiating, and making return calls
 *  to Step infrastructure.
 *  
 *  Singleton - meant as a static utility
 */
public class FragmentManager : MonoBehaviour
{
    // Singleton instance variable
    public static FragmentManager instance = null;

    public GameSession gameSession;

    // Manager orchestration
    public StepManager stepManager;
    //list of managers - component/feature level singletons that perform operations on object pools
    public BackgroundManager backgroundManager;
    public DialogueManager dialogueManager;
    //Choice Manager...

    public SerializedFragment currentSerializedFragment;
    public List<SerializedFragment> fragmentHistory;

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
        //get game session and stepManager
        //perform safety checks for stepManager
        if (!this.stepManager)
        {
            //get by tag
            GameObject[] stepManagers = GameObject.FindGameObjectsWithTag("StepManager");
            if (stepManagers.Length == 0)
            {
                Debug.Log("ERROR: StepManager GameObject not found.");
            }
            else
            {
                this.stepManager = stepManagers[0].GetComponent<StepManager>();
            }
        }

        //safety check for backgroundManager
        if (!this.backgroundManager)
        {
            //get by tag
            GameObject[] backgroundManagers = GameObject.FindGameObjectsWithTag("BackgroundManager");
            if (backgroundManagers.Length == 0)
            {
                Debug.Log("ERROR: backgroundManager GameObject not found.");
            }
            else
            {
                this.backgroundManager = backgroundManagers[0].GetComponent<BackgroundManager>();
            }
        }

        //safety check for dialogueManager
        if (!this.dialogueManager)
        {
            //get by tag
            GameObject[] dialogueManagers = GameObject.FindGameObjectsWithTag("DialogueManager");
            if (dialogueManagers.Length == 0)
            {
                Debug.Log("ERROR: dialogueManager GameObject not found.");
            }
            else
            {
                this.dialogueManager = dialogueManagers[0].GetComponent<DialogueManager>();
            }
        }

        //Initializes Step Interpreter
        this.stepManager.InitializeStepStoryAssembler();

        //Render currently loaded fragment, save to our manager for reference
        SerializedFragment fragment = this.stepManager.Render();
        this.currentSerializedFragment = fragment;

        //create history, set intro scene as first in history.
        this.fragmentHistory = new List<SerializedFragment>();
        this.fragmentHistory.Add(this.currentSerializedFragment);

        //Instantiate scene ased on fragment load
        this.RenderFromCurrentFragment();
    }

    public void RenderFromCurrentFragment() 
    {
        this.backgroundManager.RenderBackgroundFromFragment(this.currentSerializedFragment);
        this.dialogueManager.RenderDialogueFromFragment(this.currentSerializedFragment);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


/*    void InitFragmentFromStepFragment() { }

    void InitBackgroundFromSerializedFragment(){
    
    }


    void InitCharacters();
    void InitChoices();
    void InitContent();*/

}
