using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum QuestType
{
    MONSTER_SLAYING,
    //MINOTAUR_SLAYING,
    //GOBLIN_SLAYING,
    //DEMON_SLAYING,
}


[Serializable]
public class Quest
{

    [Serializable]
    public class HiddenVariables
    {
        public QuestType questType;
        public string questName;
        public string description;
        public bool isCompleted;
        public int requiredCount;
        public int currentCount;
        //public int prevcount;
        public int questGiverID;
    }


    //[HideInInspector]
    public HiddenVariables hiddenVariables;

    

    //UPDATE THE PROGRESS OF THE INDIVIDUAL QUEST
    public void UpdateProgress()
    {
        
        hiddenVariables.currentCount += 1;
        

        //hiddenVariables.prevcount = hiddenVariables.currentCount;

        if (hiddenVariables.currentCount >= hiddenVariables.requiredCount)
        {
            hiddenVariables.isCompleted = true;
        }
    }
}

