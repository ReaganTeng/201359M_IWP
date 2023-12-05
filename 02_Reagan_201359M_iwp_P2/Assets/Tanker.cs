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

    public void Start()
    {
        attackcollider = GetComponent<BoxCollider2D>();
        circlecollider = GetComponent<CircleCollider2D>();

        circlecollider.enabled = true;
        attackcollider.enabled = false;

        damage = 10;
    }

    protected override void Update()
    {
        base.Update();
        // Enemy is too close to the player, move away from the player.


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
                        currentState = EnemyState.IDLE;
                        chargingtime = 0;
                    }

                    break;
                }
            //case EnemyState.HURT:
               
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
            Debug.Log("BANG Player");
            //DAMAGE PLAYER
            Player player = collision.gameObject.GetComponent<Player>();


            if (player.playerShield != null
                && player.playerShield.shieldActive)
            {
                player.playerShield.shieldtimer = 0;
            }
            else
            {
                player.health -= damage;
            }


            //collision.gameObject.GetComponent<Player>().immunity_timer = .5f;
            attackcooldown = .5f;
            //Destroy(gameObject);
            //collision.gameObject.GetComponent<Player>().healthbar.value = collision.gameObject.GetComponent<Player>().health;
            //collision.gameObject.GetComponent<Player>().healthbar.value = collision.gameObject.GetComponent<Player>().health;
   

            




            Debug.Log("PLAYER HEALTH " + collision.gameObject.GetComponent<Player>().health);
        }
    }
}