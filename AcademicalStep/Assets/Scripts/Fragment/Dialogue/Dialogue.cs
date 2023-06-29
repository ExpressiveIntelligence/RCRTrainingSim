using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/** Goal one: 
 * 
 */
public class Dialogue : MonoBehaviour
{
    public GameSession gameSession;
    //TODO: Not critical, but we could add safety checks for these references in the prefab.
    public TMP_Text speakerText;
    public TMP_Text contentText;

    //contains all active choices
    public Choice[] choices;

    //TODO: Hacky probably?! Linked in prefab editor.
    //We'll use these to hardcode in our choices for our basic interface. 
    //We can always swap this piece out when/if we decide to make our controller more sophisticated.
    public Choice choice0;
    public Choice choice1;
    public Choice choice2;
    public Choice choice3;
    public Choice choice4;
    public Choice choice5;

    // Start is called before the first frame update
    void Start()
    {
        this.gameSession = GameSession.instance;


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SubmitChoiceToStoryAssembler(string choiceId) 
    {

        SerializedFragment temp = this.gameSession.stepManager.Select(choiceId.ToLower());
        Debug.Log(temp.ToString());
    }

    public void SetChoices(SerializedChoice[] choices) 
    {
        Debug.Log("Choice Length: " + choices.Length);
        //TODO: Hack because of a race condition in start isn't assigning this in time.
        if (this.choices.Length == 0 || this.choices == null)
        {
            this.InstantiateChoiceGameObjects();
        }
        //look for no choices or too many choices and print a warning.
        //TODO: Centralized logging
        if (choices == null || choices.Length == 0)
        {
            Debug.Log("STEP WARNING: No choices provided to Dialogue.");
        }
        else if (choices.Length >= this.choices.Length) 
        {
            Debug.Log("STEP WARNING: More than 6 choices provided.");
        }
        
        for (int i = 0; i < this.choices.Length; i++) 
        {
            //if there's a choice in the serialized array, render it
            if (i < choices.Length)
            {
                this.choices[i].gameObject.SetActive(true);
                this.choices[i].SetChoiceText(choices[i].text);
                this.choices[i].choiceId = choices[i].id;
            }
            else 
            {
                this.choices[i].gameObject.SetActive(false);
            }
        }


    }

    public void InstantiateChoiceGameObjects() 
    {
        this.choices = new Choice[6];
        this.SetChoiceAtIndex(0, this.choice0);
        this.SetChoiceAtIndex(1, this.choice1);
        this.SetChoiceAtIndex(2, this.choice2);
        this.SetChoiceAtIndex(3, this.choice3);
        this.SetChoiceAtIndex(4, this.choice4);
        this.SetChoiceAtIndex(5, this.choice5);
    }

    public void SetChoiceAtIndex(int index, Choice choice) 
    {
        Debug.Log("Reached choice insert");
       this.choices[index] = choice;        
    }

    public void SetSpeakerText(string speakerName) 
    {
        this.speakerText.text = speakerName;
    }

    public void SetContentText(string dialogueContent) 
    {
        this.contentText.text = dialogueContent;
    }
}
