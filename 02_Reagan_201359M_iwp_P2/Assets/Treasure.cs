using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Item;
using System;
using Random = UnityEngine.Random;

public class Treasure : MonoBehaviour
{
    public List<Sprite> chestSprites;

    SpriteRenderer sr;
    TextMeshProUGUI textPrompt;

    public GameObject itemPrefab;

    bool unlocked;
    // Start is called before the first frame update
    void Awake()
    {
        unlocked = false;
        sr = GetComponent<SpriteRenderer>();

        textPrompt = GetComponentInChildren<TextMeshProUGUI>();

        textPrompt.enabled = false;
        sr.sprite = chestSprites[0];

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)
            && textPrompt.enabled
            && !unlocked)
        {
            UnlockChest();
        }
    }


    void UnlockChest()
    {
        unlocked = true;
        //drop 10 random items
        for (int i = 0; i < 10; i++)
        {
            int enumLength = Enum.GetValues(typeof(ItemType)).Length - 1;
            ItemType itemchosen = (ItemType)(Random.Range(0, enumLength));

            GameObject item = Instantiate(itemPrefab, transform.position, Quaternion.identity);
            item.GetComponent<Item>().SetItem(itemchosen, 99);
        }
        sr.sprite = chestSprites[1];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")
            && !unlocked)
        {
            textPrompt.enabled = true;
            //unlocked = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !unlocked)
        {
            textPrompt.enabled = false;
            //unlocked = true;
        }
    }

}
