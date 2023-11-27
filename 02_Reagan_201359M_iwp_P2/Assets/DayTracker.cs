using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DayTracker : MonoBehaviour
{
    GameObject dayPanel;
    TextMeshProUGUI textprompt;

    bool buttonpressed;

    void Awake()
    {
        buttonpressed = false;
        textprompt = GetComponentInChildren<TextMeshProUGUI>();
        dayPanel = GameObject.FindGameObjectWithTag("DayPanel");

        if (dayPanel.activeSelf)
        {
            dayPanel.SetActive(!dayPanel.activeSelf);
        }
        textprompt.enabled = false;

    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && textprompt.enabled
            && !buttonpressed)
        {
            dayPanel.SetActive(!dayPanel.activeSelf);
            Debug.Log("SHOP TOGGLED");
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
