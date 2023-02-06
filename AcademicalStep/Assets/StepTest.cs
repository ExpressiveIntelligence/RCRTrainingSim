using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Step;

public class StepTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Module m = new Module("Test");
        m.LoadDefinitions("./Assets/Step/test.step");
        var result = m.Call("Test");
        print(result);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
