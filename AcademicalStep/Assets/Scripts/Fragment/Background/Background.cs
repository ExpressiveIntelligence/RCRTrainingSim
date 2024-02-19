using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Calypso;

public class Background : MonoBehaviour
{
    protected SpriteController m_spriteController;
    private void Awake() 
    {
        m_spriteController = GetComponent<SpriteController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
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
