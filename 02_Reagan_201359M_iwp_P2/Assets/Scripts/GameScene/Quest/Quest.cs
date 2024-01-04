using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class Quest
{

    [Serializable]
    public class HiddenVariables
    {
        public string questName;
        public string description;
        public bool isCompleted;
        public int requiredCount;
        public int currentCount;
        public int questGiverID;
    }


    //[HideInInspector]
    public HiddenVariables hiddenVariables;

    //public void Initialize(int requiredNumber)
    //{
    //    requiredCount = requiredNumber;
    //    currentCount = 0;
    //    isCompleted = false;
    //    //Quest newQuest = new Quest();
    //}

    //UPDATE THE PROGRESS OF THE INDIVIDUAL QUEST
    public void UpdateProgress()
    {
        hiddenVariables.currentCount++;

        if (hiddenVariables.currentCount >= hiddenVariables.requiredCount)
        {
            hiddenVariables.isCompleted = true;
        }
    }
}

