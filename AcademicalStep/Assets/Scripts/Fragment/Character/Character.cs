using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Calypso;

//TODO: Create a Serialized Version
public class Character : MonoBehaviour
{ 
    protected SpriteController m_spriteController;
    // TODO: establish any unique properties or metadata we need from the character
    // E.G. map of properties
    private void Awake()
    {
        m_spriteController = GetComponent<SpriteController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.SetSprite("fallback");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSprite(params string[] tags)
    {
        if ( m_spriteController == null ) return;

        m_spriteController.SetSpriteFromTags( tags );
    }
}
