using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Item;
using static Projectile;

public class PowerUpNotificationUI : MonoBehaviour
{
    public TextMeshProUGUI powerupName;
    public TextMeshProUGUI powerupDescription;
    public Image powerupImage;


    CanvasGroup powerUpCanvasGroup;

    [HideInInspector]
    public Dictionary<EffectType, Action> itemActions = new Dictionary<EffectType, Action>();

    // Start is called before the first frame update
    void Start()
    {
        powerUpCanvasGroup
            = GetComponent<CanvasGroup>();

        itemActions = new Dictionary<EffectType, Action>
        {
            { EffectType.ONE_HIT, () =>
                {
                    powerupName.text = "ONE HIT";
                    powerupDescription.text = "MORE STRENGTH IN THE SWING, MORE DAMAGE TO THE ENEMY";
                }
            },
            { EffectType.MINER_SENSE, () =>
                {
                    powerupName.text = "MINER SENSE";
                    powerupDescription.text = "ENHANCES YOUR SENSE OF WHERE THE TRASURE IS";
                }
            }
            ,
            { EffectType.GHOST, () =>
               {
                    powerupName.text = "GHOST";
                    powerupDescription.text = "YOU CAN WALK THROUGH WALLS, BUT BE CAREFUL NOT TO GET STUCK AFTER POWER UP EXPIRES";
               }
            },
            { EffectType.SPIRIT_FIRE, () =>
                {
                    powerupName.text = "SPIRIT FIRE";
                    powerupDescription.text = "GIVE YOU POWER TO UNLEASH UNLIMITED PROJECTILES";
                }
            },
            { EffectType.GEM_WISDOM, () =>
                {
                    powerupName.text = "GEM WISDOM";
                    powerupDescription.text = "ENHANCES RED GEM AND GREEN GEM DAMAGE AS PROJECTILES";
                }
            },
            //
            
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NotifyPowerUp(EffectType effectType, Sprite sprite)
    {
        if (itemActions.ContainsKey(effectType))
        {
            itemActions[effectType].Invoke();
        }

        powerupImage.sprite = sprite;

        powerUpCanvasGroup.alpha = 1;
        powerUpCanvasGroup.interactable = true;
        powerUpCanvasGroup.blocksRaycasts = true;
    }


    public void closeUI()
    {

        powerUpCanvasGroup.alpha = 0;
        powerUpCanvasGroup.interactable = false;
        powerUpCanvasGroup.blocksRaycasts = false;
    }
}
