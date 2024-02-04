using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialManager", menuName = "Tutorial Manager")]
public class TutorialSystem : ScriptableObject
{
    public bool StoryBeginningCompleted;
    public bool MovementTutorialCompleted;
    public bool AttackTutorialCompleted;
    public bool GemTutorialCompleted;
    public bool SwitchCharactersCompleted;
    public bool ShopTutorialCompleted;
    public bool DayTrackerCompleted;
    public bool ATMCompleted;
    public bool CaveCompleted;


    private const string PlayerPrefsKey = "TutorialSystemData";

    public void SaveData()
    {
        PlayerPrefs.SetString(PlayerPrefsKey, JsonUtility.ToJson(this));
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsKey))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(PlayerPrefsKey), this);
        }
        else
        {
            ResetTutorials();
        }
    }

    // Optional: Reset all tutorial flags to their initial state
    public void ResetTutorials()
    {
        StoryBeginningCompleted = false;
        MovementTutorialCompleted = false;
        AttackTutorialCompleted = false;
        GemTutorialCompleted = false;
        SwitchCharactersCompleted = false;
        ShopTutorialCompleted = false;
        DayTrackerCompleted = false;
        ATMCompleted = false;
        CaveCompleted = false;

        SaveData();
    }
}
