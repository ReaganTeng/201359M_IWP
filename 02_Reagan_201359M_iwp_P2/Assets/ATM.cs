using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ATM : MonoBehaviour
{
    GameObject ATMPanel;
    TextMeshProUGUI textprompt;

    bool buttonpressed;

    void Awake()
    {
        buttonpressed = false;
        textprompt = GetComponentInChildren<TextMeshProUGUI>();
        ATMPanel = GameObject.FindGameObjectWithTag("ATMPanel");

        if (ATMPanel.activeSelf)
        {
            ATMPanel.SetActive(!ATMPanel.activeSelf);
        }
        textprompt.enabled = false;

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && textprompt.enabled
            && !buttonpressed)
        {
            ATMPanel.SetActive(!ATMPanel.activeSelf);
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
