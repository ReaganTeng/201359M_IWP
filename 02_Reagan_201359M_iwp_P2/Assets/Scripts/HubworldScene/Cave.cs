using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Cave : Interactables
{
    //TextMeshProUGUI textprompt;

    //bool buttonpressed;

    public override void Update()
    {

        base.Update();
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {

        base.OnTriggerEnter2D(other);


    }

    public override void Interact()
    {
        base.Interact();


        SceneManager.LoadScene("GameScene");
    }

    public override void OnTriggerExit2D(Collider2D other)
    {

        base.OnTriggerExit2D(other);


    }
}
