using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Interactables : MonoBehaviour
{
    [HideInInspector]
    public SpriteRenderer sr;
    [HideInInspector]
    public TextMeshProUGUI textPrompt;

    public virtual void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        textPrompt = GetComponentInChildren<TextMeshProUGUI>();
        textPrompt.enabled = false;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //textPrompt.enabled = true;
        }
    }


    public virtual void Interact()
    {

    }

    public virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //textPrompt.enabled = false;
        }
    }
}
