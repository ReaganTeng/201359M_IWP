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
    void Awake()
    {
        slotlimit = 10;
        canvasGroup = GetComponent<CanvasGroup>();
        DisplayItems();
        AS = GetComponent<AudioSource>();


        //shopItems = new List<ShopItemData>()

        CloseDescriptionPanel();
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
        //Debug.Log("TOGGGGGGGGGGGG");
        DescriptionPanelCloseButton.interactable = !DescriptionPanelCloseButton.interactable;
        closebutton.interactable = !closebutton.interactable;
    }

    public void DisplayItems()
    {
        RectTransform shopContentRectTransform = shopContent.GetComponent<RectTransform>();


        foreach (ShopItemData item in shopItems)
        {
            //THE IF STATEMENT ARE FOR ITEMS THAT ARE INTENTED TO BE BOUGHT ONCE OR A FEW TIMES BASED ON THA SCRIPTABLEOBJECT STATS
            bool shouldDisplayItem = false;

            bool ContainsTheseItems = new HashSet<ShopItemData.ShopItem> {
                    ShopItemData.ShopItem.HEALTH_UPGRADE,
                    ShopItemData.ShopItem.DAMAGE_UPGRADE,
                    ShopItemData.ShopItem.INVENTORY_UPGRADE,
                    ShopItemData.ShopItem.VETERAN_CHARACTER,
                    ShopItemData.ShopItem.PROFESSOR_CHARACTER
                }.Contains(item.shopItem);

            // Check conditions for displaying the item
            if (
                //IF ITEM DOES NOT CONTAIN ANY OF THE FOLLOWING ITEMS
                !ContainsTheseItems
                 ||
                //IF THEY CONTAIN, MAKE SURE THEY FUFIL EXTRA CONDITIONS
                (
                    (item.shopItem == ShopItemData.ShopItem.HEALTH_UPGRADE && upgradesScriptableObject.HealthBuff < 60) ||
                    (item.shopItem == ShopItemData.ShopItem.DAMAGE_UPGRADE && upgradesScriptableObject.DamageBuff < 60) ||
                    (item.shopItem == ShopItemData.ShopItem.INVENTORY_UPGRADE && upgradesScriptableObject.slotProperty.Count < slotlimit) ||
                    (item.shopItem == ShopItemData.ShopItem.VETERAN_CHARACTER && !characterUnlockManager.unlockedCharacters.Contains(CharacterUnlockManager.CharacterType.VETERAN)) ||
                    (item.shopItem == ShopItemData.ShopItem.PROFESSOR_CHARACTER && !characterUnlockManager.unlockedCharacters.Contains(CharacterUnlockManager.CharacterType.PROFESSOR))
                )
            )
            {
                shouldDisplayItem = true;
            }

            if (shouldDisplayItem)
            {
                GameObject itemObject = Instantiate(shopitemPrefab, shopContent.transform);
                GameObject[] children = shopContentRectTransform.GetComponentsInChildren<RectTransform>()
                    .Where(child => child.CompareTag("ShopItemPanel"))
                    .Select(child => child.gameObject)
                    .ToArray();

                // Update the shopRectTransform size based on the total height of items
                if (children.Length % 3 == 0)
                {
                    // Assuming each child has a RectTransform component
                    float childHeight = children[0].GetComponent<RectTransform>().sizeDelta.y;
                    // Adjust the size of the shopRectTransform
                    shopContentRectTransform.sizeDelta = new Vector2(shopContentRectTransform.sizeDelta.x,
                        shopContentRectTransform.sizeDelta.y + (childHeight));
                }

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
            }
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
        Debug.Log($"Bought {item.itemName} for {item.price} coins.");

        // Assuming you have a Player script on the player GameObject
        //PlayerHubWorld player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHubWorld>();

        switch (item.shopItem)
        {
            case ShopItemData.ShopItem.HEALTH_UPGRADE:
                {
                    if (upgradesScriptableObject.HealthBuff < 60)
                    {
                        upgradesScriptableObject.HealthBuff += 20;
                        DeductPrice(item);

                    }

                    //INDICATE OUT OF STOCK
                    if (upgradesScriptableObject.HealthBuff >= 60)
                    {
                        shopItems.Remove(item);
                        CanvasGroup cg = itemGO.GetComponent<CanvasGroup>();
                        cg.interactable = false;
                        cg.alpha = .2f;
                    }
                    break;
                }
            case ShopItemData.ShopItem.DAMAGE_UPGRADE:
                {
                    if (upgradesScriptableObject.DamageBuff < 60)
                    {
                        upgradesScriptableObject.DamageBuff += 20;
                        DeductPrice(item);
                    }

                    if (upgradesScriptableObject.DamageBuff >= 60)
                    {
                        shopItems.Remove(item);
                        CanvasGroup cg = itemGO.GetComponent<CanvasGroup>();
                        cg.interactable = false;
                        cg.alpha = .2f;
                    }
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

                    if(upgradesScriptableObject.slotProperty.Count >= slotlimit)
                    {
                        shopItems.Remove(item);
                        CanvasGroup cg = itemGO.GetComponent<CanvasGroup>();
                        cg.interactable = false;
                        cg.alpha = .2f;
                    }
                    break;
                }
            case ShopItemData.ShopItem.PROFESSOR_CHARACTER:
                {
                    characterUnlockManager.UnlockCharacter(CharacterUnlockManager.CharacterType.PROFESSOR);
                    DeductPrice(item);
                    shopItems.Remove(item);
                    CanvasGroup cg = itemGO.GetComponent<CanvasGroup>();
                    cg.interactable = false;
                    cg.alpha = .2f;
                    break;
                }
            case ShopItemData.ShopItem.VETERAN_CHARACTER:
                {
                    characterUnlockManager.UnlockCharacter(CharacterUnlockManager.CharacterType.VETERAN);
                    shopItems.Remove(item);
                    DeductPrice(item);

                    CanvasGroup cg = itemGO.GetComponent<CanvasGroup>();
                    cg.interactable = false;
                    cg.alpha = .2f;
                    break;
                }

            //EQUIPMENT
            case ShopItemData.ShopItem.POTION:
                {
                    ItemType itemchosen = ItemType.POTION;
                    GameObject it = Instantiate(itemPrefab, transform.position, Quaternion.identity);
                    it.GetComponent<Item>().SetItem(itemchosen, 10);
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
                    break;
                }
            case ShopItemData.ShopItem.BULLET:
                {
                    ItemType itemchosen = ItemType.BULLET;
                    GameObject it = Instantiate(itemPrefab, transform.position, Quaternion.identity);
                    it.GetComponent<Item>().SetItem(itemchosen, 10);
                    
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

                    break;
                }
            case ShopItemData.ShopItem.BOMB:
                {
                    ItemType itemchosen = ItemType.BOMB;
                    GameObject it = Instantiate(itemPrefab, transform.position, Quaternion.identity);
                    it.GetComponent<Item>().SetItem(itemchosen, 10);

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
                    break;
                }
            default:
                //Debug.LogWarning($"Unknown attribute to upgrade: {item.attributeToUpgrade}");
                break;
        }


      


        //foreach (Transform child in shopContent.transform)
        //{
        //    Destroy(child.gameObject);
        //}

        //DisplayItems();

        
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
                    break;
                }
            case "GameScene":
                {
                    PlayerPrefs.SetFloat("MoneyEarned", PlayerPrefs.GetFloat("MoneyEarned")
                        - item.price);
                    break;
                }
            default:
                break;
        }

        //PLAY CHACHING SOUND
        AS.clip = chaChingClip;
        AS.Play();
    }
}
