using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceButton : MonoBehaviour
{
    //TODO: eventually should probably make this a state machine
    public bool isHover;
    public SpriteRenderer choiceButtonSR;
    public Color idleColor = new Color(.25f,.25f,.25f,1f);
    public Color hoverColor = new Color(.35f, .35f, .35f, 1f);

    public GameObject loadingPanel;

    // Start is called before the first frame update
    void Start()
    {
        this.loadingPanel = GameObject.FindWithTag("Loading").transform.GetChild(0).gameObject; //get reference to loading panel for coroutine
        this.isHover = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseOver()
    {
        if (!isHover)
        {
            this.isHover = true;
            this.choiceButtonSR.color = this.hoverColor;
        }
    }

    void OnMouseDown() 
    {
        //this.loadingPanel.SetActive(true);
        //StartCoroutine(SubmitChoice());
        this.transform.parent.GetComponent<Choice>().SubmitChoiceToDialogue();
    }

    void OnMouseExit()
    {
        if (isHover)
        {
            this.isHover = false;
            this.choiceButtonSR.color = this.idleColor;
        }
    }

    IEnumerator TurnOnLoadingPanel() 
    {
        this.loadingPanel.SetActive(true);
        yield return null;
    }
    IEnumerator SubmitChoice() 
    {
        this.transform.parent.GetComponent<Choice>().SubmitChoiceToDialogue();
        yield return null;
    }

}
