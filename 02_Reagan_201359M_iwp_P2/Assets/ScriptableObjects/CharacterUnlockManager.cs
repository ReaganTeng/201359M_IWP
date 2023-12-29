using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterUnlockManager", menuName = "Character Unlock Manager")]
public class CharacterUnlockManager : ScriptableObject
{
    public enum CharacterType
    {
        //PLAYERS
        JOE,
        PROFESSOR,
        VETERAN,

        //ENEMIES,
        SHOOTER,
        CHASER,
        TANKER,
    }

    public List<CharacterType> unlockedCharacters = new List<CharacterType>();


    //THE CHARACTER YOU CHOSE TO COLLABORATE WITH YOU
    public List<CharacterType> selectedCharacters = new List<CharacterType>();


    public void UnlockCharacter(CharacterType character)
    {
        if (!unlockedCharacters.Contains(character))
        {
            unlockedCharacters.Add(character);
        }
    }

    public void SelectCharacter(CharacterType character)
    {
        if (!unlockedCharacters.Contains(character))
        {
            unlockedCharacters.Add(character);
        }
    }

}
