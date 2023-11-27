using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private List<Effect> activeEffects = new List<Effect>();


    public int health;
    public int damage;
    public SpriteRenderer spriteRenderer;




    //bool effectapplied = false;
    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    // Update is called once per frame
    protected virtual void Update()
    {
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
            effect.UpdateEffect(Time.deltaTime);

            if (effect.IsExpired)
            {
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
            Debug.Log("{type} effect applied.");
        }
        else
        {
            Debug.Log("Player already has {type} effect.");
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
                return new PoisonEffect(10f, 2f, 0.75f);
            case EffectType.SHIELD:
                return new ShieldEffect(15f);
            case EffectType.BURN:
                return new BurnEffect(20f, 1f);
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

    public abstract void UpdateEffect(float deltaTime);
}

// Poison effect
public class PoisonEffect : Effect
{
    private float interval;
    private float intervalTimer;
    private float damage;
    private float slowFactor;
    //private float intervalTimer;
    public PoisonEffect(float duration, float interval, float slowFactor)
    {
        intervalTimer = 0;
        Type = EffectType.POISON;
        Duration = duration;
        this.interval = interval;
        this.slowFactor = slowFactor;
        damage = 10f; // Adjust the damage amount as needed
    }

    public override void UpdateEffect(float deltaTime)
    {
        Debug.Log("EFFECT POISON " + interval);
        intervalTimer += Time.deltaTime;
        
        Duration -= deltaTime;
        if (Duration <= 0)
        {
            IsExpired = true;
            return;
        }

        // Apply poison damage at intervals
        //if (Mathf.Approximately(Duration % interval, 0f))
            if(intervalTimer >= interval)
        {
            ApplyDamage();
            intervalTimer = 0;
        }

        // Apply speed reduction
        ApplySpeedReduction();
    }

    private void ApplyDamage()
    {
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
    {
        Type = EffectType.SHIELD;
        Duration = duration;
    }

    public override void UpdateEffect(float deltaTime)
    {
        Duration -= deltaTime;
        if (Duration <= 0)
        {
            IsExpired = true;
            Debug.Log("Shield effect expired.");
        }
        

        // Implement logic for the shield effect
        Debug.Log("Shield: Player is protected.");
    }
}

// Burn effect
public class BurnEffect : Effect
{
    private float interval;
    private float damage;

    public BurnEffect(float duration, float interval)
    {
        Type = EffectType.BURN;
        Duration = duration;
        this.interval = interval;
        damage = 15f; // Adjust the damage amount as needed
    }

    public override void UpdateEffect(float deltaTime)
    {
        Duration -= deltaTime;
        if (Duration <= 0)
        {
            IsExpired = true;
            return;
        }

        // Apply burn damage at intervals
        if (Mathf.Approximately(Duration % interval, 0f))
        {
            ApplyDamage();
        }
    }

    private void ApplyDamage()
    {
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
