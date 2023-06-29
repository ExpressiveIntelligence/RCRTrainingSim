using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePrefab;
    public GameObject dialogueGO;
    public Dialogue dialogue;
    //TODO: Add references to all relevant gameobject components that will need content updated. 
    //e.g. text

    // Start is called before the first frame update
    void Start()
    {
        this.dialoguePrefab = (GameObject)Resources.Load("Dialogue");
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RenderDialogueFromFragment(SerializedFragment serializedFragment) 
    {
        //check if background is initialized
        if (this.dialogueGO == null)
        {
            this.InstantiateEmptyDialogueFromPrefab();
        }

        //update speaker label
        this.dialogue.SetSpeakerText(serializedFragment.speakerID);
        this.dialogue.SetContentText(serializedFragment.content);


    }

    public void InstantiateEmptyDialogueFromPrefab() 
    {
        if (this.dialoguePrefab != null)
        {
            //TODO: don't hardcode these coordinates, but not super urgent until we need resolution scaling.
            this.dialogueGO = Instantiate(this.dialoguePrefab, new Vector3(5.85f, 0, -0.1f), Quaternion.identity);
            this.dialogue = this.dialogueGO.GetComponent<Dialogue>();
            Debug.Log("Empty dialogue instantiated.");

        }
        else
        {
            Debug.Log("Attempted to instantiate empty background gameobject without prefab!");
        }

    }
}
