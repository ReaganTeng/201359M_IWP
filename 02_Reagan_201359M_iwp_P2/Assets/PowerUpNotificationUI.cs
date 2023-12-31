using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpNotificationUI : MonoBehaviour
{
    public TextMeshProUGUI powerupName;
    public TextMeshProUGUI powerupDescription;
    public Image powerupImage;


    CanvasGroup powerUpCanvasGroup;
    // Start is called before the first frame update
    void Start()
    {
        powerUpCanvasGroup
            = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void closeUI()
    {
        
        powerUpCanvasGroup.alpha = 0;
        powerUpCanvasGroup.interactable = false;
    }
}
