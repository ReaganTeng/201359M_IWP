using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Upgrades;

[CreateAssetMenu(fileName = "CharacterUnlockManager", menuName = "Character Unlock Manager")]
public class CharacterUnlockManager : ScriptableObject
{
    [System.Serializable]
    public class CharacterListWrapper
    {
        public List<CharacterUnlockManager.CharacterType> characterList;
    }

    public enum CharacterType
    {
        // PLAYERS
        JOE,
        PROFESSOR,
        VETERAN,

        // ENEMIES,
        SHOOTER,
        CHASER,
        TANKER,
    }

    public List<CharacterType> unlockedCharacters = new List<CharacterType>();

    // THE CHARACTER YOU CHOSE TO COLLABORATE WITH YOU
    public List<CharacterType> selectedCharacters = new List<CharacterType>();

    // PlayerPrefs keys
    string UnlockedCharactersKey = "UnlockedCharacters";
    string SelectedCharactersKey = "SelectedCharacters";

    // Load data from PlayerPrefs
    // Load data from PlayerPrefs
    public void LoadData()
    {
        unlockedCharacters.Clear();
        selectedCharacters.Clear();
        


        // Load data from PlayerPrefs
        string unlockedCharactersJson = PlayerPrefs.GetString(UnlockedCharactersKey);
        string selectedCharactersJson = PlayerPrefs.GetString(SelectedCharactersKey);

        // Deserialize Lists from JSON
        CharacterListWrapper unlockedWrapper = JsonUtility.FromJson<CharacterListWrapper>(unlockedCharactersJson);
        CharacterListWrapper selectedWrapper = JsonUtility.FromJson<CharacterListWrapper>(selectedCharactersJson);

        // Update Lists
        unlockedCharacters = unlockedWrapper != null ? unlockedWrapper.characterList : new List<CharacterType>();
        selectedCharacters = selectedWrapper != null ? selectedWrapper.characterList : new List<CharacterType>();

        if (!selectedCharacters.Contains(CharacterType.JOE))
        {
            selectedCharacters.Add(CharacterType.JOE);
        }
        if (!unlockedCharacters.Contains(CharacterType.JOE))
        {
            unlockedCharacters.Add(CharacterType.JOE);
        }
    }



    // Save data to PlayerPrefs
    // Save data to PlayerPrefs
    public void SaveData()
    {
        // Create wrapper instances
        CharacterListWrapper unlockedWrapper = new CharacterListWrapper { characterList = unlockedCharacters };
        CharacterListWrapper selectedWrapper = new CharacterListWrapper { characterList = selectedCharacters };

        // Serialize Lists to JSON
        string unlockedCharactersJson = JsonUtility.ToJson(unlockedWrapper);
        string selectedCharactersJson = JsonUtility.ToJson(selectedWrapper);

        // Log serialized JSON data (for debugging)
        Debug.Log($"Serialized Unlocked Characters: {unlockedCharactersJson}");
        Debug.Log($"Serialized Selected Characters: {selectedCharactersJson}");

        // Save JSON data to PlayerPrefs
        PlayerPrefs.SetString(UnlockedCharactersKey, unlockedCharactersJson);
        PlayerPrefs.SetString(SelectedCharactersKey, selectedCharactersJson);

        // Save PlayerPrefs
        PlayerPrefs.Save();

        // Log confirmation (for debugging)
        Debug.Log($"SAVED Unlocked Characters: {PlayerPrefs.GetString(UnlockedCharactersKey)}" +
                  $"\nSaved Selected Characters: {PlayerPrefs.GetString(SelectedCharactersKey)}"
                 );
    }

    public void EmptyCharacterList()
    {
        unlockedCharacters.Clear();
        selectedCharacters.Clear();


        if (!selectedCharacters.Contains(CharacterType.JOE))
        {
            selectedCharacters.Add(CharacterType.JOE);
        }
        if (!unlockedCharacters.Contains(CharacterType.JOE))
        {
            unlockedCharacters.Add(CharacterType.JOE);
        }

        SaveData();
    }
    public void UnlockCharacter(CharacterType character)
    {
        if (!unlockedCharacters.Contains(character))
        {
            unlockedCharacters.Add(character);
        }

        if (!selectedCharacters.Contains(character))
        {
            selectedCharacters.Add(character);

        }

        SaveData();

    }

    //public void SelectCharacter(CharacterType character)
    //{
    //    if (!selectedCharacters.Contains(character))
    //    {
    //        selectedCharacters.Add(character);
    //        SaveData();
    //    }
    //}

}
