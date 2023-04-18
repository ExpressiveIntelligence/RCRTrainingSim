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

    // Player Profile Information and Save Data
    public PlayerProfile playerProfile;

    // Updated Game World Object
    public GameWorld gameWorld;

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
}
