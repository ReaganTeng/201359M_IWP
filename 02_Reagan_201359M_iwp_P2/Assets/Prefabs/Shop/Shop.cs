using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static ShopItem;
using static System.Net.Mime.MediaTypeNames;
using System.Linq;
using Unity.VisualScripting;
using static Item;
using Image = UnityEngine.UI.Image;
using UnityEngine.SceneManagement;


public class Shop : MonoBehaviour
{
    public CharacterUnlockManager characterUnlockManager;
    //public InventoryHubWorldManager inventoryHubWorldManager;
    public Inventory inventoryManager;

    public List<ShopItemData> shopItems;

    public GameObject shopitemPrefab;
    public GameObject itemPrefab;
    [HideInInspector]
    public CanvasGroup canvasGroup;
    public GameObject shopContent;
    public AudioClip chaChingClip;

    [HideInInspector] public AudioSource AS;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        DisplayItems();
        AS = GetComponent<AudioSource>();
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
            if (children.Length % 3 == 0)
            {
                // Assuming each child has a RectTransform component
                float childHeight = children[0].GetComponent<RectTransform>().sizeDelta.y;
                // Adjust the size of the shopRectTransform
                shopContentRectTransform.sizeDelta = new Vector2(shopContentRectTransform.sizeDelta.x,
                    shopContentRectTransform.sizeDelta.y + (childHeight));

                Debug.Log($"SIZE IT");
            }

            //SET THE UI IMAGE AND TEXT
            itemObject.GetComponent<ShopItemUI>().ItemName.text 
                = $"{item.itemName} - {item.price} coins";
            Image sourceImg = itemObject.GetComponent<ShopItemUI>().ItemImage;
            sourceImg.sprite = item.itemSprite;
            Button buyButton = itemObject.GetComponentInChildren<Button>();
            buyButton.onClick.AddListener(() => BuyItem(item));
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


    void BuyItem(ShopItemData item)
    {
        // Implement logic for buying the item
        Debug.Log($"Bought {item.itemName} for {item.price} coins.");

        // Assuming you have a Player script on the player GameObject
        PlayerHubWorld player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHubWorld>();

        switch (item.shopItem)
        {
            case ShopItemData.ShopItem.HEALTH_UPGRADE:
                player.IncreaseHealth(item.upgradeValue);
                break;
            case ShopItemData.ShopItem.PROFESSOR_CHARACTER:
                characterUnlockManager.UnlockCharacter(CharacterUnlockManager.CharacterType.PROFESSOR);
                shopItems.Remove(item);
                break;
            case ShopItemData.ShopItem.VETERAN_CHARACTER:

                characterUnlockManager.UnlockCharacter(CharacterUnlockManager.CharacterType.VETERAN);
                shopItems.Remove(item);
                break;



            //EQUIPMENT
            case ShopItemData.ShopItem.POTION:
                {
                    ItemType itemchosen = ItemType.POTION;
                    GameObject it = Instantiate(itemPrefab, transform.position, Quaternion.identity);
                    it.GetComponent<Item>().SetItem(itemchosen, 10);
                    //if (inventoryHubWorldManager != null)
                    //{
                    //    inventoryHubWorldManager.AddItem(it.GetComponent<Item>(), 1);
                    //}
                    if (inventoryManager != null)
                    {
                        inventoryManager.AddItem(it.GetComponent<Item>(), 1);
                    }
                    //Destroy(it);
                    break;
                }
            case ShopItemData.ShopItem.BULLET:
                {
                    ItemType itemchosen = ItemType.BULLET;
                    GameObject it = Instantiate(itemPrefab, transform.position, Quaternion.identity);
                    it.GetComponent<Item>().SetItem(itemchosen, 10);
                    //if (inventoryHubWorldManager != null)
                    //{
                    //    inventoryHubWorldManager.AddItem(it.GetComponent<Item>(), 1);
                    //    //inventoryHubWorldManager.InstantiateInventorySlots();
                    //}
                    if (inventoryManager != null)
                    {
                        inventoryManager.AddItem(it.GetComponent<Item>(), 1);
                    }
                   // Destroy(it);
                    break;
                }
            case ShopItemData.ShopItem.BOMB:
                {
                    ItemType itemchosen = ItemType.BOMB;
                    GameObject it = Instantiate(itemPrefab, transform.position, Quaternion.identity);
                    it.GetComponent<Item>().SetItem(itemchosen, 10);
                    //if (inventoryHubWorldManager != null)
                    //{ 
                    //    inventoryHubWorldManager.AddItem(it.GetComponent<Item>(), 1);
                    //    //inventoryHubWorldManager.InstantiateInventorySlots();
                    //}
                    if (inventoryManager != null)
                    {
                        inventoryManager.AddItem(it.GetComponent<Item>(), 1);
                    }
                    //Destroy(it);
                    break;
                }
            default:
                //Debug.LogWarning($"Unknown attribute to upgrade: {item.attributeToUpgrade}");
                break;
        }


        //DEDUCT PRICE
        // Get the currently active scene
        Scene currentScene = SceneManager.GetActiveScene();
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


        //foreach (Transform child in shopContent.transform)
        //{
        //    Destroy(child.gameObject);
        //}

        //DisplayItems();

        //PLAY CHACHING SOUND
        AS.clip = chaChingClip;
        AS.Play();
    }
}
