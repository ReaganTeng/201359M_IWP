using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static ShopItem;
using static System.Net.Mime.MediaTypeNames;
using System.Linq;
using Unity.VisualScripting;

public class Shop : MonoBehaviour
{
    public CharacterUnlockManager characterUnlockManager;

    public List<ShopItemData> shopItems;
    public Transform itemContainer;
    public GameObject itemPrefab;
    public GameObject shopContent;
    public AudioClip chaChingClip;

    [HideInInspector] public AudioSource AS;

    void Awake()
    {
        //DisplayItems();
        AS = GetComponent<AudioSource>();
    }

    // Instantiate a new text element and set its content
    //TextMeshProUGUI newText = Instantiate(contentText, panelContent.transform);
    //newText.text = textDisplayItems()
    //// Ensure the Content object is big enough to contain all the elements
    //RectTransform contentRectTransform = panelContent.GetComponent<RectTransform>();
    //// Adjust the RectTransform of the new text element
    //RectTransform rectTransform = newText.GetComponent<RectTransform>();
    //rectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, rectTransform.sizeDelta.y * panelContent.transform.childCount);
    //rectTransform.anchoredPosition = new Vector2(0, -rectTransform.sizeDelta.y * panelContent.transform.childCount);


    public void DisplayItems()
    {
        RectTransform parentTransform = GetComponent<RectTransform>();
        
        foreach (ShopItemData item in shopItems)
        {
            GameObject itemObject = Instantiate(itemPrefab, shopContent.transform);
            RectTransform[] children = itemObject.GetComponentsInChildren<RectTransform>();

            //DISPLAY THE CONTENTS
            RectTransform itemTransform
                = children.FirstOrDefault(child =>
                child.CompareTag("ShopItemPanel")).GetComponent<RectTransform>();
            
            RectTransform shopRectTransform = shopContent.GetComponent<RectTransform>();
            shopRectTransform.sizeDelta = new Vector2(shopRectTransform.sizeDelta.x, itemTransform.sizeDelta.y * shopItems.Count);

            itemObject.GetComponentInChildren<TextMeshProUGUI>().text
                = $"{item.itemName} - {item.price} coins";
            Button buyButton = itemObject.GetComponentInChildren<Button>();
            buyButton.onClick.AddListener(() => BuyItem(item));
            Debug.Log($"FINAL HEIGHT {itemTransform.rect.height}");
        }
    }


    //void BuyItem(ShopItemData item)
    //{
        
    //}



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
            //case "Damage":
            //    player.IncreaseDamage(item.upgradeValue);
            //    break;
            // Add more cases for other attributes
            default:
                //Debug.LogWarning($"Unknown attribute to upgrade: {item.attributeToUpgrade}");
                break;
        }


        // Upgrade the player's attribute
        //switch (item.attributeToUpgrade)
        //{
        //    case "Health":
        //        player.IncreaseHealth(item.upgradeValue);
        //        break;
        //    //case "Damage":
        //    //    player.IncreaseDamage(item.upgradeValue);
        //    //    break;
        //    // Add more cases for other attributes
        //    default:
        //        Debug.LogWarning($"Unknown attribute to upgrade: {item.attributeToUpgrade}");
        //        break;
        //}

        //DEDUCT PRICE
        PlayerPrefs.SetFloat("GrossMoney", PlayerPrefs.GetFloat("GrossMoney") - item.price);

        foreach (Transform child in shopContent.transform)
        {
            Destroy(child.gameObject);
        }

        DisplayItems();

        //PLAY CHACHING SOUND
        AS.clip = chaChingClip;
        AS.Play();
    }
}
