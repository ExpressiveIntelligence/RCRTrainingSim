using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Choice : MonoBehaviour
{
    public TMP_Text choiceText;
    public string choiceId;


    // Start is called before the first frame update
    void Start()
    {
           
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetChoiceText(string content) 
    {
        this.choiceText.text = content;
    }


}
