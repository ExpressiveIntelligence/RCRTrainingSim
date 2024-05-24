using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(TMP_Text))]

public class Hyperlink : MonoBehaviour, IPointerClickHandler 
{
    string SURVEY_URL = "https://ucsantacruz.co1.qualtrics.com/jfe/form/SV_eOS8poNqutIKkN8";

    public void OnPointerClick(PointerEventData eventData)
    {
        TMP_Text textMeshPro = GetComponent<TMP_Text>();

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshPro, eventData.position, null);

        if(linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = textMeshPro.textInfo.linkInfo[linkIndex];
            Debug.Log("linkIndex Found. LinkInfo is: " + linkInfo + ", LinkID is: " + linkInfo.GetLinkID());
            if(linkInfo.GetLinkID() == "surveylink"){
                Debug.Log("reached inside of surveylink");
                Application.OpenURL(SURVEY_URL);
            }
        } else {
            Debug.Log("No linkIndex found.");
        }
    }
}
