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

    public List<Sprite> powerupSprites;


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
                powerupImage.sprite = powerupSprites[0];
                powerupDescription.text = "MORE STRENGTH IN THE SWING, MORE DAMAGE TO THE ENEMY";
                }

            },
            { EffectType.MINER_SENSE, () =>

                    { powerupImage.sprite = powerupSprites[1];
                        powerupDescription.text = "ENHANCES YOUR SENSE OF WHERE THE TRASURE IS";
                    }

            }
            ,
            { EffectType.GHOST, () =>
               { powerupImage.sprite = powerupSprites[2];
                        powerupDescription.text = "YOU CAN WAL THROUGH WALLS";
                    }

            },
            { EffectType.SPIRIT_FIRE, () =>
                { powerupImage.sprite = powerupSprites[3];
                        powerupDescription.text = "GIVE YOU POWER TO UNLEASH UNLIMITED PROJECTILES";
                    }

            },
            { EffectType.GEM_WISDOM, () =>
                { powerupImage.sprite = powerupSprites[4];
                        powerupDescription.text = "ENHANCES RED GEM AND GREEN GEM DAMAGE";
                    }
            },
            //
            
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NotifyPowerUp(EffectType effectType)
    {
        if (itemActions.ContainsKey(effectType))
        {
            itemActions[effectType].Invoke();
        }

        powerUpCanvasGroup.alpha = 1;
        powerUpCanvasGroup.interactable = true;
    }


    public void closeUI()
    {

        powerUpCanvasGroup.alpha = 0;
        powerUpCanvasGroup.interactable = false;
    }
}
