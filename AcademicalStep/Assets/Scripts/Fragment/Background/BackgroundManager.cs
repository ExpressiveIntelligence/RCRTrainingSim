using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public GameObject backgroundPrefab;
    public GameObject background;
    public SpriteRenderer bgSpriteRenderer;

    // Start is called before the first frame update.
    void Start()
    {
        //loads the backgroundPrefab resource
        //TODO: Use addressables to load prefabs without manual linking.


        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InstantiateEmptyBackgroundFromPrefab() 
    {
        if (this.backgroundPrefab != null)
        {
            this.background = (GameObject)Instantiate(Resources.Load("Background"), new Vector3(0, 0, 0.2f), Quaternion.identity);
            this.bgSpriteRenderer = this.background.GetComponent<SpriteRenderer>();
            Debug.Log("Empty Background Instantiated.");
        }
        else 
        {
            Debug.Log("Attempted to instantiate empty background gameobject without prefab!");
        }
        
    }

    // Create Background
    public void RenderBackgroundFromFragment(SerializedFragment serializedFragment) 
    {
        
        //check if background is initialized
        if (this.background == null) 
        {
            this.InstantiateEmptyBackgroundFromPrefab();
        }

        //Null check, print error and render deep blue background
        if (string.IsNullOrEmpty(serializedFragment.backgroundPath))
        {
            Debug.Log("No background image path provided. Please provide appropriate path to resource in authoring tool.");
        }
        else 
        {
            //attempt to load sprite
            var bgSprite = Resources.Load<Sprite>(serializedFragment.backgroundPath);
            //check for valid load, render amber screen on failed file lookup
            if (bgSprite != null)
            {
                this.bgSpriteRenderer.sprite = bgSprite;
            }
            else 
            {
                Debug.Log("Background image path corrupted. Please provide appropriate path to resource in authoring tool. Provided path: " + serializedFragment.backgroundPath);
                this.bgSpriteRenderer.color = Color.red;
            }
        }


     
    }
    // Update Background from Fragment

    // 
}
