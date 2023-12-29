using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditor.U2D;
using UnityEngine;
using static CharacterUnlockManager;


[System.Serializable]
public class CharacterAnimationClips
{
    public CharacterUnlockManager.CharacterType characterType;
    public List<AnimationClip> animationClips;
}

public class Character : MonoBehaviour
{
    //[SerializeField]
    public List<AudioClip> audioclips;


    //[SerializeField]
    public List<CharacterAnimationClips> characterAnimations = new List<CharacterAnimationClips>();

    // public Dictionary

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
    float speed;

    [HideInInspector]
    public bool disabled;

    [HideInInspector]
    public int projectileDamage;


    public CharacterUnlockManager.CharacterType characterType;

    //bool effectapplied = false;
    protected virtual void Awake()
    {
        currentAnimIdx = 0;
        disabled = false;
        animatorComponent = GetComponent<Animator>();
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


    public void PlayAnimation(CharacterUnlockManager.CharacterType characterType, int clipIDX)
    {
        AnimatorController animController = animatorComponent.runtimeAnimatorController as AnimatorController;
        CharacterAnimationClips characterAnimationEntry =
            characterAnimations.Find(entry => entry.characterType == characterType);
        AnimationClip animationClipToPlay = characterAnimationEntry.animationClips[clipIDX];

        if (animatorComponent != null)
        {
            string stateName = "clip"; // Replace with the actual name of your state
            int layerIndex = 0; // Assuming the state is on the base layer, change if needed

            // Check if the Animator Controller is not null
            if (animController != null)
            {
                // Loop through the states in the specified layer
                for (int stateIndex = 0; stateIndex < animController.layers[layerIndex].stateMachine.states.Length; stateIndex++)
                {
                    AnimatorState state = animController.layers[layerIndex].stateMachine.states[stateIndex].state;

                    // Check if the state has the specified name
                    if (state.name == stateName)
                    {
                        // Assign a new animation clip
                        state.motion = animationClipToPlay;
                        // Play the assigned animation clip
                        animatorComponent.Play(stateName);
                        return; // Exit the loop once the state is found and handled
                    }
                }
                Debug.LogError($"Animator state '{stateName}' not found in layer {layerIndex}.");
            }
            else
            {
                Debug.LogError("Animator Controller not found.");
            }
        }
        else
        {
            Debug.LogError("Animator Controller not found.");
        }

    }


    //public AnimationClip GetCurrentAnimationClip()
    //{
    //    // Check the current animation clip that is playing
    //    foreach (AnimationState state in animationComponent)
    //    {
    //        if (animationComponent.IsPlaying(state.name))
    //        {
    //            // state.name is the name of the currently playing animation clip
    //            Debug.Log("Current Animation Clip: " + state.name);
    //            break;
    //            return state.name;
    //        }
    //    }
    //    return state.name;
    //}


    // Update is called once per frame
    protected virtual void Update()
    {
        if (disabled)
        {
            return;
        }


        //if (Input.GetKey(KeyCode.Space))
        //{
        //    if (activeEffects.Count <= 0
        //        //&& !effectapplied
        //        )
        //    {
        //        Debug.Log("EFFECT APPLIED");
        //        ApplyEffect(EffectType.POISON);
        //        //effectapplied = true;
        //        //for(int i = 0; i < activeEffects.Count; i++)
        //        //{
        //        //    if (activeEffects[i].)
        //        //    {

        //        //    }
        //        //}
        //    }
        //}

        // Update and check duration for each active effect
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            Effect effect = activeEffects[i];
            effect.UpdateEffect(ref health, Time.deltaTime);

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
        else
        {
            Debug.Log($"Player already has {type} effect.");
        }
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
                    return new PoisonEffect(10f, 2f, 0.75f);
                }
            case EffectType.SHIELD:
                {
                    
                    return new ShieldEffect(15f);
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
                    return new BurnEffect(20f, 1f);
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
}

// Base class for player effects
public abstract class Effect
{
    public EffectType Type { get; protected set; }
    public float Duration { get; protected set; }
    public bool IsExpired { get; protected set; }


    public Color particleColor { get; protected set; }


    //public int health { get; protected set; }

    // Constructor that takes a reference to player's health
    //public Effect(ref int health)
    //{
    //    this.health = health;
    //}

    public virtual void UpdateEffect(ref int health, float deltaTime)
    {
        Duration -= deltaTime;
        if (Duration <= 0)
        {
            IsExpired = true;
            return;
        }

        Debug.Log("UPDATING EFFECT");
       
    }
}

// Poison effect
public class PoisonEffect : Effect
{
    private float interval;
    private float intervalTimer;
    private int damage;
    private float slowFactor;
    //private float intervalTimer;
    public PoisonEffect(float duration, float interval, float slowFactor)
    {
        intervalTimer = 0;
        Type = EffectType.POISON;
        Duration = duration;
        this.interval = interval;
        this.slowFactor = slowFactor;
        damage = 10; // Adjust the damage amount as needed
    }

    public override void UpdateEffect(ref int health, float deltaTime)
    {
        base.UpdateEffect(ref health, deltaTime);

        Debug.Log("EFFECT POISON " + interval);
        intervalTimer += Time.deltaTime;
        
        // Apply poison damage at intervals
        //if (Mathf.Approximately(Duration % interval, 0f))
        if(intervalTimer >= interval)
        {
            ApplyDamage(ref health);
            intervalTimer = 0;
        }

        // Apply speed reduction
        ApplySpeedReduction();
    }

    private void ApplyDamage(ref int health)
    {
        health -= damage;
        //Debug.Log($"HEALTH REDUCED TO {health}");
        // Implement logic to damage the player
        Debug.Log($"Poison: Player takes {damage} damage.");
    }

    private void ApplySpeedReduction()
    {
        // Implement logic to slow down the player
        Debug.Log("Poison: Player is slowed down.");
    }
}

// Shield effect
public class ShieldEffect : Effect
{ 
    public ShieldEffect(float duration)
        //: base(ref health)
    {
        Type = EffectType.SHIELD;
            Duration = duration;
    }

    public override void UpdateEffect(ref int health, float deltaTime)
    {
        
        base.UpdateEffect(ref health, deltaTime);


        // Implement logic for the shield effect
        Debug.Log("Shield: Player is protected.");
    }
}

// Burn effect
public class BurnEffect : Effect
{
    private float interval;
    private int damage;

    private float intervalTimer;

    public BurnEffect(float duration, float interval)
        //base(ref health)
    {
        intervalTimer = 0;
        Type = EffectType.BURN;
        Duration = duration;
        this.interval = interval;
        damage = 15; // Adjust the damage amount as needed
    }

    public override void UpdateEffect(ref int health, float deltaTime)
    {
        base.UpdateEffect(ref health, deltaTime);


        Debug.Log("UPDATING BURN EFFECT");

        // Apply burn damage at intervals
        intervalTimer += Time.deltaTime;

        // Apply poison damage at intervals
        //if (Mathf.Approximately(Duration % interval, 0f))
        if (intervalTimer >= interval)
        {
            ApplyDamage(ref health);
            intervalTimer = 0;
        }
    }

    private void ApplyDamage(ref int health)
    {
        health -=damage;
        // Implement logic to damage the player
        Debug.Log($"Burn: Player takes {damage} damage.");
    }
}


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
