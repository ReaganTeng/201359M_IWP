using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Item;

public class PowerUp : Item
{

    public enum PowerUps
    {
        //ADD THESE FIRST
        ONE_HIT, //ONE HIT KILL
        SPIRIT_FIRE,// SHOOT PROJECTILES AT FOUR DIFFERENT CORNERS
        MINER_SENSE, //DETECT TREASURE CHESTS
        GEM_WISDOM,  //INCREASE GEM DAMAGE/DURATION
        GHOST, //PASS THROUGH WALLS
        //

        //
        //ODOR_OF_BEASTS,
    }

    //[HideInInspector]
    //public PowerUps powerup;

    [HideInInspector]
    public EffectType powerUpEffectType; // The type of effect the power-up applies

    [HideInInspector]
    public Dictionary<PowerUps, Dictionary<ItemType, EffectType>> powerUpitemType 
        = new Dictionary<PowerUps, Dictionary<ItemType, EffectType>>();

   
    //public SpriteRenderer powerUpIconRenderer;
    public List<Sprite> powerupIconsList = new List<Sprite>();
    //public Sprite backgroundImage;


    protected override void Awake()
    {
        base.Awake();
        // Initialize power-up durations
        InitialiseItemType();
        // Add more power-up durations as needed
    }


    void InitialiseItemType()
    {
        powerUpitemType.Add(PowerUps.ONE_HIT, new Dictionary<ItemType, EffectType>
        {
            { ItemType.ONE_HIT, EffectType.ONE_HIT },
            // Add more item types and their corresponding effect types as needed
        });
        powerUpitemType.Add(PowerUps.SPIRIT_FIRE, new Dictionary<ItemType, EffectType>
        {
            { ItemType.SPIRIT_FIRE, EffectType.SPIRIT_FIRE },
            // Add more item types and their corresponding effect types as needed
        });

        powerUpitemType.Add(PowerUps.MINER_SENSE, new Dictionary<ItemType, EffectType>
        {
            { ItemType.MINER_SENSE, EffectType.MINER_SENSE },
            // Add more item types and their corresponding effect types as needed
        });

        powerUpitemType.Add(PowerUps.GEM_WISDOM, new Dictionary<ItemType, EffectType>
        {
            { ItemType.GEM_WISDOM, EffectType.GEM_WISDOM },
            // Add more item types and their corresponding effect types as needed
        });

        powerUpitemType.Add(PowerUps.GHOST, new Dictionary<ItemType, EffectType>
        {
            { ItemType.GHOST, EffectType.GHOST },
            // Add more item types and their corresponding effect types as needed
        });
    }


   
    public void SetPowerUpItem(PowerUps powerupType, int stacksize)
    {
        Dictionary<ItemType, EffectType> itemEffects = powerUpitemType[powerupType];
        ItemType itemType = ItemType.NOTHING;
        
        
        foreach (var kvp in itemEffects)
        {
            itemType = kvp.Key;
            powerUpEffectType = kvp.Value;
        }

        //Debug.Log($"PU INDEX IS {(int)powerUpEffectType}");

        base.SetItem(itemType, stacksize);

        // Set the sprite of the power-up
        //if (powerupIconsList.Count > (int)powerUpEffectType)
        {
            Sprite iconSprite = powerupIconsList[(int)powerupType];
            itemImage.sprite = iconSprite;
        }
      
    }

    
    public override void Update()
    {
        base.Update();
    }

    //public override void OnDestroy()
    //{
    //    base.OnDestroy();
    //}

}
