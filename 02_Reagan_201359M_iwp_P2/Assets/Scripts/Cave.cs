using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Cave : MonoBehaviour
{
    TextMeshProUGUI textprompt;

    bool buttonpressed;

    void Awake()
    {
        buttonpressed = false;
        textprompt = GetComponentInChildren<TextMeshProUGUI>();
       
        textprompt.enabled = false;

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && textprompt.enabled
            && !buttonpressed)
        {
            SceneManager.LoadScene("GameScene");
            //Debug.Log("SHOP TOGGLED");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            buttonpressed = true;
        }
        else
        {
            buttonpressed = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            textprompt.enabled = true;
        }

    }


    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            textprompt.enabled = false;
        }
    }
}
