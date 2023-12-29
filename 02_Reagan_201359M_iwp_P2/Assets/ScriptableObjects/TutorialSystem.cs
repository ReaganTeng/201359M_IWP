using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialManager", menuName = "Tutorial Manager")]
public class TutorialSystem : ScriptableObject
{
    public bool MovementTutorialCompleted;
    public bool AttackTutorialCompleted;
    public bool GemTutorialCompleted;
}
