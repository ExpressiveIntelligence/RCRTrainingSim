using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryPanelController : MonoBehaviour
{
    public GameObject contentContainer;
    public GameObject historyEntryPrefab;

    private List<GameObject> historyEntries = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void show() 
    {
        foreach (GameObject entry in historyEntries) 
        {
            Destroy(entry);
        }
        historyEntries.Clear();

        foreach (SerializedFragment fragment in StepManager.instance.fragmentHistory) 
        {
            GameObject entry = Instantiate(historyEntryPrefab, contentContainer.transform);
            entry.GetComponentInChildren<TMPro.TMP_Text>().text = fragment.content;
            historyEntries.Add(entry);
        }

        this.gameObject.SetActive(true);
    
    }
    
    public void hide() 
    {
        this.gameObject.SetActive(false);
    }  
}
