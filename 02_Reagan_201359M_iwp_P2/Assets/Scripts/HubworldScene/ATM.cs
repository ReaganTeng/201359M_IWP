using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ATM : Interactables
{
    [HideInInspector]
    public HubWorldMenuManager hubWorldMenuManager;

    [HideInInspector]
    public GameObject atmPanel;

    public override void Awake()
    {
        base.Awake();

        hubWorldMenuManager = GameObject.FindGameObjectWithTag("GameMGT").GetComponent<HubWorldMenuManager>();
        atmPanel = hubWorldMenuManager.ATMPanel;
    }



    public override void Update()
    {

        base.Update();

        if (atmPanel == null)
        {
            atmPanel = hubWorldMenuManager.ATMPanel;

        }

        //if (Input.GetKeyDown(KeyCode.E)
        //    && textPrompt.enabled)
        //{
        //    hubWorldMenuManager.togglePanel(atmPanel);
        //}
    }



    public override void Interact()
    {
        base.Interact();

        
        hubWorldMenuManager.togglePanel(atmPanel);
        
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
