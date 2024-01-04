// QuestManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static ShopItem;
using System.Linq;
using static UnityEditor.Progress;
using UnityEditor.PackageManager.Requests;

public class QuestManager : MonoBehaviour
{

    [HideInInspector] public MenuManager menuManager;
    [HideInInspector] public GameObject questPanel;


    public GameObject questUIContent;
    public GameObject questPrefab;
    public List<Quest> quests = new List<Quest>();

    public TextMeshProUGUI questText;

    [HideInInspector]
    public AudioSource AS;
    public AudioClip newQuestClip, questCompletedClip;
    


    void Awake()
    {
        questText.text = "";
        //InitializeQuests();
        AS = GetComponent<AudioSource>();
        menuManager = GetComponent<MenuManager>();
        questPanel = menuManager.QuestPanel;
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
        //if(menuManager == null)
        //{
        //    menuManager = GetComponent<MenuManager>();
        //}
        //menuManager.ToggleQuestPanel();
        GameObject itemObject = Instantiate(questPrefab, questUIContent.transform);
        itemObject.GetComponent<QuestUI>().UpdateUI(newQuest);
        //menuManager.newQuestNot.SetActive(true);
        //menuManager.questCompletedNot.SetActive(false);
        //menuManager.ToggleQuestPanel();

        AS.clip = newQuestClip;
        AS.Play();
    }



    private void Update()
    {
        //UpdateQuestProgress("Killing Monster");
    }

    //UPDATE WHENEVER PLAYER PERFORMS CERTAIN SPECIFIC ACTIONS
    public void UpdateQuestProgress(string questName)
    {
        //questText.text = "Completed";
        List<Quest> matchingQuests = quests.FindAll(q => q.hiddenVariables.questName == questName);

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
