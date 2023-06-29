using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    //TODO: Not critical, but we could add safety checks for these references in the prefab.
    public TMP_Text SpeakerText;
    public TMP_Text ContentText;

    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSpeakerText(string speakerName) 
    {
        this.SpeakerText.text = speakerName;
    }

    public void SetContentText(string rawContent) 
    {
        this.ContentText.text = rawContent;
    }
}
