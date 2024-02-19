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

    public Character Ned;
    public Character Brad;

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

        //Initializes Step Interpreter and render the first fragment
        SerializedFragment fragment = this.stepManager.InitializeStepStoryAssembler();

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
        //update brad and ned
        if(this.currentSerializedFragment.characters[0].tags.ContainsKey("expression")){
            Debug.Log("Found tag to render with: " + this.currentSerializedFragment.characters[0].tags["expression"]);
            this.Brad.SetSprite(this.currentSerializedFragment.characters[0].tags["expression"]);
        } else {
            this.Brad.SetSprite("fallback");
        }

        if(this.currentSerializedFragment.characters[1].tags.ContainsKey("expression")){
            Debug.Log("Found tag to render with: " + this.currentSerializedFragment.characters[1].tags["expression"]);
            this.Ned.SetSprite(this.currentSerializedFragment.characters[1].tags["expression"]);
        } else {
            this.Ned.SetSprite("fallback");
        }
        
        //Save for when we have Ned Sprites
        //this.Ned.SetSprite(this.currentSerializedFragment.characters[0])
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
