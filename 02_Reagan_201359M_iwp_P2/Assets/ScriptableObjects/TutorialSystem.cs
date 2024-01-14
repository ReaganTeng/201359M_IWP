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
}
