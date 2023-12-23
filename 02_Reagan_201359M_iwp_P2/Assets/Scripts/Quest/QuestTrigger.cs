// QuestTrigger.cs
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class QuestTrigger : Interactables
{
    public string questGivername;

    public string questName;

    //[HideInInspector]
    public int questGiverid;
    //[Header("Quest Information")]
    [HideInInspector]
    public string QuestDes;
    //[Tooltip("The number of enemies to kill for this quest.")]
    [HideInInspector]
    public int requiredCount;


    //[HideInInspector]
    public QuestManager QM;

    bool givenQuest;

    public override void Awake()
    {
        base.Awake();
        givenQuest = false;
        QM = GameObject.FindGameObjectWithTag("GameMGT").GetComponent<QuestManager>();


    }

    public override void Update()
    {
        //base.Update();

        if (Input.GetKeyDown(KeyCode.E)
            && textPrompt.enabled)
        {
            //CHECK IF QUEST IS ALREADY GIVEN
            if (!givenQuest)
            {
                Debug.Log("TRIGGERED");
                requiredCount = 3;
                //AddQuest(string questName, string description, int requiredCount);
                QM.AddQuest(questName, QuestDes, requiredCount, questGiverid);
                givenQuest = true;
            }
            else
            {
                //CHECK IF QUEST IS ALD COMPLETED
                Quest quest = QM.quests.Find(template
                    => template.hiddenVariables.questGiverID == questGiverid);

                if (quest.hiddenVariables.isCompleted)
                {
                    Debug.Log("QUEST COMPLETED");
                    QM.quests.Remove(quest);
                    Destroy(this.gameObject);
                }
                else
                {
                    Debug.Log("QUEST NOT COMPLETED");
                }
            }
        }
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {

        base.OnTriggerEnter2D (other);

        
    }

    public override void OnTriggerExit2D(Collider2D other)
    {

        base.OnTriggerExit2D(other);


    }

}

