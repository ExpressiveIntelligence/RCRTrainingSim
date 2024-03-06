using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Core singleton for application. Used to navigate across managers and key game objects
[DefaultExecutionOrder(5)]
public class GameSession : MonoBehaviour
{
    // Singleton instance variable
    public static GameSession instance = null;

    [SerializeField]
    private Academical.DialoguePanel _dialoguePanel;

    [SerializeField]
    private Button _refreshButton;

    public Character ned;
    public Character brad;
    public Background background;

    public int backgroundCounter = 0; //counts how many choices have passed between backgrounds
    public int backgroundRotation = 3; //indicates how many choices should pass before changing backgrounds

    public GameObject loadingPanel;

    public string participantId = "participant";
    public GoogleSheetsManager sheetManager;

    // Awake is used to instantiate class as singleton
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StepManager.OnReady += HandleStepManagerOnReady;

        _dialoguePanel.OnChoiceSelected += HandleChoiceSelected;
        _refreshButton.onClick.AddListener(HandleRefreshButtonClick);

        //Initializes Step Interpreter and render the first fragment
        StepManager.instance.InitializeStepStoryAssembler();
    }

    private void HandleChoiceSelected(string choiceID)
    {
        StepManager.instance.Select(choiceID);
    }

    private void HandleRefreshButtonClick()
    {
        StepManager.instance.Reload();
    }

    private void HandleStepManagerOnReady()
    {
        //Instantiate scene based on fragment load
        UpdateVisuals();
    }

    public void SaveFragmentHistory() 
    {
        sheetManager.SavePlaythroughData(this.participantId, StepManager.instance.fragmentHistory);
    }

    public void UpdateVisuals()
    {
        var currentSerializedFragment = StepManager.instance.currentFragment;

        //update brad and ned
        if (currentSerializedFragment.characters[0].tags.ContainsKey("expression"))
        {
            Debug.Log("Found tag to render with: " + currentSerializedFragment.characters[0].tags["expression"]);
            this.brad.SetSprite(currentSerializedFragment.characters[0].tags["expression"]);
        }
        else
        {
            this.brad.SetSprite("fallback");
        }

        if (currentSerializedFragment.characters[1].tags.ContainsKey("expression"))
        {
            Debug.Log("Found tag to render with: " + currentSerializedFragment.characters[1].tags["expression"]);
            this.ned.SetSprite(currentSerializedFragment.characters[1].tags["expression"]);
        }
        else
        {
            this.ned.SetSprite("fallback");
        }

        //change background every 3 turns
        if (this.backgroundCounter == 0)
        {
            this.background.SetSprite("random");
        }
        backgroundCounter++;
        if (this.backgroundCounter >= this.backgroundRotation)
        {
            this.backgroundCounter = 0;
        }

        _dialoguePanel.SetSpeakerName(currentSerializedFragment.speakerID);
        _dialoguePanel.SetTextContent(currentSerializedFragment.content);
        _dialoguePanel.SetChoices(currentSerializedFragment.choices);

    }
}
