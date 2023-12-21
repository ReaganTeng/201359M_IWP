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
    //newText.text = text;
    //// Ensure the Content object is big enough to contain all the elements
    //RectTransform contentRectTransform = panelContent.GetComponent<RectTransform>();
    //// Adjust the RectTransform of the new text element
    //RectTransform rectTransform = newText.GetComponent<RectTransform>();
    //rectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, rectTransform.sizeDelta.y * panelContent.transform.childCount);
    //rectTransform.anchoredPosition = new Vector2(0, -rectTransform.sizeDelta.y * panelContent.transform.childCount);


    public void DisplayItems()
    {
        RectTransform parentTransform = GetComponent<RectTransform>();
        //float itemHeight = itemPrefab.GetComponentInChildren<RectTransform>().rect.height;


        //foreach (ShopItemData item in shopItems)
        //{
        //    // Instantiate a new text element and set its content
        //    GameObject itemObject = Instantiate(itemPrefab, parentTransform);
        //    //newText.GetComponentInChildren<TextMeshProUGUI>().text = text;
        //    //RectTransform rectTransform = newText.GetComponent<RectTransform>();
        //    // Ensure the Content object is big enough to contain all the elements
        //    RectTransform contentRectTransform = item.GetComponent<RectTransform>();
        //    contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x,
        //        rectTransform.sizeDelta.y * itemContent.transform.childCount * 1.0f);
        //    RectTransform[] children = newText.GetComponentsInChildren<RectTransform>();
        //    //Debug.Log($"CRT {itemContent.transform.childCount}");
        //    for (int x = 0; x < children.Length; x++)
        //    {
        //        Button childButton = children[x].GetComponentInChildren<Button>();
        //        if (childButton != null)
        //        {
        //            childButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Buy for {price}";
        //            // Add a listener to the button for a click event
        //            childButton.onClick.AddListener(
        //            () =>
        //            {
        //                playfablandingmgt.BuyItem(catalogName, itemID, currencyType, int.Parse(price));
        //                Destroy(newText.gameObject);
        //            }
        //            );
        //        }
        //    }
        //}


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

        // Upgrade the player's attribute
        switch (item.attributeToUpgrade)
        {
            case "Health":
                player.IncreaseHealth(item.upgradeValue);
                break;
            //case "Damage":
            //    player.IncreaseDamage(item.upgradeValue);
            //    break;
            // Add more cases for other attributes
            default:
                Debug.LogWarning($"Unknown attribute to upgrade: {item.attributeToUpgrade}");
                break;
        }

        //DEDUCT PRICE
        PlayerPrefs.SetFloat("GrossMoney", PlayerPrefs.GetFloat("GrossMoney") - item.price);

        //PLAY CHACHING SOUND
        AS.clip = chaChingClip;
        AS.Play();
    }
}
