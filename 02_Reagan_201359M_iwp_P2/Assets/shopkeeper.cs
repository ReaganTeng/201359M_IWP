using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class shopkeeper : MonoBehaviour
{
    GameObject shopPanel;
    TextMeshProUGUI textprompt;

    bool buttonpressed;

    void Awake()
    {
        buttonpressed = false;
        textprompt = GetComponentInChildren<TextMeshProUGUI>();
        shopPanel = GameObject.FindGameObjectWithTag("ShopPanel");
        shopPanel.GetComponent<Shop>().DisplayItems();

        //shopPanel.GetComponent<Shop>().itemContainer = shopPanel.gameObject.GetComponent<Transform>();

        if (shopPanel.activeSelf)
        {
            shopPanel.SetActive(!shopPanel.activeSelf);
        }
        textprompt.enabled = false;

    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && textprompt.enabled
            && !buttonpressed)
        {
            shopPanel.SetActive(!shopPanel.activeSelf);
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

    public void closePanel()
    {
        shopPanel.SetActive(!shopPanel.activeSelf);
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
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
