using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tanker : Enemy
{
    public enum TankerAttackPattern
    {
        STRAIGHT_CHARGE,
        CLEAVE,
        SHIELD_BASH
    }

    public TankerAttackPattern currentAttackPattern;

    protected override void Update()
    {
        base.Update();

        if (currentState == EnemyState.ATTACK)
        {
            // Implement tanker-specific attack pattern behavior
            switch (currentAttackPattern)
            {
                case TankerAttackPattern.STRAIGHT_CHARGE:
                    // Implement straight charge attack
                    break;
                case TankerAttackPattern.CLEAVE:
                    // Implement cleave attack
                    break;
                case TankerAttackPattern.SHIELD_BASH:
                    // Implement shield bash attack
                    break;
            }
        }
    }
}
