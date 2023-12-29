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

        //characterAnimations =characterAnimations;
    }

    float stage1 = 1;
    float stage2 = 2;
    float timertochase = 0;


    protected override void Update()
    {
        base.Update();


        PlayAnimation(characterType, currentAnimIdx);
      

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

                    string aboutToAttackName = characterAnimations.Find(
                       template => template.characterType == characterType
                       ).animationClips[1].name;
                        //0 _ IDLE
                        //1 _ about to atta
                        //2 _ attack
                        //3 _ run
                        //4 _ death
                        //5 - hurt


                    //ANIMATION
                    if (!animatorComponent.GetCurrentAnimatorStateInfo(0).IsName(aboutToAttackName))

                    {
                        if (distance <= 1.0f * 1.5f)
                        {
                            //animatorComponent.SetFloat("AttackStage", stage1);
                            currentAnimIdx = 1;
                        }
                        else
                        {

                            FollowPlayer();
                        }
                    }


                    string AttackName = characterAnimations.Find(
                   template => template.characterType == characterType
                   ).animationClips[2].name;
                    //0 _ IDLE
                    //1 _ about to atta
                    //2 _ attack
                    //3 _ run
                    //4 _ death
                    //5 - hurt

                    if (animatorComponent.GetCurrentAnimatorStateInfo(0).IsName(AttackName)
                        //&& animatorComponent.GetFloat("AttackStage") == stage1
                        && animatorComponent.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                    {
                        currentState = EnemyState.ATTACK;
                    }

                    break;
                }
            case EnemyState.ATTACK:
                {

                    string AttackName = characterAnimations.Find(
                  template => template.characterType == characterType
                  ).animationClips[2].name;

                    //if (animatorComponent.GetFloat("AttackStage") == stage1)
                    if (animatorComponent.GetCurrentAnimatorStateInfo(0).IsName(AttackName))
                    {
                            //animatorComponent.SetFloat("AttackStage", stage2);
                        animatorComponent.Play("clip", 0, 0f);
                    }

                    switch (currentAttackPattern)
                    {
                        case ChaserAttackPattern.SWIPE:
                        case ChaserAttackPattern.POUNCE:
                            //animatorComponent.SetFloat("AttackStage", stage2);

                            if (
                            animatorComponent.GetCurrentAnimatorStateInfo(0).IsName(AttackName)
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

                                currentAnimIdx = 0;
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
