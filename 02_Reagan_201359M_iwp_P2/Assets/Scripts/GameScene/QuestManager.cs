// QuestManager.cs
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;


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

    public GameObject toggleQuestButton;

    UIElementAnimations UIAnims;

    void Awake()
    {
        UIAnims = GetComponent<UIElementAnimations>();
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
        //Debug.Log("NOTIFICATIONS DISABLED");
        
        UIAnims.StopScaleAnimation(toggleQuestButton);
        toggleQuestButton.transform.localScale = Vector3.one;

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
        //Debug.Log("QUEST ADDED");
        quests.Add(newQuest);
        
        GameObject itemObject = Instantiate(questPrefab, questUIContent.transform);
        itemObject.GetComponent<QuestUI>().UpdateUI(newQuest);
        itemObject.GetComponent<QuestUI>().giverID = QuestGiverId;

        //questUIContent.GetComponent<RectTransform>().sizeDelta =
        //    new Vector2(questUIContent.GetComponent<RectTransform>().sizeDelta.x
        //     , questUIContent.GetComponent<RectTransform>().sizeDelta.y + itemObject.GetComponent<RectTransform>().sizeDelta.y);
        //itemObject.GetComponent<RectTransform>().sizeDelta
        //    = new Vector2(questUIContent.GetComponent<RectTransform>().sizeDelta.x
        //     , itemObject.GetComponent<RectTransform>().sizeDelta.y);

        //foreach(RectTransform trans )
        //updateContentSize();
        newQuestNotification.enabled = true;
        questUpdatedNotification.enabled = false;

        UIAnims.SetScaleAnimation(1.5f, .5f, toggleQuestButton);


        AS.clip = newQuestClip;
        //Debug.Log($"CLIP SWITCHED TO {AS.clip}");
        AS.Play();
    }


    //public void updateContentSize()
    //{
    //    int childCount = 0;
    //    //GameObject itemObject = Instantiate(questPrefab);

    //    foreach (RectTransform trans in questUIContent.GetComponentsInChildren<RectTransform>())
    //    {
    //        QuestUI questUIComponent = trans.GetComponent<QuestUI>();

    //        if (questUIComponent != null)
    //        {
    //            childCount++;
    //            Debug.Log($"FOUND CHILD {childCount}");
    //        }
    //    }

    //    questUIContent.GetComponent<RectTransform>().sizeDelta =
    //       new Vector2(questUIContent.GetComponent<RectTransform>().sizeDelta.x
    //        , questPrefab.GetComponent<RectTransform>().sizeDelta.y * childCount);

    //    //Destroy(itemObject);

    //}


    private void Update()
    {
        
        

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
                //Debug.Log("QUEST UPDATED");
                quest.UpdateProgress();
                CheckQuestCompletion(quest);
            }
            int idx = quests.IndexOf(quest);
            QuestUI questUIComponent = questUIContent.transform.GetChild(idx).GetComponent<QuestUI>();
            if (questUIComponent != null
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
            
            newQuestNotification.enabled = false;
            questUpdatedNotification.enabled = true;
            UIAnims.SetScaleAnimation(1.5f, .5f, toggleQuestButton);

            AS.clip = questCompletedClip;
            AS.Play();
        }
    }


    //public void CloseQuestPanel()
    //{
    //    Ques
    //}
}
