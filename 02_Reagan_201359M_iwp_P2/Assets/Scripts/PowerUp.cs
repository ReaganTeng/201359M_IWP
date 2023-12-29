using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
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
        ODOR_OF_BEASTS,
        
    }

    public enum EffectType
    {
        ONE_HIT,
        SPIRIT_FIRE,
        MINER_SENSE,
        GEM_WISDOM,
        GHOST,
    }


    [HideInInspector]
    public PowerUps powerup;

    public EffectType powerUpEffectType; // The type of effect the power-up applies
    public Dictionary<EffectType, float> powerUpDurations = new Dictionary<EffectType, float>();

    private void Start()
    {
        // Initialize power-up durations
        powerUpDurations.Add(EffectType.ONE_HIT, 10f);
        powerUpDurations.Add(EffectType.SPIRIT_FIRE, 15f);
        powerUpDurations.Add(EffectType.MINER_SENSE, 20f);
        powerUpDurations.Add(EffectType.GEM_WISDOM, 12f);
        powerUpDurations.Add(EffectType.GHOST, 8f);
        // Add more power-up durations as needed
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyPowerUpEffect(other.gameObject);
            Destroy(gameObject);
        }
    }

    private void ApplyPowerUpEffect(GameObject player)
    {
        Character characterScript = player.GetComponent<Character>();
        if (characterScript != null && powerUpDurations.ContainsKey(powerUpEffectType))
        {
            float duration = powerUpDurations[powerUpEffectType];

            // Apply the power-up effect to the player with the determined duration
            //characterScript.ApplyEffect(powerUpEffectType, duration);
            Debug.Log($"Power-up applied: {powerUpEffectType}, Duration: {duration} seconds");
        }
    }


    //public GameObject powerUpPrefab;
    //void Update()
    //{
    //    // Example: Check for user input to spawn a power-up
    //    if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        SpawnPowerUp();
    //    }
    //}

    //void SpawnPowerUp()
    //{
    //    // Instantiate the power-up prefab at the character's position
    //    Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
    //}
}
