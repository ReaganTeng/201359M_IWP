using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DayTracker : Interactables
{
    //[HideInInspector]
    public HubWorldMenuManager hubWorldMenuManager;

    //[HideInInspector]
    public GameObject dayPanel;

    public override void Awake()
    {
        base.Awake();

        hubWorldMenuManager = GameObject.FindGameObjectWithTag("GameMGT").GetComponent<HubWorldMenuManager>();
        

        //dayPanel = hubWorldMenuManager.DayPanel;
    }


    public override void Update()
    {

        base.Update();

        if(dayPanel == null )
        {
            dayPanel = hubWorldMenuManager.DayPanel;

        }

        //if (Input.GetKeyDown(KeyCode.E)
        //    && textPrompt.enabled)
        //{
        //    hubWorldMenuManager.togglePanel(dayPanel);
        //}
    }


    public override void Interact()
    {
        base.Interact();

        
        hubWorldMenuManager.togglePanel(dayPanel);
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }

    public override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
    }
}
