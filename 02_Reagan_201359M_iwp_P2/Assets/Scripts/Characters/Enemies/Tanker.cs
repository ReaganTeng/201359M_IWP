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

    float chargetimer = 0;
    float chargingtime = 0;

    Vector3 LastKnownPosition;
    Vector3 direction;
    //float timer = 0;

    BoxCollider2D attackcollider;
    CircleCollider2D circlecollider;


    protected override void Awake()
    {
        base.Awake();

        attackcollider = GetComponent<BoxCollider2D>();
        circlecollider = GetComponent<CircleCollider2D>();

        circlecollider.enabled = true;
        attackcollider.enabled = false;

        meleedamage = 10;
        projectileDamage = 10;

        //characterType = CharacterUnlockManager.CharacterType.tan;
    }

    protected override void Update()
    {
        base.Update();
        // Enemy is too close to the player, move away from the player.

        //PlayAnimation(characterType, currentAnimIdx);
        if (disabled)
        {
            return;
        }
        switch (currentState)
        {
            case EnemyState.ABOUT_TO_ATTACK:
                {
                    attackcollider.enabled = false;
                    circlecollider.enabled = true;
                    //Debug.Log("CHARGING");
                    chargetimer += 1 * Time.deltaTime;
                    //direction = LastKnownPosition - transform.position;
                    direction = LastKnownPosition - transform.position;

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
                        currentAnimIdx = 1;
                        //animatorComponent.SetFloat("AttackStage", stage1);
                    }

                    transform.position -= direction * Time.deltaTime;
                    if (chargetimer >= 1.0f)
                    {
                        currentState = EnemyState.ATTACK;
                        chargetimer = 0;
                    }
                    break;
                }
            case EnemyState.ATTACK:
                {
                    attackcollider.enabled = true;
                    circlecollider.enabled = false;

                    Debug.Log("CHARGE");
                    chargingtime += 1 * Time.deltaTime;

                    string AttackName = characterAnimations.Find(
                   template => template.characterType == characterType
                   ).animationClips[1].name;

                    //ANIMATION
                    //if (animatorComponent.GetFloat("AttackStage") == stage1)
                    if (!animatorComponent.GetCurrentAnimatorStateInfo(0).IsName(AttackName))
                    {
                        currentAnimIdx = 1;
                        //animatorComponent.SetFloat("AttackStage", stage2);
                        //animatorComponent.Play("attack", 0, 0f);
                    }
                    //

                    switch (currentAttackPattern)
                    {
                        case TankerAttackPattern.STRAIGHT_CHARGE:
                        case TankerAttackPattern.CLEAVE:
                        case TankerAttackPattern.SHIELD_BASH:
                            // Implement shield bash attack
                            direction = LastKnownPosition - transform.position;
                            transform.position += direction * 10 * Time.deltaTime;
                            spriteRenderer.color = Color.white;
                            break;
                    }

                    if (chargingtime >= 1.0f)
                    {
                        currentAnimIdx = 0;
                        //animatorComponent.SetFloat("AttackStage", 0);
                        currentState = EnemyState.IDLE;
                        chargingtime = 0;
                    }

                    break;
                }
            default:
                //Debug.Log("DEFAULT");
                //timer += 1 * Time.deltaTime;
                //if (timer >= 5)
                {
                    attackcollider.enabled = false;
                    circlecollider.enabled = true;

                    if (player != null
                        && !player.GetComponent<Player>().AIMode)
                    {
                        LastKnownPosition = player.transform.position;
                    }
                    //currentState = EnemyState.ABOUT_TO_ATTACK;
                    //timer = 0;
                    break;

                }

        }


    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")
            && attackcooldown <= 0.0f
            //&& collision.gameObject.GetComponent<Player>().immunity_timer <= 0.0f
            )
        {
            //Debug.Log("BANG Player");
            //DAMAGE PLAYER
            Player player = collision.gameObject.GetComponent<Player>();
            Character collisionCharacter = collision.gameObject.GetComponent<Character>();

            //SHIELD
            if (collisionCharacter.playerShield != null
                && collisionCharacter.playerShield.shieldActive)
            {
                for (int i = 0; i < collisionCharacter.activeEffects.Count; i++)
                {
                    if (collisionCharacter.activeEffects[i].Type == EffectType.SHIELD)
                    {
                        //collisionCharacter.activeEffects[i].
                        collisionCharacter.activeEffects.Remove(collisionCharacter.activeEffects[i]);
                    }
                }

                collisionCharacter.playerShield.shieldActive = false;
                //collisionCharacter.activeEffects
            }
            else
            {
                if (collisionCharacter.audioSource != null)
                {
                    collisionCharacter.audioSource.clip = collisionCharacter.audioclips[0];
                    collisionCharacter.audioSource.Play();
                }
                collisionCharacter.health -= meleedamage;
            }

            //collision.gameObject.GetComponent<Player>().immunity_timer = .5f;
            attackcooldown = .5f;
            //Destroy(gameObject);
            //collision.gameObject.GetComponent<Player>().healthbar.value = collision.gameObject.GetComponent<Player>().health;
            //collision.gameObject.GetComponent<Player>().healthbar.value = collision.gameObject.GetComponent<Player>().health;
            Debug.Log("PLAYER HEALTH " + player.health);
        }
    }
}
