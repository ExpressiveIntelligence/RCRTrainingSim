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

    // Start is called before the first frame update
    void Start()
    {
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

    void OnMouseExit()
    {
        if (isHover)
        {
            this.isHover = false;
            this.choiceButtonSR.color = this.idleColor;
        }
    }
}
