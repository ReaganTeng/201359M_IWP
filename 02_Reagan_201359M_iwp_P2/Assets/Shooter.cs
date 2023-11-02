using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : Enemy
{
    public enum ShooterAttackPattern
    {
        SHOOT,
        GRENADE
    }

    public ShooterAttackPattern currentAttackPattern;

    protected override void Update()
    {
        base.Update();

        if (currentState == EnemyState.ATTACK)
        {
            // Implement shooter-specific attack pattern behavior
            switch (currentAttackPattern)
            {
                case ShooterAttackPattern.SHOOT:
                    // Implement shooting attack
                    break;
                case ShooterAttackPattern.GRENADE:
                    // Implement grenade attack
                    break;
            }
        }
    }
}
