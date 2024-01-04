using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Projectile;

public class Shooter : Enemy
{
    public enum ShooterAttackPattern
    {
        SHOOT,
        GRENADE
    }

    public ShooterAttackPattern currentAttackPattern;

    float shootTimer = 0;
    Vector3 LastKnownPosition;
    Vector3 direction;
    //float timer = 0;
    //int pelletstoshoot;

    //public GameObject Projectile;

    float projectilespeed = 10.0f;

   
    protected override void Awake()
    {
        base.Awake();

        meleedamage = 10;
        projectileDamage = 15;

        //characterType = CharacterUnlockManager.CharacterType.SHOOTER;
    }



    protected override void Update()
    {
        PlayAnimation(currentAnimState);

        if (disabled)
        {
            return;
        }

        base.Update();

        if (health <= 0)
        {
            //DEATH
            Die();
            return;
        }

        StateManager();
    }




    public override void StateManager()
    {
        base.StateManager();

        switch (currentState)
        {
            case EnemyState.ABOUT_TO_ATTACK:
                {
                    //0 _ IDLE
                    //1 _ about to atta
                    //2 _ attack
                    //3 _ run
                    //4 _ death
                    //5 - hurt

                    //Debug.Log("SHOOTING");
                    shootTimer += 1 * Time.deltaTime * speed;
                    direction = LastKnownPosition - transform.position;
                    //transform.position -= direction * Time.deltaTime;

                    if (shootTimer >= 1.0)
                    {
                        currentState = EnemyState.ATTACK;
                        shootTimer = 0;
                    }

                    //ANIMATION
                    if (!animatorComponent.GetCurrentAnimatorStateInfo(0).IsName(ABOUT_TO_ATTACK))
                    {
                        currentAnimState = ABOUT_TO_ATTACK;
                    }
                    break;
                }
            case EnemyState.ATTACK:
                {
                    //ANIMATION
                    if (!animatorComponent.GetCurrentAnimatorStateInfo(0).IsName(ATTACK))
                    {
                        currentAnimState = ATTACK;
                        animatorComponent.Play(currentAnimState, 0, 0f);
                        Debug.Log("SHOOT");
                        ShootProjectiles(
                            ProjectileType.NORMAL,
                            player.transform.position,
                            transform.position);
                    }
                    //
                    currentAnimState = IDLE;
                    //spriteRenderer.color = Color.blue;
                    currentState = EnemyState.IDLE;

                    break;
                }
            default:
                {
                    break;
                }
        }
    }

}
