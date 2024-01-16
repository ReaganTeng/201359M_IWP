// QuestManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static ShopItem;

public class QuestManager : MonoBehaviour
{

    [HideInInspector] public MenuManager menuManager;
    [HideInInspector] public GameObject questPanel;


    public GameObject questUIContent;
    public GameObject questPrefab;

    [HideInInspector]
    public List<Quest> quests = new List<Quest>();


    [HideInInspector]
    public AudioSource AS;
    public AudioClip newQuestClip, questCompletedClip;


    public Image newQuestNotification;
    public Image questUpdatedNotification;

    void Awake()
    {
        //questText.text = "";
        //InitializeQuests();
        AS = GetComponent<AudioSource>();
        menuManager = GetComponent<MenuManager>();
        questPanel = menuManager.QuestPanel;

        newQuestNotification.enabled = false;
        questUpdatedNotification.enabled = false;

    }

    public void InitializeQuests()
    {
        
        //foreach (Quest quest in quests)
        //{
        //    //quest.Initialize(1);
        //}
    }


    public void DisableNotifications()
    {
        newQuestNotification.enabled = false;
        questUpdatedNotification.enabled = false;
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
        
        GameObject itemObject = Instantiate(questPrefab, questUIContent.transform);
        itemObject.GetComponent<QuestUI>().UpdateUI(newQuest);
        

        questUIContent.GetComponent<RectTransform>().sizeDelta =
            new Vector2(questUIContent.GetComponent<RectTransform>().sizeDelta.x
             , questUIContent.GetComponent<RectTransform>().sizeDelta.y + itemObject.GetComponent<RectTransform>().sizeDelta.y);


        itemObject.GetComponent<RectTransform>().sizeDelta
            = new Vector2(questUIContent.GetComponent<RectTransform>().sizeDelta.x
             , itemObject.GetComponent<RectTransform>().sizeDelta.y);

        //questUIContent.GetComponent<RectTransform>().sizeDelta =
        //   new Vector2(questUIContent.GetComponent<RectTransform>().sizeDelta.x
        //    , questUIContent.GetComponent<RectTransform>().sizeDelta.y + itemObject.GetComponent<RectTransform>().sizeDelta.y);


        //foreach(RectTransform trans )
        //updateContentSize();
        newQuestNotification.enabled = true;
        questUpdatedNotification.enabled = false;

        AS.clip = newQuestClip;
        AS.Play();
    }


    public void updateContentSize()
    {
        int childCount = 0;

        //GameObject itemObject = Instantiate(questPrefab);


        foreach (RectTransform trans in questUIContent.GetComponentsInChildren<RectTransform>())
        {
            QuestUI questUIComponent = trans.GetComponent<QuestUI>();

            if (questUIComponent != null)
            {
                childCount++;
                Debug.Log($"FOUND CHILD {childCount}");
            }
        }

        questUIContent.GetComponent<RectTransform>().sizeDelta =
           new Vector2(questUIContent.GetComponent<RectTransform>().sizeDelta.x
            , questPrefab.GetComponent<RectTransform>().sizeDelta.y * childCount);

        //Destroy(itemObject);

    }
    private void Update()
    {

        updateContentSize();
        //UpdateQuestProgress("Killing Monster");

        //foreach (RectTransform trans in questUIContent.GetComponentsInChildren<RectTransform>())
        //{
        //    if(trans.GetComponent<QuestUI>() != null)
        //    {
        //        int childcount = questUIContent.transform.GetComponent<QuestUI>().childCount;
        //    }
        //}
    }

    //UPDATE WHENEVER PLAYER PERFORMS CERTAIN SPECIFIC ACTIONS
    public void UpdateQuestProgress(QuestType questtype)
    {
        //questText.text = "Completed";
        //FIND ALL THE MATCHING QUESTS WITH THE SAME NAME
        List<Quest> matchingQuests = quests.FindAll(q => q.hiddenVariables.questType == questtype);
        Debug.Log($"MATCHING QUESTS {matchingQuests.Count}");
        foreach (Quest quest in matchingQuests)
        {
            if (quest != null && !quest.hiddenVariables.isCompleted)
            {
                Debug.Log("QUEST UPDATED");
                quest.UpdateProgress();
                CheckQuestCompletion(quest);
            }
            int idx = quests.IndexOf(quest);
            QuestUI questUIComponent = questUIContent.transform.GetChild(idx).GetComponent<QuestUI>();
            if (questUIComponent != null
                //&& questUIComponent.questStr == quest.hiddenVariables.questName
                && quest.hiddenVariables.currentCount <= quest.hiddenVariables.requiredCount
                )
            {
                questUIComponent.UpdateUI(quest);
            }
        }
    }

    void CheckQuestCompletion(Quest quest)
    {

        //UPDATE QUEST UI
        if (quest.hiddenVariables.isCompleted)
        {
            Debug.Log($"Quest '{quest.hiddenVariables.questName}' completed!");
            //questText.text = "Completed";
            //menuManager.newQuestNot.SetActive(false);
            //menuManager.questCompletedNot.SetActive(true);

            newQuestNotification.enabled = false;
            questUpdatedNotification.enabled = true;

            AS.clip = questCompletedClip;
            AS.Play();
            // Provide rewards or trigger other events
        }
    }


    //public void CloseQuestPanel()
    //{
    //    Ques
    //}
}
