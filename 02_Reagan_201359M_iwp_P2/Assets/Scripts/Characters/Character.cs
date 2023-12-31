using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditor.EditorTools;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static CharacterUnlockManager;
using static Item;
using static Projectile;
using static UnityEditor.Experimental.GraphView.GraphView;


[System.Serializable]
public class CharacterAnimationClips
{
    public CharacterUnlockManager.CharacterType characterType;
    public List<AnimationClip> animationClips;
}

public class Character : MonoBehaviour
{
    public List<AudioClip> audioclips;
    public List<CharacterAnimationClips> characterAnimations = new List<CharacterAnimationClips>();
    public AnimatorController animController;


    public GameObject projectilePrefab;


    [HideInInspector]
    public int currentAnimIdx;
    [HideInInspector]
    public Animator animatorComponent;
    [HideInInspector]
    public int health;
    [HideInInspector]
    public int meleedamage;
    [HideInInspector]
    public SpriteRenderer spriteRenderer;
    [HideInInspector]
    public Shield playerShield;
    [HideInInspector]
    public List<Effect> activeEffects = new List<Effect>();
    [HideInInspector]
    public ParticleSystem ps;
    [HideInInspector]
    ParticleSystem.EmissionModule emissionModule;
    [HideInInspector]
    public AudioSource audioSource;
    [HideInInspector]
    public bool disabled;
    [HideInInspector]
    public int projectileDamage;

    float speed;

    public CharacterUnlockManager.CharacterType characterType;

    [HideInInspector]
   public  GameObject powerupNotificationPanel;
    CanvasGroup powerUpCanvasGroup
           ;
    //bool effectapplied = false;
    protected virtual void Awake()
    {
        currentAnimIdx = 0;
        disabled = false;
        //animatorComponent = GetComponent<Animator>();
        powerupNotificationPanel = GameObject.FindGameObjectWithTag("PowerUpNotification");
        powerUpCanvasGroup = powerupNotificationPanel.GetComponent<CanvasGroup>();
        powerUpCanvasGroup.alpha = 0;
        powerUpCanvasGroup.interactable = false;


        animatorComponent = GetComponent<Animator>();
        //animatorComponent.runtimeAnimatorController = animController;
        // Instantiate a new instance of the AnimatorController (RuntimeAnimatorController)
        AnimatorController newController = Instantiate(animController);
        if (newController != null)
        {
            // Assign the new controller to the animatorComponent
            animatorComponent.runtimeAnimatorController = newController;
        }
        

        audioSource = GetComponent<AudioSource>();
        speed = .1f;
        playerShield = GetComponentInChildren<Shield>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ps = GetComponent<ParticleSystem>();
        //ps.emission.enabled = true;
        if (ps != null)
        {
            emissionModule = ps.emission;
            emissionModule.enabled = false;
        }
    }

    //[ContextMenu("Change State Motion")]
    public void PlayAnimation(CharacterUnlockManager.CharacterType characterType, int clipIDX)
    {
        //GET THE CLIP YOU WANT TO PLAY
        CharacterAnimationClips characterAnimationEntry =
        characterAnimations.Find(entry => entry.characterType == characterType);
        AnimationClip clipToPlay = characterAnimationEntry.animationClips[clipIDX];
        //

        if (characterAnimationEntry != null && animatorComponent != null)
        {
            // Check if animatorComponent has a valid runtimeAnimatorController
            //if (animatorComponent.runtimeAnimatorController == null)
            //{
            //    Debug.LogError("AnimatorController is missing.");
            //    return;
            //}

            // Get the Animator Controller
            AnimatorController animatorCon = animatorComponent.runtimeAnimatorController as AnimatorController;
            if (animatorCon != null)
            {
                // Get the existing state by name
                AnimatorStateMachine stateMachine = animatorCon.layers[0].stateMachine;
                AnimatorState state = null;

                foreach (ChildAnimatorState childState in stateMachine.states)
                {
                    if (childState.state.name == "clip") // Replace with your actual state name
                    {
                        state = childState.state;
                        // Assign the animation clip to the state's motion
                        state.motion = clipToPlay;
                        // Play the animation by setting the state
                        Debug.Log($"STATE {state.name}");
                        animatorComponent.Play(state.name); // Replace with your actual state name
                        break;
                    }
                }
            }

        }
    }


    protected virtual Vector3 FindPath(
        Vector2 targetPos, //THE TARGET THE OBJECT WILL FOLLOW 
        Vector2 followerPos //THE OBJECT WHO WILL FOLLOW THE TARGET
        )
    {
       //RAYCAST ALL DIRECTIONS TO DETECT ANY WALLS
        Vector2 directionToPlayer = targetPos - followerPos;
        directionToPlayer.Normalize();
        float raycastDistance = 10.0f;
        //DRAW RAYS
        for (float angle = 0; angle < 360; angle += 1)
        {
            //float x = 1.0f;
            Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
            direction.Normalize();
            //Debug.DrawRay(enemyPosition, direction * raycastDistance * x, Color.red);
            //Debug.DrawRay(playerPosition, direction * raycastDistance * x, Color.magenta);
        }
        //Debug.DrawRay(enemyPosition, directionToPlayer * raycastDistance, Color.blue);
        //
        RaycastHit2D hitwall = Physics2D.Raycast(followerPos, directionToPlayer, raycastDistance, LayerMask.GetMask("WallTilemap"));
        //

        if (hitwall.collider != null)
        {
            // Scenario 1: There is an obstacle between the enemy and the player
            // Initialize variables for finding the closest intersection
            List<Vector3> lineOfSightPoints = new List<Vector3>();
            float minDistance = float.MaxValue;
            Vector3 closestIntersection = Vector3.zero;

            for (float angle = 0; angle < 360; angle += 1)
            {
                Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
                Vector2 endPoint = followerPos + direction * raycastDistance;

                List<Vector3> pointsInDirection = BresenhamLine(followerPos, endPoint);
                lineOfSightPoints.AddRange(pointsInDirection);
            }

            foreach (var point in lineOfSightPoints)
            {
                bool enemyLinecastClear = !Physics2D.Linecast(followerPos, point, LayerMask.GetMask("WallTilemap"));
                bool playerLinecastClear = !Physics2D.Linecast(targetPos, point, LayerMask.GetMask("WallTilemap"));

                // Filter out points that are too close to the enemy or player
                float minDistanceFromEntities = 0.1f; // Adjust this value as needed
                if (enemyLinecastClear && playerLinecastClear &&
                    Vector3.Distance(point, followerPos) > minDistanceFromEntities &&
                    Vector3.Distance(point, targetPos) > minDistanceFromEntities)
                {
                    float distance = Vector3.Distance(targetPos, point);
                    if (distance < minDistance)
                    {
                        closestIntersection = point;
                        minDistance = distance;
                    }
                }
            }
            lineOfSightPoints.Clear();
            return closestIntersection;
        }
        else
        {
            //Debug.Log("THERE'S NO OBSTACLE");
            //Debug.Log("PLAYER'S POSITION " + playerPosition);
            // Scenario 2: There is no obstacle between the enemy and the player
            return targetPos;
        }
    }


    protected virtual List<Vector3> BresenhamLine(Vector2 start, Vector2 end)
    {
        List<Vector3> linePoints = new List<Vector3>();

        int x0 = Mathf.RoundToInt(start.x);
        int y0 = Mathf.RoundToInt(start.y);
        int x1 = Mathf.RoundToInt(end.x);
        int y1 = Mathf.RoundToInt(end.y);

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);

        int sx = (x0 < x1) ? 1 : -1;
        int sy = (y0 < y1) ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            linePoints.Add(new Vector3(x0, y0));

            if (x0 == x1 && y0 == y1)
                break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err = err - dy;
                x0 = x0 + sx;
            }
            if (e2 < dx)
            {
                err = err + dx;
                y0 = y0 + sy;
            }
        }

        return linePoints;
    }


    public virtual void ShootProjectiles(
        ProjectileType pt, //projectileTYPE
        Vector3 targetposition,
        Vector3 sourceposition//,
        //Quaternion projectile_Rotation
        )
    {
        if (projectilePrefab != null)
        {
            projectilePrefab.GetComponent<Projectile>().projectiletype = pt;
            //Vector3 direction = transform.right;
            //if (projectile_Rotation == Quaternion.Euler(0, 0, 0))
            //{
                Vector3 direction = (targetposition - sourceposition).normalized;
            //}
            GameObject projectile 
                = Instantiate(projectilePrefab, sourceposition, Quaternion.identity);

            if (projectile != null)
            {
                //projectile.transform.rotation = projectile_Rotation;
                projectile.GetComponent<Projectile>().setdata(projectileDamage, 
                    10, direction, gameObject);
                //Debug.Log($"ROTATION IS {projectile.transform.rotation.eulerAngles}");
            }
        }
    }


    // Update is called once per frame
    protected virtual void Update()
    {
        if (disabled)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (activeEffects.Count <= 0)
            {
                //Debug.Log("EFFECT APPLIED");
                ApplyEffect(EffectType.GHOST);
            }
        }

        PlayAnimation(characterType, currentAnimIdx);
        // Update and check duration for each active effect
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            Effect effect = activeEffects[i];
            effect.UpdateEffect(Time.deltaTime);

            //ENABLE SHIELD
            if (effect.Type == EffectType.SHIELD)
            {
                //Shield sh = GetComponentInChildren<Shield>();
                if (playerShield != null)
                {
                    playerShield.shieldActive = true;
                }
            }

            if (effect.IsExpired)
            {
                emissionModule.enabled = false;
                // Remove the expired effect
                activeEffects.RemoveAt(i);
                Debug.Log($"{effect.Type} effect expired.");
            }

        }
    }

    // Apply a new effect to the player
    public void ApplyEffect(EffectType type)
    {
        // Check if the player already has the effect
        if (!HasEffect(type))
        {
            Effect newEffect = CreateEffect(type);
            activeEffects.Add(newEffect);
            Debug.Log($"{type} effect applied.");
        }
        //else
        //{
        //    Debug.Log($"Player already has {type} effect.");
        //}
    }


    
    // Check if the player has a specific effect
    public bool HasEffect(EffectType type)
    {
        return activeEffects.Exists(effect => effect.Type == type);
    }

    // Create a new effect based on the type
    private Effect CreateEffect(EffectType type)
    {
        switch (type)
        {
            case EffectType.POISON:
                {
                    if (ps != null)
                    {
                        ParticleSystem.MainModule mainModule = ps.main;
                        mainModule.startColor = Color.green;
                        emissionModule.enabled = true;
                    }

                    return new PoisonEffect(10f, 2f, 0.75f, this);
                }
            case EffectType.SHIELD:
                {
                    
                    return new ShieldEffect(15f, this);
                }
            case EffectType.BURN:
                {
                    if (ps != null)
                    {
                        ParticleSystem.MainModule mainModule = ps.main;
                        mainModule.startColor = Color.red;
                        emissionModule.enabled = true;
                        //Debug.Log("BEGIN BURN");
                    }
                    return new BurnEffect(20f, 1f, this);
                }
            case EffectType.ONE_HIT:
                {
                    if (ps != null)
                    {
                        ParticleSystem.MainModule mainModule = ps.main;
                        mainModule.startColor = Color.white;
                        emissionModule.enabled = true;
                        //Debug.Log("BEGIN BURN");
                    }
                    powerupNotificationPanel.GetComponent<PowerUpNotificationUI>().powerupName.text 
                        = "ONE HIT";
                    powerUpCanvasGroup.alpha = 1;
                    powerUpCanvasGroup.interactable = true;
                    return new OneHitEffect(10f, 3, this);
                }
            case EffectType.SPIRIT_FIRE:
                {
                    //if (ps != null)
                    //{
                    //    ParticleSystem.MainModule mainModule = ps.main;
                    //    mainModule.startColor = Color.white;
                    //    emissionModule.enabled = true;
                    //    //Debug.Log("BEGIN BURN");
                    //}
                    powerupNotificationPanel.GetComponent<PowerUpNotificationUI>().powerupName.text
                        = "SPIRIT FIRE";
                    powerUpCanvasGroup.alpha = 1;
                    powerUpCanvasGroup.interactable = true;
                    return new SpiritFireEffect(10f, this);
                }
            case EffectType.GEM_WISDOM:
                {
                    //if (ps != null)
                    //{
                    //    ParticleSystem.MainModule mainModule = ps.main;
                    //    mainModule.startColor = Color.white;
                    //    emissionModule.enabled = true;
                    //    //Debug.Log("BEGIN BURN");
                    //}
                    powerupNotificationPanel.GetComponent<PowerUpNotificationUI>().powerupName.text
                        = "GEM_WISDON";
                    powerUpCanvasGroup.alpha = 1;
                    powerUpCanvasGroup.interactable = true;
                    return new GemWisdomEffect(10f, this);
                }
            case EffectType.MINER_SENSE:
                {
                    //if (ps != null)
                    //{
                    //    ParticleSystem.MainModule mainModule = ps.main;
                    //    mainModule.startColor = Color.white;
                    //    emissionModule.enabled = true;
                    //    //Debug.Log("BEGIN BURN");
                    //}
                    powerupNotificationPanel.GetComponent<PowerUpNotificationUI>().powerupName.text
                        = "MINER SENSE";
                    powerUpCanvasGroup.alpha = 1;
                    powerUpCanvasGroup.interactable = true;
                    return new MinerSenseEffect(10f);
                }
            case EffectType.GHOST:
                {
                    //if (ps != null)
                    //{
                    //    ParticleSystem.MainModule mainModule = ps.main;
                    //    mainModule.startColor = Color.white;
                    //    emissionModule.enabled = true;
                    //    //Debug.Log("BEGIN BURN");
                    //}
                    powerupNotificationPanel.GetComponent<PowerUpNotificationUI>().powerupName.text
                        = "GHOST";
                    powerUpCanvasGroup.alpha = 1;
                    powerUpCanvasGroup.interactable = true;
                    return new GhostEffect(10f);
                }
            // Add more cases for additional effects
            default:
                Debug.LogError("Unknown effect type: {type}");
                return null;
        }
    }
}

// Enum to represent different types of player effects
public enum EffectType
{
    POISON,
    SHIELD,
    BURN,
    // Add more effect types as needed

    ONE_HIT,
    SPIRIT_FIRE,
    MINER_SENSE,
    GEM_WISDOM,
    GHOST,
}

// Base class for player effects



//ONE_HIT - character class' meleedamage increases, then reverts back to old value when duration is up
//SPIRIT_FIRE - SHOOT PROJECTILES AT FOUR DIFFERENT DIRECTIONS
//MINER_SENSE - DETECT TREASURE CHESTS by manipulation the chest's spriterender sort order
//GEM_WISDOM - INCREASE character class' projectiledamage, then reverts back to old value when duration is up
//GHOST - off the colliders of gameobjects which have "WallTileMap" tag






//POWERUPS
//public class OneHitEffect : Effect
//{
//    private int originalMeleeDamage;

//    public OneHitEffect(float duration, int meleeDamageIncrease) : base()
//    {
//        Type = EffectType.ONE_HIT;
//        Duration = duration;
//        originalMeleeDamage = meleeDamageIncrease;
//    }

//    public override void UpdateEffect(ref int health, float deltaTime)
//    {
//        base.UpdateEffect(ref health, deltaTime);
//    }

//    public override void ApplyEffect(Character character)
//    {
//        character.meleedamage = int.MaxValue; // Set to a very high value for a one-hit effect
//    }

//    public override void RevertEffect(Character character)
//    {
//        character.meleedamage = originalMeleeDamage;
//    }
//}

//public class SpiritFireEffect : Effect
//{
//    // Implement SpiritFireEffect logic
//}

//public class MinerSenseEffect : Effect
//{
//    // Implement MinerSenseEffect logic
//}

//public class GemWisdomEffect : Effect
//{
//    private int originalProjectileDamage;

//    public GemWisdomEffect(float duration, int projectileDamageIncrease) : base()
//    {
//        Type = EffectType.GEM_WISDOM;
//        Duration = duration;
//        originalProjectileDamage = projectileDamageIncrease;
//    }

//    public override void UpdateEffect(ref int health, float deltaTime)
//    {
//        base.UpdateEffect(ref health, deltaTime);
//    }

//    public override void ApplyEffect(Character character)
//    {
//        character.projectileDamage = int.MaxValue; // Set to a very high value for increased projectile damage
//    }

//    public override void RevertEffect(Character character)
//    {
//        character.projectileDamage = originalProjectileDamage;
//    }
//}

//public class GhostEffect : Effect
//{
//    // Implement GhostEffect logic
//}



//private bool hasTakenDamage = false;

//private void ApplyPoisonEffect(StatusEffect effect)
//{
//    // Damage health every 2 seconds
//    if (Mathf.Approximately(effect.Timer % 2f, 0f) && !hasTakenDamage)
//    {
//        // Adjust the damage amount as needed
//        TakeDamage(10);
//        hasTakenDamage = true; // Set the flag to true to indicate that damage has been applied
//    }
//    else if (!Mathf.Approximately(effect.Timer % 2f, 0f))
//    {
//        // Reset the flag when the next 2-second interval begins
//        hasTakenDamage = false;
//    }

//    // Slow down speed by 25%
//    // Adjust the speed reduction factor as needed
//    float speedReductionFactor = 0.75f;
//    ApplySpeedReduction(speedReductionFactor);
//}
