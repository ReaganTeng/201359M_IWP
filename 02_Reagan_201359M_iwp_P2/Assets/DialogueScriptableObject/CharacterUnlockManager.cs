using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterUnlockManager", menuName = "Character Unlock Manager")]
public class CharacterUnlockManager : ScriptableObject
{
    public List<Character> unlockedCharacters = new List<Character>();

    public void UnlockCharacter(Character character)
    {
        if (!unlockedCharacters.Contains(character))
        {
            unlockedCharacters.Add(character);
        }
    }
}
