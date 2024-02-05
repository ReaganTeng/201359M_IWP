using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Debug = UnityEngine.Debug;
using static TutorialTrigger;




[Serializable]
public class TutorialDialogues
{
    public TutorialType tutType;
    public DialogueRealTime dialogue;
}

public class TutorialTrigger : MonoBehaviour
{
    public TutorialSystem tutorialSystem;

    public List<TutorialDialogues> tutorialDialogues = new List<TutorialDialogues>();

    public enum TutorialType
    {
        STORY_BEGINNING,
        MOVEMENT,
        CHARACTER_SWITCH,
        ATTACK,
        GEM,
        ATM,
        DAY_TRACKER,
        SHOP_SYSTEM,
    }

    [HideInInspector]
    public DialogueManager DM;

    string GameScene;
    string HubWorldScene;
    Scene currentScene;

    public void Awake()
    {
        tutorialSystem.LoadData();

        GameScene = "GameScene";
        HubWorldScene = "HubWorld";
        DM = GameObject.FindGameObjectWithTag("GameMGT").GetComponent<DialogueManager>();
    }

    void Update()

    {
        currentScene = SceneManager.GetActiveScene();
        TriggerDialogue(TutorialType.STORY_BEGINNING, ref tutorialSystem.StoryBeginningCompleted, HubWorldScene);
        TriggerDialogue(TutorialType.MOVEMENT, ref tutorialSystem.MovementTutorialCompleted, GameScene);



        if (GameObject.FindGameObjectsWithTag("Player").Length > 1 && tutorialSystem.MovementTutorialCompleted)
        {
            TriggerDialogue(TutorialType.CHARACTER_SWITCH, ref tutorialSystem.SwitchCharactersCompleted, GameScene);
        }
    }

    void TriggerDialogue(TutorialType tutType, ref bool boolToSet, string requiredScene)
    {
        if (DM.currentDialogue != null)
        {
            return;
        }


        DialogueRealTime d = tutorialDialogues.Find(entry => entry.tutType == tutType).dialogue;
        if (!boolToSet 
            && currentScene.name == requiredScene
            && DM.dialoguePanel != null)
        {
            DM.StartDialogue(d);
            boolToSet = true;
        }

        tutorialSystem.SaveData();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Item":
                TriggerDialogue(TutorialType.GEM, ref tutorialSystem.GemTutorialCompleted, GameScene);
                break;
            case "Enemy":
                TriggerDialogue(TutorialType.ATTACK, ref tutorialSystem.AttackTutorialCompleted, GameScene);
                break;
            case "DayTracker":
                TriggerDialogue(TutorialType.DAY_TRACKER, ref tutorialSystem.DayTrackerCompleted, HubWorldScene);
                break;
            case "ATM":
                TriggerDialogue(TutorialType.ATM, ref tutorialSystem.ATMCompleted, HubWorldScene);
                break;
            case "Shopkeeper":
                TriggerDialogue(TutorialType.SHOP_SYSTEM, ref tutorialSystem.ShopTutorialCompleted, HubWorldScene);
                break;
            default:
                break;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        // Handle OnTriggerExit2D if needed
    }
}
