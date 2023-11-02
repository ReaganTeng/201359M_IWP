using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


//ART CAME FROM: https://opengameart.org/content/goblin-free-pixelart, https://opengameart.org/content/gothicvania-cemetery-pack

public class Chaser : Enemy
{
    public enum ChaserAttackPattern
    {
        SWIPE,
        POUNCE
    }

    public ChaserAttackPattern currentAttackPattern;



    protected override void Update()
    {
        base.Update();

        // Implement state-specific behavior in derived classes
        switch (currentState)
        {
            case EnemyState.ABOUT_TO_ATTACK:
                FollowPlayer();
                if(distance <= 1.0f)
                {
                    currentState = EnemyState.ATTACK;
                }
                break;
            case EnemyState.ATTACK:
                switch (currentAttackPattern)
                {
                    case ChaserAttackPattern.SWIPE:
                    case ChaserAttackPattern.POUNCE:
                        //currentState = EnemyState.CHASE;
                        break;
                }
                break;
            case EnemyState.HURT:
                break;
        }
    }
}
