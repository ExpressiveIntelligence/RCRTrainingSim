using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticipantInfoPanel : MonoBehaviour
{
    public TMPro.TMP_InputField inputField;

    void OnEnable()
    {
        inputField.onValueChanged.AddListener(HandleParticipantIdChanged);
    }

    void OnDisable()
    {
        inputField.onValueChanged.RemoveListener(HandleParticipantIdChanged);
    }

    private void HandleParticipantIdChanged(string value)
    {
        GameSession.instance.participantId = value;
    }
}
