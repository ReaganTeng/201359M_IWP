// Example of a SaveLoadManager scriptable object
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SaveLoadManager", menuName = "Save Load Manager")]
public class SaveLoadManager : ScriptableObject
{
    public void SaveUnlockedCharacters(List<Character> unlockedCharacters)
    {
        // Save unlocked characters to player prefs or another save system
    }

    public List<Character> LoadUnlockedCharacters()
    {
        // Load unlocked characters from player prefs or another save system
        return new List<Character>();
    }
}

