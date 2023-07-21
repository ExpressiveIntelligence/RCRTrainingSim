using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshStepButton : MonoBehaviour
{
    //TODO: eventually should probably make this a state machine
    public bool isHover;
    public SpriteRenderer stepRefreshButtonSR;
    public Color idleColor = new Color(.25f, .25f, .25f, 1f);
    public Color hoverColor = new Color(.35f, .35f, .35f, 1f);
    public StepManager stepManager;

    // Start is called before the first frame update
    void Start()
    {
        this.isHover = false;
        this.stepManager = this.transform.parent.GetComponent<StepManager>();
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
            this.stepRefreshButtonSR.color = this.hoverColor;
        }
    }

    void OnMouseDown()
    {
        //this.transform.parent.GetComponent<Choice>().SubmitChoiceToDialogue();
        SerializedFragment temp = this.stepManager.Reload();
        Debug.Log("Successfully refreshed Step corpus");
        this.stepManager.gameSession.fragmentManager.currentSerializedFragment = temp;
        //TODO: do we want to clear on refresh?
        this.stepManager.gameSession.fragmentManager.fragmentHistory = new List<SerializedFragment>();
        this.stepManager.gameSession.fragmentManager.fragmentHistory.Add(temp);
        this.stepManager.gameSession.fragmentManager.RenderFromCurrentFragment();
    }

    void OnMouseExit()
    {
        if (isHover)
        {
            this.isHover = false;
            this.stepRefreshButtonSR.color = this.idleColor;
        }
    }
}
