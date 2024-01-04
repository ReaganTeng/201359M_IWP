using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    public TextMeshProUGUI questNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI progressText;


    public string questStr;

    public void UpdateUI(Quest quest)
    {
        if(questStr == "")
        {
            questStr = quest.hiddenVariables.questName;
        }
        questNameText.text = "Quest: " + quest.hiddenVariables.questName;
        descriptionText.text = "Description: " + quest.hiddenVariables.description;
        progressText.text = "Progress: " + quest.hiddenVariables.currentCount + " / " + quest.hiddenVariables.requiredCount;
        Debug.Log("QUEST UI UPDATED");
    }
}
