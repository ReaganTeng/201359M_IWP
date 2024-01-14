using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    [HideInInspector]
    public GameObject instructions;
    [HideInInspector] public GameObject QuestPanel;
    [HideInInspector]
    public GameObject powerupNotificationPanel;
    [HideInInspector]
    public GameObject dialoguePanel;
    [HideInInspector]
    public GameObject ShopPanel;
    [HideInInspector]
    public GameObject GamePlayPanel;
    [HideInInspector]
    public GameObject CharacterSwitchPanel;
    [HideInInspector]
    public GameObject GameCompletePanel;
    [HideInInspector]
    public GameObject GameOverPanel;

    [HideInInspector]
    public GameObject SettingsPanel;

    //bool buttonpressed;

    public Slider MasterVolumeSlider;
    public Slider MusicVolumeSlider;
    public Slider SoundVolumeSlider;

    void Awake()
    {
        dialoguePanel = GameObject.FindGameObjectWithTag("DialoguePanel");
        powerupNotificationPanel = GameObject.FindGameObjectWithTag("PowerUpNotification");
        instructions = GameObject.FindGameObjectWithTag("Instructions");
        QuestPanel = GameObject.FindGameObjectWithTag("QuestPanel");
        ShopPanel = GameObject.FindGameObjectWithTag("ShopPanel");
        CharacterSwitchPanel = GameObject.FindGameObjectWithTag("CharacterSwitchPanel");
        GamePlayPanel = GameObject.FindGameObjectWithTag("GamePlayPanel");
        GameCompletePanel = GameObject.FindGameObjectWithTag("GameCompletePanel");
        GameOverPanel = GameObject.FindGameObjectWithTag("GameOverPanel");
        SettingsPanel = GameObject.FindGameObjectWithTag("SettingsPanel");

        togglePanel(dialoguePanel);
        togglePanel(powerupNotificationPanel);
        togglePanel(QuestPanel);
        togglePanel(ShopPanel);
        togglePanel(CharacterSwitchPanel);
        togglePanel(GameCompletePanel);
        togglePanel(GameOverPanel);
        togglePanel(SettingsPanel);

        //togglePanel(instructions);

        //GamePlayPanel.GetComponent<CanvasGroup>().interactable = true;
        //GamePlayPanel.GetComponent<CanvasGroup>().alpha = 1;
        //GamePlayPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            MasterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        }

        if (PlayerPrefs.HasKey("SoundVolume"))
        {
            SoundVolumeSlider.value = PlayerPrefs.GetFloat("SoundVolume");
        }

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            MusicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }



        OnValueChangedSliders();
    }


    public void OnValueChangedSliders()
    {
        MasterVolumeSlider.onValueChanged.AddListener((float value) => AdjustVolume("MasterVolume", MasterVolumeSlider));
        SoundVolumeSlider.onValueChanged.AddListener((float value) => AdjustVolume("SoundVolume", SoundVolumeSlider));
        MusicVolumeSlider.onValueChanged.AddListener((float value) => AdjustVolume("MusicVolume", MusicVolumeSlider));

    }

    public void AdjustVolume(string dataname, Slider slider)
    {
        //string dataname = "MasterVolume";
        float val = slider.value;
        PlayerPrefs.SetFloat(dataname, val);
        Debug.Log($"VALUE IS {val}");
        //SendJSON(dataname, val);
    }



    public void togglePanel(GameObject panel)
    {
        Debug.Log("TOGGLING PANEL");
        CanvasGroup panelCG = panel.GetComponent<CanvasGroup>();
        panelCG.interactable = !panelCG.interactable;
        panelCG.blocksRaycasts = !panelCG.blocksRaycasts;
        if(panelCG.interactable
           && panelCG.blocksRaycasts)
        {
            panelCG.alpha = 1;
        }
        else
        {
            panelCG.alpha = 0;
        }

        
    }


    public void togglePanel_CloseButton(GameObject closeButtonGO)
    {
        CanvasGroup panelCG = closeButtonGO.GetComponentInParent<CanvasGroup>();
        panelCG.interactable = !panelCG.interactable;
        panelCG.blocksRaycasts = !panelCG.blocksRaycasts;
        if (panelCG.interactable
           && panelCG.blocksRaycasts)
        {
            panelCG.alpha = 1;
        }
        else
        {
            panelCG.alpha = 0;
        }

    }


    public void Update()
    {
        //togglePanel(dialoguePanel);
        //togglePanel(powerupNotificationPanel);
        //togglePanel(instructions);
        //togglePanel(QuestPanel);

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            togglePanel(SettingsPanel);
        }


        if (!dialoguePanel.GetComponent<CanvasGroup>().interactable
        && !powerupNotificationPanel.GetComponent<CanvasGroup>().interactable
        && !QuestPanel.GetComponent<CanvasGroup>().interactable
        && !ShopPanel.GetComponent<CanvasGroup>().interactable
        && !CharacterSwitchPanel.GetComponent<CanvasGroup>().interactable
        && !GameOverPanel.GetComponent<CanvasGroup>().interactable
         && !GameCompletePanel.GetComponent<CanvasGroup>().interactable
          && !SettingsPanel.GetComponent<CanvasGroup>().interactable)
        {
            if (!GamePlayPanel.GetComponent<CanvasGroup>().interactable)
            {
                GamePlayPanel.GetComponent<CanvasGroup>().interactable = true;
                GamePlayPanel.GetComponent<CanvasGroup>().alpha = 1;
                GamePlayPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            
            Character[] charactersInScene = FindObjectsOfType<Character>();
            foreach (Character character in charactersInScene)
            {
                character.disabled = false;
            }
        }
        else
        {
            if (GamePlayPanel.GetComponent<CanvasGroup>().interactable)
            {
                GamePlayPanel.GetComponent<CanvasGroup>().interactable = false;
                GamePlayPanel.GetComponent<CanvasGroup>().alpha = 0;
                GamePlayPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }

            Character[] charactersInScene = FindObjectsOfType<Character>();
            foreach (Character character in charactersInScene)
            {
                character.disabled = true;
            }
        }

        //FREEZE ALL GAMEOBJECTS
    }

    public void toggleInstruction()
    {
        if (instructions != null)
        {
            instructions.SetActive(!instructions.activeSelf);
            if (Time.timeScale > 0)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }


    public void ReturnToHubWorld()
    {


        //LOAD BACK TO HUBWORLD;
        SceneManager.LoadScene("HubWorld");
    }


    public void ReturnToHubWorldFromGameOver()
    {

        SceneManager.LoadScene("HubWorld");

    }


    public void RestartGame()
    {
        //LOAD BACK TO HUBWORLD;
        //SceneManager.LoadScene("HubWorld");
    }


    //public void ToggleQuestPanel()
    //{
    //    QuestPanel.SetActive(!QuestPanel.activeSelf);
    //    if(QuestPanel.activeSelf )
    //    {
    //        //newQuestNot.SetActive(false);
    //        //questCompletedNot.SetActive(false);
    //    }
    //}
}
