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

    private void Start()
    {

    }

    float stage1 = 1;
    float stage2 = 2;
    float timertochase = 0;


    protected override void Update()
    {
        base.Update();


        // Implement state-specific behavior in derived classes
        switch (currentState)
        {
            case EnemyState.ABOUT_TO_ATTACK:
                {

                    timertochase += 1 * Time.deltaTime;

                    if(timertochase >= 3)
                    {
                        currentState = EnemyState.IDLE;
                        timertochase = 0;
                    }

                    if (distance <= 1.0f)
                    {
                        animator.SetFloat("AttackStage", stage1);
                        if (animator.GetCurrentAnimatorStateInfo(0).IsName("attack")
                            && animator.GetFloat("AttackStage") == stage1
                            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {       
                            currentState = EnemyState.ATTACK;
                        }
                    }
                    else
                    {
                        FollowPlayer();
                    }
                    break;
                }
            case EnemyState.ATTACK:
                {
                    if (animator.GetFloat("AttackStage") == stage1)
                    {
                        animator.SetFloat("AttackStage", stage2);
                        animator.Play("attack", 0, 0f);
                    }

                    switch (currentAttackPattern)
                    {
                        case ChaserAttackPattern.SWIPE:
                        case ChaserAttackPattern.POUNCE:
                            animator.SetFloat("AttackStage", stage2);

                            if (animator.GetCurrentAnimatorStateInfo(0).IsName("attack")
                             && animator.GetFloat("AttackStage") == stage2
                           && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f
                           && attackcooldown <= 0.0)
                            {
                                spriteRenderer.color = Color.white;
                                animator.SetFloat("AttackStage", 0);
                                attackcooldown = .5f;

                                if (distance <= 2.0f)
                                {
                                    player.GetComponent<Player>().health -= damage;
                                }

                                currentState = EnemyState.IDLE;
                            }
                            break;
                    }
                    break;
                }
            case EnemyState.HURT:
                break;
        }
    }
}
