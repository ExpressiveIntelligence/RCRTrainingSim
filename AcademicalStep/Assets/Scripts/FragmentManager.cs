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

    // Helpful references
    public StepManager stepManager; 
    public GameSession gameSession; 

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
        //safety check for stepManager
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

        //Initializes Step Interpreter
        this.stepManager.InitStep();

        //Render currently loaded fragment
        //TODO: Set loaded fragment in earlier menu or from file
        SerializedFragment fragment = this.stepManager.Render();
        Debug.Log(fragment.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

/*    void InitFragmentFromStepFragment() { }

    void InitBackground();


    void InitCharacters();
    void InitChoices();
    void InitContent();*/

}
