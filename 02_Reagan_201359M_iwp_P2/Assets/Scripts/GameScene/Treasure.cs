using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Item;
using System;
using Random = UnityEngine.Random;
using static PowerUp;

public class Treasure : Interactables
{
    public List<Sprite> chestSprites;
    public GameObject itemPrefab;
    public GameObject powerUpPrefab;

    [HideInInspector]
    public AudioSource AS;

    public AudioClip openSound;

    bool unlocked;
    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        AS = GetComponent<AudioSource>();
        unlocked = false;
        sr.sprite = chestSprites[0];
    }

    // Update is called once per frame
    public override void Update()
    {
        //base.Update();
        //if(Input.GetKeyDown(KeyCode.E)
        //    && textPrompt.enabled
        //    && !unlocked)
        //{
        //    //Debug
        //    UnlockChest();
        //}
    }


    public override void Interact()
    {
        base.Interact();
        //Debug.Log("INTERACTED");
        if (!unlocked)
        {
            AS.clip = openSound;
            AS.Play();
            //Debug
            UnlockChest();
        }
    }


    void UnlockChest()
    {
        //drop 10 random gems
        for (int i = 0; i < 10; i++)
        {
            //int enumLength = Enum.GetValues(typeof(ItemType)).Length - 1;
            ItemType itemchosen = (ItemType)(Random.Range(0, 3));
            GameObject item = Instantiate(itemPrefab, transform.position, Quaternion.identity);
            item.GetComponent<Item>().SetItem(itemchosen, 99);
        }

      
        //drop powerups
        int powerupdropper = (Random.Range(1, 3));
        if (powerupdropper % 2 == 0)
        {
            PowerUps powerupchosen = (PowerUps)(Random.Range(0,
                Enum.GetValues(typeof(PowerUps)).Length - 1));
            //PowerUps powerupchosen = PowerUps.ONE_HIT;

            GameObject item = Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
            item.GetComponent<PowerUp>().SetPowerUpItem(powerupchosen, 1);
        }
        //

        //FOR PRESENTATION PURPOSE ONLY
        presenterMode(transform.position);

        //textPrompt.enabled = false;
        sr.sprite = chestSprites[1];
        unlocked = true;
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")
            && !unlocked)
        {
            //textPrompt.enabled = true;
            //unlocked = true;
        }
    }

    public override void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")
            //&& !unlocked
            )
        {
            //textPrompt.enabled = false;
            //unlocked = true;
        }
    }


    void presenterMode(Vector3 pos)
    {

        GameObject item = Instantiate(powerUpPrefab, pos, Quaternion.identity);
        item.GetComponent<PowerUp>().SetPowerUpItem(PowerUps.SPIRIT_FIRE, 1);


        item = Instantiate(powerUpPrefab, pos, Quaternion.identity);
        item.GetComponent<PowerUp>().SetPowerUpItem(PowerUps.GEM_WISDOM, 1);

        item = Instantiate(powerUpPrefab, pos, Quaternion.identity);
        item.GetComponent<PowerUp>().SetPowerUpItem(PowerUps.MINER_SENSE, 1);

        item = Instantiate(powerUpPrefab, pos, Quaternion.identity);
        item.GetComponent<PowerUp>().SetPowerUpItem(PowerUps.ONE_HIT, 1);

        item = Instantiate(powerUpPrefab, pos, Quaternion.identity);
        item.GetComponent<PowerUp>().SetPowerUpItem(PowerUps.GHOST, 1);

    }
}
