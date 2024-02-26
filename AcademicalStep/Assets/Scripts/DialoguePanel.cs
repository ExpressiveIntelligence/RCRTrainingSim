using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Academical
{
    public class DialoguePanel : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _speakerName;

        [SerializeField]
        private TMP_Text _textContent;

        [SerializeField]
        private Button[] _choiceButtons;

        public UnityAction<string> OnChoiceSelected;

        public void SetSpeakerName(string name)
        {
            _speakerName.text = name;
        }

        public void SetTextContent(string text)
        {
            _textContent.text = text;
        }

        public void SetChoices(SerializedChoice[] choices)
        {
            if (choices.Length >= _choiceButtons.Length)
            {
                Debug.Log("STEP WARNING: More than 6 choices provided.");
            }

            for (int i = 0; i < _choiceButtons.Length; i++)
            {
                //if there's a choice in the serialized array, render it
                if (i < choices.Length)
                {
                    string choiceID = choices[i].id.ToLower();
                    _choiceButtons[i].GetComponentInChildren<TMP_Text>().text = choices[i].text;
                    _choiceButtons[i].onClick.RemoveAllListeners();
                    _choiceButtons[i].onClick.AddListener(() =>
                    {
                        OnChoiceSelected?.Invoke(choiceID);
                    });
                    _choiceButtons[i].gameObject.SetActive(true);
                }
                else
                {
                    _choiceButtons[i].onClick.RemoveAllListeners();
                    _choiceButtons[i].gameObject.SetActive(false);
                }
            }
        }

        public void ClearChoices()
        {
            foreach (Button btn in _choiceButtons)
            {
                btn.GetComponent<TMP_Text>().text = "";
            }
        }
    }

}
