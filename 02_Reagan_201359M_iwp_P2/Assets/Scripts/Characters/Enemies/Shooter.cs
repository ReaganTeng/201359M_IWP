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
        base.Update();

        if(disabled)
        {
            return;
        }

        switch (currentState)
        {
            case EnemyState.ABOUT_TO_ATTACK:
                {
                    string aboutToAttackName = characterAnimations.Find(
                   template => template.characterType == characterType
                   ).animationClips[1].name;
                    //0 _ IDLE
                    //1 _ about to atta
                    //2 _ attack
                    //3 _ run
                    //4 _ death
                    //5 - hurt

                    //Debug.Log("SHOOTING");
                    shootTimer += 1 * Time.deltaTime;
                    direction = LastKnownPosition - transform.position;
                    //transform.position -= direction * Time.deltaTime;

                    if (shootTimer >= 1.0)
                    {
                        currentState = EnemyState.ATTACK;
                        shootTimer = 0;
                    }

                    //ANIMATION
                    if (!animatorComponent.GetCurrentAnimatorStateInfo(0).IsName(aboutToAttackName))
                    {
                        //animatorComponent.SetFloat("AttackStage", stage1);
                        currentAnimIdx = 1;
                    }
                    break;
                }
            case EnemyState.ATTACK:
                {
                    string aboutToAttackName = characterAnimations.Find(
                   template => template.characterType == characterType
                   ).animationClips[1].name;

                    //ANIMATION
                    if (animatorComponent.GetCurrentAnimatorStateInfo(0).IsName(aboutToAttackName))
                    {
                        //animatorComponent.SetFloat("AttackStage", stage2);
                        currentAnimIdx = 2;
                        animatorComponent.Play("clip", 0, 0f);
                    }
                    //
                    //Debug.Log("SHOOT");
                    ShootProjectiles(
                        ProjectileType.NORMAL,
                        player.transform.position,
                        transform.position//,
                        //Quaternion.Euler(0, 0, 0)
                        );
                    currentAnimIdx = 0;
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
