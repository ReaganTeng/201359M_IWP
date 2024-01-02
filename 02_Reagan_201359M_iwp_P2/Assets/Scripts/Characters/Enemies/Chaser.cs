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

    protected override void Awake()
    {
        base.Awake();
        //characterType = CharacterUnlockManager.CharacterType.CHASER;
        //characterAnimations =characterAnimations;
    }


    float timertochase = 0;


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

        // Implement state-specific behavior in derived classes
        StateManager();

    }


    public override void StateManager()
    {
        base.StateManager();


        switch (currentState)
        {
            case EnemyState.ABOUT_TO_ATTACK:
                {
                    timertochase += 1 * Time.deltaTime;

                    if (timertochase >= 3)
                    {
                        currentState = EnemyState.IDLE;
                        timertochase = 0;
                    }

                    //ANIMATION
                    if (!animatorComponent.GetCurrentAnimatorStateInfo(0).IsName(ABOUT_TO_ATTACK))

                    {
                        if (distance <= 1.0f)
                        {
                            //animatorComponent.SetFloat("AttackStage", stage1);
                            currentAnimState = ABOUT_TO_ATTACK;
                        }
                        else
                        {
                            FollowPlayer();
                        }
                    }

                    if (animatorComponent.GetCurrentAnimatorStateInfo(0).IsName(ABOUT_TO_ATTACK)
                        //&& animatorComponent.GetFloat("AttackStage") == stage1
                        && animatorComponent.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                    {
                        currentState = EnemyState.ATTACK;
                    }

                    break;
                }
            case EnemyState.ATTACK:
                {

                    //  string AttackName = characterAnimations.Find(
                    //template => template.characterType == characterType
                    //).animationClips[2].name;

                    //if (animatorComponent.GetFloat("AttackStage") == stage1)
                    if (animatorComponent.GetCurrentAnimatorStateInfo(0).IsName(ABOUT_TO_ATTACK))
                    {
                        //animatorComponent.SetFloat("AttackStage", stage2);
                        //animatorComponent.Play(ATTACK, 0, 0f);
                        currentAnimState = ATTACK;
                    }
                    //else
                    //{
                    //    currentAnimState ;
                    //}

                    switch (currentAttackPattern)
                    {
                        case ChaserAttackPattern.SWIPE:
                        case ChaserAttackPattern.POUNCE:
                            //animatorComponent.SetFloat("AttackStage", stage2);
                            currentAnimState = ATTACK;
                            if (animatorComponent.GetCurrentAnimatorStateInfo(0).IsName(ATTACK)
                           // animatorComponent.GetCurrentAnimatorStateInfo(0).IsName("attack")
                           //&& animatorComponent.GetFloat("AttackStage") == stage2
                           && animatorComponent.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f
                           && attackcooldown <= 0.0)
                            {
                                //spriteRenderer.color = Color.white;
                                //animatorComponent.SetFloat("AttackStage", 0);
                                attackcooldown = .5f;

                                Player playerscript = player.GetComponent<Player>();
                                Character playercharacterscript = player.GetComponent<Character>();
                                if (distance <= 2.0f)
                                {
                                    //IF PLAYER HAS SHIELD
                                    if (playercharacterscript.playerShield != null
                                    && playercharacterscript.playerShield.shieldActive)
                                    {
                                        for (int i = 0; i < playercharacterscript.activeEffects.Count; i++)
                                        {
                                            if (playercharacterscript.activeEffects[i].Type == EffectType.SHIELD)
                                            {
                                                //collisionCharacter.activeEffects[i].
                                                playercharacterscript.activeEffects.Remove(playercharacterscript.activeEffects[i]);
                                            }
                                        }

                                        playercharacterscript.playerShield.shieldActive = false;
                                        //collisionCharacter.activeEffects
                                    }
                                    //IF PLAYER HAS NO SHIELD
                                    else
                                    {
                                        if (playercharacterscript.audioSource != null)
                                        {
                                            playercharacterscript.audioSource.clip = playercharacterscript.audioclips[0];
                                            playercharacterscript.audioSource.Play();
                                        }
                                        playercharacterscript.health -= meleedamage;

                                    }
                                }

                                currentAnimState = IDLE;
                                currentState = EnemyState.IDLE;
                            }
                            break;
                    }
                    break;
                }
                //case EnemyState.HURT:
                //    break;
        }
    }
}
