using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Core singleton for application. Used to navigate across managers and key game objects
//TODO:  Player, World, 
public class GameSession : MonoBehaviour
{

    // Singleton instance variable
    public static GameSession instance = null;

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
