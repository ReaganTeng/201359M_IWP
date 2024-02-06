using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Item;
using static PowerUp;
using Random = UnityEngine.Random;

public class QuestUI : MonoBehaviour
{

    public GameObject itemPrefab;
    public GameObject powerUpPrefab;

    public TextMeshProUGUI questNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI progressText;

    public string questStr;

    public Button CompleteButton;


    [HideInInspector] public int giverID;

    public void Awake()
    {
        CompleteButton.gameObject.SetActive(false);
    }

    public void UpdateUI(Quest quest)
    {
        if(questStr == "")
        {
            questStr = quest.hiddenVariables.questName;
        }
        questNameText.text = "Quest: " + quest.hiddenVariables.questName;
        descriptionText.text = "Description: " + quest.hiddenVariables.description;
        progressText.text = "Progress: " + quest.hiddenVariables.currentCount + " / " + quest.hiddenVariables.requiredCount;


        //SET ACTIVE COMPLETE BUTTON TO DECLARE THE PLAYER HAS COMPLETED QUEST
        if (quest.hiddenVariables.currentCount >= quest.hiddenVariables.requiredCount)
        {
            CompleteButton.gameObject.SetActive(true);
            CompleteButton.onClick.AddListener(
                () =>
                {
                    CompleteQuest();
                }
                );
        }
        //Debug.Log("QUEST UI UPDATED");
    }


    

    public void CompleteQuest()
    {
       
        QuestManager QM = FindObjectOfType< QuestManager >();
        Quest quest = QM.quests.Find(template
            => template.hiddenVariables.questGiverID == giverID);

        QM.quests.Remove(quest);

        //FIND LEADING PLAYER
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Player playerScript = null;
        foreach(GameObject player in players)
        {
            playerScript = player.GetComponent<Player>();
            if(!playerScript.AIMode)
            {
                break;
            }
        }
        Vector2 playerpos = playerScript.gameObject.transform.position;
        //

        //DROP GEMS
        for (int i = 0; i < 10; i++)
        {
            //int enumLength = Enum.GetValues(typeof(ItemType)).Length - 1;
            ItemType itemchosen = (ItemType)(Random.Range(0, 3));
            GameObject item = Instantiate(itemPrefab, playerpos, Quaternion.identity);
            item.GetComponent<Item>().SetItem(itemchosen, 99);
        }

        //drop powerups
        int powerupdropper = (Random.Range(1, 3));
        if (powerupdropper % 2 == 0)
        {
            PowerUps powerupchosen = (PowerUps)(Random.Range(0,
                Enum.GetValues(typeof(PowerUps)).Length - 1));

            GameObject item = Instantiate(powerUpPrefab, playerpos, Quaternion.identity);
            item.GetComponent<PowerUp>().SetPowerUpItem(powerupchosen, 1);
        }


        Destroy(gameObject);
       
    }


    
}
