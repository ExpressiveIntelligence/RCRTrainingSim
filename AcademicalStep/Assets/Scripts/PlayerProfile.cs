using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Create a serialized version with the group
public class PlayerProfile : MonoBehaviour
{

    // Singleton instance variable
    public static PlayerProfile instance = null;

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
