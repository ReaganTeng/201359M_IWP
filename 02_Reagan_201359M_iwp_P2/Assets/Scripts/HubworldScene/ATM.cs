using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ATM : Interactables
{
    [HideInInspector]
    public HubWorldMenuManager hubWorldMenuManager;

    public override void Awake()
    {
        base.Awake();

        hubWorldMenuManager = GameObject.FindGameObjectWithTag("GameMGT").GetComponent<HubWorldMenuManager>();
    }



    public override void Update()
    {

        base.Update();
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
