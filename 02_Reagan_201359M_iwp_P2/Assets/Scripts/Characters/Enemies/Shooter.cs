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
    float timer = 0;
    int pelletstoshoot;

    public GameObject Projectile;

    float projectilespeed = 10.0f;

    float stage1 = 1;
    float stage2 = 2;

    protected override void Awake()
    {
        base.Awake();

        meleedamage = 10;
        projectileDamage = 15;
    }



    protected override void Update()
    {
        base.Update();

        switch (currentState)
        {
            case EnemyState.ABOUT_TO_ATTACK:
                {
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
                    if (!animatorComponent.GetCurrentAnimatorStateInfo(0).IsName("attack"))
                    {
                        animatorComponent.SetFloat("AttackStage", stage1);
                    }
                    break;
                }
            case EnemyState.ATTACK:
                {
                    //ANIMATION
                    if (animatorComponent.GetFloat("AttackStage") == stage1)
                    {
                        animatorComponent.SetFloat("AttackStage", stage2);
                        animatorComponent.Play("attack", 0, 0f);
                    }
                    //
                    //Debug.Log("SHOOT");
                    ShootProjectile();
                    break;
                }
            //case EnemyState.HURT:
            //    {
            //        Debug.Log("OUCH SHOOTER");
            //        break;
            //    }
            default:
                {
                    //Debug.Log("DEFAULT");
                    //timer += 1 * Time.deltaTime;
                    //if (timer >= 5)
                    //{
                    //    currentState = EnemyState.ABOUT_TO_ATTACK;
                    //    timer = 0;
                    //}

                    break;
                }
        }
    }
    void ShootProjectile()
    {
        // Instantiate the projectile
        GameObject projectile = Instantiate(Projectile, transform.position, Quaternion.identity);

        if (projectile != null)
        {
            Projectile projectilescript = projectile.GetComponent<Projectile>();
            projectilescript.projectiletype = ProjectileType.NORMAL;
            projectilescript.setdata(projectileDamage, projectilespeed,
                (player.transform.position - transform.position).normalized, gameObject);
        }
        animatorComponent.SetFloat("AttackStage", 0);
        //spriteRenderer.color = Color.blue;
        currentState = EnemyState.IDLE;
    }

}
