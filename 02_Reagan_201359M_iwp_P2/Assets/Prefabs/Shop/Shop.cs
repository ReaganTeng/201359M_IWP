using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static ShopItem;
using static System.Net.Mime.MediaTypeNames;
using System.Linq;

public class Shop : MonoBehaviour
{
    public List<ShopItemData> shopItems;
    public Transform itemContainer;
    public GameObject itemPrefab;

    void Awake()
    {
        //DisplayItems();
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
        
        foreach (ShopItemData item in shopItems)
        {
            GameObject itemObject = Instantiate(itemPrefab, parentTransform);
            RectTransform[] children = itemObject.GetComponentsInChildren<RectTransform>();
            RectTransform itemTransform
                = children.FirstOrDefault(child =>
                child.CompareTag("ShopItemPanel")).GetComponent<RectTransform>();
            float itemHeight = itemTransform.rect.height;
            // Set other properties of the RectTransform as needed
            //itemTransform.localPosition = new Vector3(0f, -itemHeight * shopItems.IndexOf(item), 0f);
            //itemTransform.localScale = new Vector3(1.0f, 1.0f, 0f);
            itemTransform.sizeDelta = 
                new Vector2(parentTransform.sizeDelta.x,
                parentTransform.sizeDelta.y * shopItems.Count);
            itemTransform.anchoredPosition 
                = new Vector2(0, 
                -parentTransform.sizeDelta.y 
                * shopItems.Count);




            itemObject.GetComponentInChildren<TextMeshProUGUI>().text 
                = $"{item.itemName} - {item.price} coins";
            Button buyButton = itemObject.GetComponentInChildren<Button>();
            buyButton.onClick.AddListener(() => BuyItem(item));
            Debug.Log($"FINAL HEIGHT {itemTransform.rect.height}");


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
    }
}
