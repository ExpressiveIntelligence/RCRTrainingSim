using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Create a serialized version with Alex
public class GameWorld : MonoBehaviour
{
    // Singleton instance variable
    public static GameWorld instance = null;

    // Create a map of characters for save state purposes
    public Dictionary<string, Character> characters;

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
