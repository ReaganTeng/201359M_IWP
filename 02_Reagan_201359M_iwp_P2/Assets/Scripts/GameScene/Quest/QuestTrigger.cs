// QuestTrigger.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static Item;
using static PowerUp;
using static Projectile;
using Random = UnityEngine.Random;




public class QuestTrigger : DialogueTrigger
{

    //public QuestType questtype;

    [HideInInspector]
    public string questGivername;

    [HideInInspector]
    public string questName;

    [HideInInspector]
    public int questGiverid;
    //[Header("Quest Information")]
    [HideInInspector]
    public string QuestDes;
    //[Tooltip("The number of enemies to kill for this quest.")]
    [HideInInspector]
    public int requiredCount;

    [HideInInspector]
    public QuestManager QM;

    [HideInInspector]
    public bool givenQuest;

    [HideInInspector]
    public Dictionary<QuestType, Action> itemActions = new Dictionary<QuestType, Action>();


    [HideInInspector] QuestType questType;

    public GameObject itemPrefab;
    public GameObject powerUpPrefab;

    public override void Awake()
    {
        base.Awake();
        givenQuest = false;
        QM = GameObject.FindGameObjectWithTag("GameMGT").GetComponent<QuestManager>();
        dialogue.InitializeQuestGiver(ref QM, this);
        ImplementQuestType();


    }

    public void ImplementQuestType()
    {
        QuestType questTypeChosen = (QuestType)(Random.Range(0,
                        Enum.GetValues(typeof(QuestType)).Length - 1));
        questType = questTypeChosen;

        itemActions = new Dictionary<QuestType, Action>
        {
            { QuestType.MONSTER_SLAYING, () =>
            {
                questName = "Killing Monsters";
                QuestDes = "Kill some monsters";
            }
            },
            
             //
        };

        if (itemActions.ContainsKey(questType))
        {
           
                itemActions[questType].Invoke();
            
        }
    }


    public override void Update()
    {
        base.Update();

        //if(givenQuest)
        //{
        //    Debug.Log("MUTEEEE");
        //}

        
    }

    public override void Interact()
    {
        
        if (!givenQuest)
        {
            base.Interact();
        }
        //else
        //{

        //Quest quest = QM.quests.Find(template
        //    => template.hiddenVariables.questGiverID == questGiverid);
        //if (quest != null &&
        //    quest.hiddenVariables.isCompleted)
        //{
        //    Debug.Log("QUEST COMPLETED");
        //    int idx = QM.quests.IndexOf(quest);
        //    GameObject questUI = QM.questUIContent.GetComponent<RectTransform>().GetChild(idx).gameObject;
        //    Destroy(questUI);
        //    QM.quests.Remove(quest);

        //    for (int i = 0; i < 10; i++)
        //    {
        //        //int enumLength = Enum.GetValues(typeof(ItemType)).Length - 1;
        //        ItemType itemchosen = (ItemType)(Random.Range(0, 3));
        //        GameObject item = Instantiate(itemPrefab, transform.position, Quaternion.identity);
        //        item.GetComponent<Item>().SetItem(itemchosen, 99);
        //    }

        //    //drop powerups
        //    int powerupdropper = (Random.Range(1, 3));
        //    if (powerupdropper % 2 == 0)
        //    {
        //        PowerUps powerupchosen = (PowerUps)(Random.Range(0,
        //            Enum.GetValues(typeof(PowerUps)).Length - 1));

        //        GameObject item = Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
        //        item.GetComponent<PowerUp>().SetPowerUpItem(powerupchosen, 1);
        //    }
        //}
        //Destroy(gameObject);


        //    //List<Quest> quests = QM.quests.FindAll(template
        //    //   => template.hiddenVariables.questGiverID == questGiverid);
        //    //foreach (Quest q in quests)
        //    //{
        //    //    if (q != null &&
        //    //        q.hiddenVariables.isCompleted)
        //    //    {
        //    //        //Debug.Log("QUEST COMPLETED");
        //    //        int idx = QM.quests.IndexOf(q);
        //    //        GameObject questUI = QM.questUIContent.GetComponent<RectTransform>().GetChild(idx).gameObject;
        //    //        Destroy(questUI);
        //    //        QM.quests.Remove(q);
        //    //        QM.updateContentSize();
        //    //    }
        //    //}
        //}

        //QM.updateContentSize();



    }



    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }


    public override void OnTriggerExit2D(Collider2D other)
    {

        base.OnTriggerExit2D(other);


    }


    //public void OnDestroy()
    //{
    //    //Quest quest = QM.quests.Find(template
    //    //        => template.hiddenVariables.questGiverID == questGiverid);
    //    //if (quest != null &&
    //    //       quest.hiddenVariables.isCompleted)
    //    {
    //        //drop 10 random gems
    //        for (int i = 0; i < 10; i++)
    //        {
    //            //int enumLength = Enum.GetValues(typeof(ItemType)).Length - 1;
    //            ItemType itemchosen = (ItemType)(Random.Range(0, 3));
    //            GameObject item = Instantiate(itemPrefab, transform.position, Quaternion.identity);
    //            item.GetComponent<Item>().SetItem(itemchosen, 99);
    //        }

    //        //drop powerups
    //        int powerupdropper = (Random.Range(1, 3));
    //        if (powerupdropper % 2 == 0)
    //        {
    //            PowerUps powerupchosen = (PowerUps)(Random.Range(0,
    //                Enum.GetValues(typeof(PowerUps)).Length - 1));

    //            GameObject item = Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
    //            item.GetComponent<PowerUp>().SetPowerUpItem(powerupchosen, 1);
    //        }
    //        //
    //    }
    //}

}

