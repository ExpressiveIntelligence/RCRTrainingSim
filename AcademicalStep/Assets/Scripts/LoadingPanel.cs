using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPanel : MonoBehaviour
{
    public GameObject loadingIcon;

    // Start is called before the first frame update
    void Start()
    {
        
        StepManager.OnLoading += HandleLoading;    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleLoading()
    {

        
    }
}
