using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Core singleton for application. Used to navigate across managers and key game objects

/**TODOS:
 * 1. Link and create safe way to reference player profile and game world
 * 
 */


public class GameSession : MonoBehaviour
{
    // Singleton instance variable
    public static GameSession instance = null;

    // Managers
    public StepManager stepManager = null;
    public FragmentManager fragmentManager = null;

    // Player Profile Information and Save Data
    public PlayerProfile playerProfile;

    // Updated Game World Object
    public GameWorld gameWorld;

    // Represents currently loaded Scene

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

        //safely check for fragmentManager
        if (!this.fragmentManager)
        {
            //get by tag
            GameObject[] fragmentManagers = GameObject.FindGameObjectsWithTag("FragmentManager");
            if (fragmentManagers.Length == 0)
            {
                Debug.Log("ERROR: FragmentManager GameObject not found.");
            }
            else
            {
                this.fragmentManager = fragmentManagers[0].GetComponent<FragmentManager>();
            }
        }

        this.fragmentManager.gameSession = this;
         
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
