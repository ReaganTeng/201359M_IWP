using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static ShopItem;
using System.Linq;
using static Item;
using Image = UnityEngine.UI.Image;
using UnityEngine.SceneManagement;




public class Shop : MonoBehaviour
{

    public GameObject descriptionPanel;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;
    public Button DescriptionPanelCloseButton;

    public Button closebutton;

    public CharacterUnlockManager characterUnlockManager;
    //public InventoryHubWorldManager inventoryHubWorldManager;

    public Upgrades upgradesScriptableObject;

    public Inventory inventoryManager;

    public List<ShopItemData> shopItems;

    public GameObject shopitemPrefab;
    public GameObject itemPrefab;
    [HideInInspector]
    public CanvasGroup canvasGroup;
    public GameObject shopContent;
    public AudioClip chaChingClip;

    [HideInInspector] public AudioSource AS;
    int slotlimit;

    public TextMeshProUGUI Money;

    void Awake()
    {
        characterUnlockManager.LoadData();
        //characterUnlockManager.SaveData();

        slotlimit = 10;
        canvasGroup = GetComponent<CanvasGroup>();
        DisplayItems();
        AS = GetComponent<AudioSource>();

        CloseDescriptionPanel();

        Scene currentScene = SceneManager.GetActiveScene();

        //DEDUCT PRICE
        // Get the currently active scene
        switch (currentScene.name)
        {
            case "HubWorld":
                {  
                    Money.text = $"${PlayerPrefs.GetFloat("GrossMoney")}";
                    break;
                }
            case "GameScene":
                {
                    Money.text = $"${PlayerPrefs.GetFloat("MoneyEarned")}";
                    break;
                }
            default:
                break;
        }

    }




    public void CloseDescriptionPanel()
    {
        CanvasGroup cg = descriptionPanel.GetComponent<CanvasGroup>();
        cg.interactable = false;
        cg.blocksRaycasts = false;
        cg.alpha = 0;
        DescriptionPanelCloseButton.interactable = false;
        closebutton.interactable = true;
    }

    void ToggleDescriptionPanel(ShopItemData item)
    {
        CanvasGroup cg = descriptionPanel.GetComponent<CanvasGroup>();
        cg.interactable = !cg.interactable;
        cg.blocksRaycasts = !cg.blocksRaycasts;
        if(cg.interactable)
        {
            cg.alpha = 1;
        }
        else
        {
            cg.alpha =0 ;
        }

        if (item != null)
        {
            itemDescription.text = item.itemDescription;
            itemName.text = item.itemName;
        }
        DescriptionPanelCloseButton.interactable = !DescriptionPanelCloseButton.interactable;
        closebutton.interactable = !closebutton.interactable;
    }

    public void DisplayItems()
    {
        RectTransform shopContentRectTransform = shopContent.GetComponent<RectTransform>();

        foreach (ShopItemData item in shopItems)
        {
            GameObject itemObject = Instantiate(shopitemPrefab, shopContent.transform);
            GameObject[] children = shopContentRectTransform.GetComponentsInChildren<RectTransform>()
                .Where(child => child.CompareTag("ShopItemPanel"))
                .Select(child => child.gameObject)
                .ToArray();

            // Update the shopRectTransform size based on the total height of items
            //if (children.Length % 3 == 0)
            //{
            //    // Assuming each child has a RectTransform component
            //    float childHeight = children[0].GetComponent<RectTransform>().sizeDelta.y;
            //    // Adjust the size of the shopRectTransform
            //    shopContentRectTransform.sizeDelta = new Vector2(shopContentRectTransform.sizeDelta.x,
            //        shopContentRectTransform.sizeDelta.y + (childHeight));
            //}

            //SET THE UI IMAGE AND TEXT
            ShopItemUI itemUI = itemObject.GetComponent<ShopItemUI>();
            itemUI.ItemName.text
                = $"{item.itemName}";
            itemUI.ItemPrice.text = $"{item.price} MINE$";
            Image sourceImg = itemObject.GetComponent<ShopItemUI>().ItemImage;
            sourceImg.sprite = item.itemSprite;
            Button buyButton = itemUI.buyButton;
            buyButton.onClick.AddListener(() => BuyItem(item, itemObject));
            Button DescriptionButton = itemUI.desButton;
            DescriptionButton.onClick.AddListener(() => ToggleDescriptionPanel(item));
            itemUI.itemtobuy = item.shopItem;

        }

        //CheckItemAvailability();
    }

    private void Update()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        //DEDUCT PRICE
        // Get the currently active scene
        switch (currentScene.name)
        {
            case "HubWorld":
                {
                    Money.text = $"${PlayerPrefs.GetFloat("GrossMoney")}";

                    break;
                }
            case "GameScene":
                {
                    Money.text = $"${PlayerPrefs.GetFloat("MoneyEarned")}";

                    break;
                }
            default:
                break;
        }


    }
    public void CheckItemAvailability()
    {
        foreach (ShopItemUI item in shopContent.GetComponentsInChildren<ShopItemUI>())
        {
            //THE IF STATEMENT ARE FOR ITEMS THAT ARE INTENTED TO BE BOUGHT ONCE OR A FEW TIMES BASED ON THA SCRIPTABLEOBJECT STATS
            bool available = false;
            string reason = "";
            {
                //bool ContainsTheseItems = new HashSet<ShopItemData.ShopItem> {
                //    ShopItemData.ShopItem.HEALTH_UPGRADE,
                //    ShopItemData.ShopItem.DAMAGE_UPGRADE,
                //    ShopItemData.ShopItem.INVENTORY_UPGRADE,
                //    ShopItemData.ShopItem.VETERAN_CHARACTER,
                //    ShopItemData.ShopItem.PROFESSOR_CHARACTER
                //}.Contains(item.shopItem);
            }

            ShopItemData.ShopItem itemtobuy = item.itemtobuy;

            // Check conditions for displaying the item
            if (
                //IF ITEM CONTAINS THESE ITEMS AND FUFIL THESE CONDITIONS
                (
                    (itemtobuy == ShopItemData.ShopItem.HEALTH_UPGRADE
                    && upgradesScriptableObject.HealthBuff < 60) ||
                    (itemtobuy == ShopItemData.ShopItem.DAMAGE_UPGRADE
                    && upgradesScriptableObject.DamageBuff < 60) ||
                    (itemtobuy == ShopItemData.ShopItem.INVENTORY_UPGRADE
                    && upgradesScriptableObject.slotProperty.Count < slotlimit) ||
                    (itemtobuy == ShopItemData.ShopItem.VETERAN_CHARACTER
                    && !characterUnlockManager.unlockedCharacters.Contains(
                        CharacterUnlockManager.CharacterType.VETERAN)) ||
                    (itemtobuy == ShopItemData.ShopItem.PROFESSOR_CHARACTER
                    && !characterUnlockManager.unlockedCharacters.Contains(
                        CharacterUnlockManager.CharacterType.PROFESSOR)) ||
                     (itemtobuy == ShopItemData.ShopItem.BOMB
                    && InventoryValid(10, ItemType.BOMB)) ||
                     (itemtobuy == ShopItemData.ShopItem.BULLET
                    && InventoryValid(10, ItemType.BULLET))
                    ||
                     (itemtobuy == ShopItemData.ShopItem.POTION
                    && InventoryValid(10, ItemType.POTION))
                )
            )
            {
                Debug.Log("CAN BUY");
                available = true;
            }
            else
            {
                Debug.Log("CANNOT BUY");
                available = false;

                switch (itemtobuy)
                {
                    //CASE 1: INVENTORY IS ALREADY FULL
                    case ShopItemData.ShopItem.BOMB:
                    case ShopItemData.ShopItem.POTION:
                    case ShopItemData.ShopItem.BULLET:
                        {
                            reason = "Inventory Full";
                            break;
                        }
                    //CASE 2 - ALREADY  FULLY UPGRADED
                    case ShopItemData.ShopItem.HEALTH_UPGRADE:
                    case ShopItemData.ShopItem.INVENTORY_UPGRADE:
                    case ShopItemData.ShopItem.DAMAGE_UPGRADE:
                        {
                            reason = "Fully Upgraded";
                            break;
                        }
                    //CASE 3 - HEALTH ALREADY UPGRADED
                    case ShopItemData.ShopItem.VETERAN_CHARACTER:
                    case ShopItemData.ShopItem.PROFESSOR_CHARACTER:
                        {
                            reason = "Already bought";
                            break;
                        }   
                    default:
                        break;

                }
            }

            
            item.ItemAvaliability(available, reason);
        }
    }


    //void BuyItem(ShopItemData item)
    //{

    //}



    public void closePanel()
    {
        //shopPanel.SetActive(!shopPanel.activeSelf);
        canvasGroup.interactable = !canvasGroup.interactable;
        if (canvasGroup.interactable)
        {
            canvasGroup.alpha = 1;
        }
        else
        {
            canvasGroup.alpha = 0;
        }
    }

    //SIZE IT IN CANVAS
    //RectTransform rectTransform = itemObject.GetComponent<RectTransform>();
    //rectTransform.sizeDelta 
    //    = new Vector2(GetComponent<RectTransform>().sizeDelta.x, 
    //    rectTransform.sizeDelta.y * GetComponent<RectTransform>().transform.childCount);
    //rectTransform.anchoredPosition 
    //    = new Vector2(0, 
    //    -rectTransform.sizeDelta.y * GetComponent<RectTransform>().transform.childCount);

    //LOGIC ON WHAT HAPPENS WHEN AN ITEM IS BOUGHT

    #region BuyingItems
    void BuyItem(ShopItemData item, GameObject itemGO)
    {
        Scene currentScene = SceneManager.GetActiveScene();

        //IF PLAYER HAVE NOT ENOUGH MONEY
        if ((currentScene.name == "HubWorld" && PlayerPrefs.GetFloat("GrossMoney") < item.price)
            || (currentScene.name == "GameScene" && PlayerPrefs.GetFloat("MoneyEarned") < item.price))
        {
            return;
        }

        // Implement logic for buying the item
        //Debug.Log($"Bought {item.itemName} for {item.price} coins.");

        // Assuming you have a Player script on the player GameObject
        //PlayerHubWorld player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHubWorld>();

        switch (item.shopItem)
        {
            #region BuyUpgrades
            case ShopItemData.ShopItem.HEALTH_UPGRADE:
                {
                    BuyUpgrades(item, itemGO,
                       ref upgradesScriptableObject.HealthBuff, 60,
                       20);
                    break;
                }
            case ShopItemData.ShopItem.DAMAGE_UPGRADE:
                {
                    BuyUpgrades(item, itemGO,
                       ref upgradesScriptableObject.DamageBuff, 60,
                       20);
                    break;
                }
            case ShopItemData.ShopItem.INVENTORY_UPGRADE:
                {
                    if (upgradesScriptableObject.slotProperty.Count < slotlimit)
                    {
                        Upgrades.SlotProperties sp = new Upgrades.SlotProperties();
                        sp.itemtype = ItemType.NOTHING;
                        upgradesScriptableObject.slotProperty.Add(sp);
                        inventoryManager.NewSlotBought();
                        DeductPrice(item);
                    }

                    //if(upgradesScriptableObject.slotProperty.Count >= slotlimit)
                    //{
                    //    shopItems.Remove(item);
                    //    CanvasGroup cg = itemGO.GetComponent<CanvasGroup>();
                    //    cg.interactable = false;
                    //    cg.alpha = .2f;
                    //}
                    break;
                }
            #endregion
            //BUY CHARACTER
            #region BuyCharacter
            case ShopItemData.ShopItem.PROFESSOR_CHARACTER:
                {
                    BuyCharacter(CharacterUnlockManager.CharacterType.PROFESSOR
                        , item, itemGO);
                    break;
                }
            case ShopItemData.ShopItem.VETERAN_CHARACTER:
                {
                    BuyCharacter(CharacterUnlockManager.CharacterType.VETERAN
                        , item, itemGO);
                    break;
                }
            #endregion
            //EQUIPMENT
            #region Buy Equipment
            case ShopItemData.ShopItem.POTION:
                {
                    BuyEquipment(ItemType.POTION, 10, item);
                    break;
                }
            case ShopItemData.ShopItem.BULLET:
                {
                    BuyEquipment(ItemType.BULLET, 10, item);
                    break;
                }
            case ShopItemData.ShopItem.BOMB:
                {
                    BuyEquipment(ItemType.BOMB, 10, item);
                    break;
                }
            #endregion
            default:
                break;
        }

        upgradesScriptableObject.SaveUpgrades();
        CheckItemAvailability();
        //DisplayItems();
    }


    void BuyUpgrades(ShopItemData item, GameObject itemGO,
        ref int statToUpgrade, int statLimit, 
        int statIncreaseIncrement)
    {
        if (statToUpgrade < statLimit)
        {
            statToUpgrade += statIncreaseIncrement;
            DeductPrice(item);
        }

        //if (statToUpgrade >= statLimit)
        //{
        //    shopItems.Remove(item);
        //    CanvasGroup cg = itemGO.GetComponent<CanvasGroup>();
        //    cg.interactable = false;
        //    cg.alpha = .2f;
        //}
    }

    void BuyEquipment(ItemType typebought, int stacksize, ShopItemData item)
    {
        ItemType itemchosen = typebought;
        GameObject it = Instantiate(itemPrefab, transform.position, Quaternion.identity);
        it.GetComponent<Item>().SetItem(itemchosen, stacksize);
        if (inventoryManager != null)
        {
            if (inventoryManager.AddItem(it.GetComponent<Item>(), 1))
            {
                DeductPrice(item);
            }
            else
            {
                Destroy(it);
            }
        }
    }

    void BuyCharacter(CharacterUnlockManager.CharacterType characterType, ShopItemData item, GameObject itemGO)
    {
        characterUnlockManager.UnlockCharacter(characterType);
        //characterUnlockManager.SaveData();
        
        
        //shopItems.Remove(item);
        DeductPrice(item);
        //CanvasGroup cg = itemGO.GetComponent<CanvasGroup>();
        //cg.interactable = false;
        //cg.alpha = .2f;
    }
    #endregion

    //CHECK WHETHER INVENTORY IS ALD FULL OF EQUIPMENT (BOMB, POTION, BULLET)
    bool InventoryValid(int maxstacksize, ItemType itemtype)
    {
        bool inventoryNotfull = false;
       
        foreach(InventorySlot slot in inventoryManager.slots)
        {
            if(slot.itemtype == ItemType.NOTHING
                ||
                (slot.itemtype == itemtype 
                && slot.Quantity < maxstacksize)
                )
            {
                Debug.Log("INVENTORY VALID");
                inventoryNotfull = true;
            }
        }
        return inventoryNotfull;
    }

    public void DeductPrice(ShopItemData item)
    {
        Scene currentScene = SceneManager.GetActiveScene();

        //DEDUCT PRICE
        // Get the currently active scene
        switch (currentScene.name)
        {
            case "HubWorld":
                {
                    PlayerPrefs.SetFloat("GrossMoney", PlayerPrefs.GetFloat("GrossMoney")
                        - item.price);
                    Money.text = $"${PlayerPrefs.GetFloat("GrossMoney")}";
                    break;
                }
            case "GameScene":
                {
                    PlayerPrefs.SetFloat("MoneyEarned", PlayerPrefs.GetFloat("MoneyEarned")
                        - item.price);
                    Money.text = $"${PlayerPrefs.GetFloat("MoneyEarned")}";
                    break;
                }
            default:
                break;
        }

        FindAnyObjectByType<UIElementAnimations>().VibrateUI(Money.gameObject, .5f, 
            2.0f, Money.gameObject.transform.position); 

        //PLAY CHACHING SOUND
        AS.clip = chaChingClip;
        AS.Play();
    }
}
