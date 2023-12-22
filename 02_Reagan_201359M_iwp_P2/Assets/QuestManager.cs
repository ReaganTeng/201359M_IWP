// QuestManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static ShopItem;
using System.Linq;

public class QuestManager : MonoBehaviour
{

    public GameObject questUIContent;
    public GameObject questPrefab;
    public List<Quest> quests = new List<Quest>();

    public TextMeshProUGUI questText;


    void Awake()
    {
        //InitializeQuests();

    }

    public void InitializeQuests()
    {
        
        //foreach (Quest quest in quests)
        //{
        //    //quest.Initialize(1);
        //}
    }


    public void AddQuest(string questName, string description, int requiredCount, int QuestGiverId)
    {
        Quest newQuest = new Quest
        {
            hiddenVariables = new Quest.HiddenVariables
            {
                questGiverID = QuestGiverId,
                questName = questName,
                description = description,
                requiredCount = requiredCount,
                isCompleted = false,
                currentCount = 0,
            }
        };
        Debug.Log("QUEST ADDED");

        quests.Add(newQuest);

        
        foreach (Quest item in quests)
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject itemObject = Instantiate(questPrefab, questUIContent.transform);
                itemObject.GetComponent<QuestUI>().UpdateUI(item);
                RectTransform[] children = itemObject.GetComponentsInChildren<RectTransform>();

                //DISPLAY THE CONTENTS
                //RectTransform itemTransform
                //    = children.FirstOrDefault(child =>
                //    child.CompareTag("QuestPanel")).GetComponent<RectTransform>();

                RectTransform shopRectTransform = questUIContent.GetComponent<RectTransform>();
                //shopRectTransform.sizeDelta = new Vector2(shopRectTransform.sizeDelta.x, itemTransform.sizeDelta.y * shopItems.Count);


                //itemObject.GetComponentInChildren<TextMeshProUGUI>().text
                //    = $"{item.itemName} - {item.price} coins";
                //Button buyButton = itemObject.GetComponentInChildren<Button>();
                //buyButton.onClick.AddListener(() => BuyItem(item));
                //Debug.Log($"FINAL HEIGHT {itemTransform.rect.height}");
            }
        }
    }



    private void Update()
    {
        //UpdateQuestProgress("Killing Monster");
    }

    //UPDATE WHENEVER PLAYER PERFORMS CERTAIN SPECIFIC ACTIONS
    public void UpdateQuestProgress(string questName)
    {
        //questText.text = "Completed";
        Quest quest = quests.Find(q => q.hiddenVariables.questName == questName);

        if (quest != null && !quest.hiddenVariables.isCompleted)
        {
            Debug.Log("QUEST UPDATED");
            quest.UpdateProgress();
            CheckQuestCompletion(quest);
        }
    }

    void CheckQuestCompletion(Quest quest)
    {
        if (quest.hiddenVariables.isCompleted)
        {
            Debug.Log($"Quest '{quest.hiddenVariables.questName}' completed!");
            questText.text = "Completed";
            // Provide rewards or trigger other events
        }
    }
}
