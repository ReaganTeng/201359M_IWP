using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

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

    [HideInInspector]
    public GameObject PausePanel;


    //WHEN THESE PANELS ARE UP, FREEZE THE GAME
    [HideInInspector]
    public List<GameObject> panelsToFreezegame = new List<GameObject>();


    public Upgrades upgrades;


    //bool buttonpressed;

    //public Slider MasterVolumeSlider;
    //public Slider MusicVolumeSlider;
    //public Slider SoundVolumeSlider;

    void Awake()
    {
        GamePlayPanel = GameObject.FindGameObjectWithTag("GamePlayPanel");



        dialoguePanel = GameObject.FindGameObjectWithTag("DialoguePanel");
        powerupNotificationPanel = GameObject.FindGameObjectWithTag("PowerUpNotification");
        instructions = GameObject.FindGameObjectWithTag("Instructions");
        QuestPanel = GameObject.FindGameObjectWithTag("QuestPanel");
        ShopPanel = GameObject.FindGameObjectWithTag("ShopPanel");
        CharacterSwitchPanel = GameObject.FindGameObjectWithTag("CharacterSwitchPanel");
        GameCompletePanel = GameObject.FindGameObjectWithTag("GameCompletePanel");
        GameOverPanel = GameObject.FindGameObjectWithTag("GameOverPanel");
        SettingsPanel = GameObject.FindGameObjectWithTag("SettingsPanel");
        PausePanel = GameObject.FindGameObjectWithTag("PausePanel");

        toggleAndPanelToList(dialoguePanel);
        toggleAndPanelToList(powerupNotificationPanel);
        toggleAndPanelToList(QuestPanel);
        toggleAndPanelToList(ShopPanel);
        toggleAndPanelToList(CharacterSwitchPanel);
        toggleAndPanelToList(GameCompletePanel);
        toggleAndPanelToList(GameOverPanel);
        toggleAndPanelToList(SettingsPanel);
        toggleAndPanelToList(PausePanel);

        //togglePanel(instructions);

        //GamePlayPanel.GetComponent<CanvasGroup>().interactable = true;
        //GamePlayPanel.GetComponent<CanvasGroup>().alpha = 1;
        //GamePlayPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;


        //OnValueChangedSliders();
    }


    //public void OnValueChangedSliders()
    //{
    //    MasterVolumeSlider.onValueChanged.AddListener((float value) => AdjustVolume("MasterVolume", MasterVolumeSlider));
    //    SoundVolumeSlider.onValueChanged.AddListener((float value) => AdjustVolume("SoundVolume", SoundVolumeSlider));
    //    MusicVolumeSlider.onValueChanged.AddListener((float value) => AdjustVolume("MusicVolume", MusicVolumeSlider));

    //}

    //public void AdjustVolume(string dataname, Slider slider)
    //{
    //    //string dataname = "MasterVolume";
    //    float val = slider.value;
    //    PlayerPrefs.SetFloat(dataname, val);
    //    Debug.Log($"VALUE IS {val}");
    //    //SendJSON(dataname, val);
    //}



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


    public void toggleAndPanelToList(GameObject panel)
    {
        //Debug.Log("TOGGLING PANEL");
        CanvasGroup panelCG = panel.GetComponent<CanvasGroup>();
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

        panelsToFreezegame.Add(panel);

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
            togglePanel(PausePanel);
        }


        if (panelsToFreezegame.All(
            template => !template.GetComponent<CanvasGroup>().interactable)
            )
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
        Inventory invmanager = GameObject.FindGameObjectWithTag("GameMGT").GetComponent<Inventory>();

        foreach (InventorySlot slot in invmanager.slots)
        {
            if (slot.itemtype != Item.ItemType.BOMB
                && slot.itemtype != Item.ItemType.POTION
                && slot.itemtype != Item.ItemType.BULLET)
            {
                slot.itemtype = Item.ItemType.NOTHING;
                slot.Quantity = 0;
                slot.quantityText.text = "";
                slot.CurrentItem = null;
                slot.slotImage.sprite = null;
            }
        }
        invmanager.ChangesInInventory();
        upgrades.SaveUpgrades();
        //LOAD BACK TO HUBWORLD;
        SceneManager.LoadScene("HubWorld");
    }

    public void QuitGame()
    {
        Inventory invmanager = GameObject.FindGameObjectWithTag("GameMGT").GetComponent<Inventory>();

        foreach (InventorySlot slot in invmanager.slots)
        {
            if (slot.itemtype != Item.ItemType.BOMB
                && slot.itemtype != Item.ItemType.POTION
                && slot.itemtype != Item.ItemType.BULLET)
            {
                slot.itemtype = Item.ItemType.NOTHING;
                slot.Quantity = 0;
                slot.quantityText.text = "";
                slot.CurrentItem = null;
                slot.slotImage.sprite = null;
            }
        }


        invmanager.ChangesInInventory();
        upgrades.SaveUpgrades();
        PlayerPrefs.SetFloat("MoneyEarned", 0);

        SceneManager.LoadScene("HubWorld");
    }

    public void ReturnToHubWorldFromGameOver()
    {
        Inventory invmanager = GameObject.FindGameObjectWithTag("GameMGT").GetComponent<Inventory>();
        PlayerPrefs.SetFloat("MoneyEarned", 0);
        //foreach (InventorySlot slot in invmanager.slots)
        //{
        //    if (slot.itemtype != Item.ItemType.BOMB
        //        && slot.itemtype != Item.ItemType.POTION
        //        && slot.itemtype != Item.ItemType.BULLET)
        //    {
        //        slot.itemtype = Item.ItemType.NOTHING;
        //        slot.Quantity = 0;
        //        slot.quantityText.text = "";
        //        slot.CurrentItem = null;
        //        slot.slotImage.sprite = null;
        //    }
        //}
        //invmanager.ChangesInInventory();
        //upgrades.SaveUpgrades();


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
