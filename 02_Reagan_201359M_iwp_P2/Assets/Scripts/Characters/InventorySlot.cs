using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;
using TMPro;
using static Item;

public class InventorySlot : MonoBehaviour
{
    //public Item CurrentItem { get; set; }


    public Item CurrentItem;

    public ItemType itemtype;

    public int Quantity;

    // Assuming slot has an Image component
    public Image slotImage ;
    public TextMeshProUGUI quantityText;

    void Awake()
    {
        //slotImage = gameObject.GetComponentInChildren<Image>();
        //quantityText = GetComponentInChildren<TextMeshProUGUI>();
    }


    public InventorySlot()
    {
        itemtype = ItemType.NOTHING;
        CurrentItem = null;
        Quantity = 0;
    }

    public bool IsEmpty()
    {
        return itemtype == ItemType.NOTHING || Quantity == 0;

        //return CurrentItem == null || Quantity == 0;
    }

    public bool IsStackable(Item item)
    {
        return !IsEmpty() &&
            itemtype == item.type 
            && Quantity < item.StackSize;
    }

    public void AddItem(Item item, int amount)
    {
        //slotImage = GetComponentInChildren<Image>().sprite;
        //quantityText = GetComponentInChildren<TextMeshProUGUI>();

        // Set the sprite of the Image component to the item's itemimage
        if (slotImage != null)
        {
            slotImage.sprite = item.itemImage.sprite;
            //sloticon.GetComponent<Image>().sprite = item.itemImage.sprite;
            //Debug.Log($"SLOT IMG IS {item.itemImage.sprite}");
        }

        if (IsEmpty())
        {
            itemtype = item.type;
            Quantity = Mathf.Min(amount, item.StackSize);
            //itemtype = CurrentItem.type;
            Debug.Log("SLOT IS EMPTY");
        }
        else if (IsStackable(item))
        {
            //int spaceLeft = CurrentItem.StackSize - Quantity;
            int spaceLeft = item.StackSize - Quantity;
            int addedAmount = Mathf.Min(amount, spaceLeft);
            Quantity += addedAmount;
            Debug.Log("SLOT ");
        }

        if (Quantity > 1)
        {
            quantityText.text = Quantity.ToString();
        }
        Debug.Log($"ITEM ADDED");
    }



    public void RemoveItem()
    {
        //slotImage = GetComponentInChildren<Image>().sprite;
        //quantityText = GetComponentInChildren<TextMeshProUGUI>();

        // Set the sprite of the Image component to the item's itemimage
        //if (slotImage != null)
        //{
        //    slotImage.sprite = item.itemImage.sprite;
        //    //sloticon.GetComponent<Image>().sprite = item.itemImage.sprite;
        //    //Debug.Log($"SLOT IMG IS {item.itemImage.sprite}");
        //}

        //int spaceLeft = item.StackSize - Quantity;
        //int addedAmount = Mathf.Min(amount, spaceLeft);
        Quantity -= 1;
        Debug.Log("SLOT REMOVED");
        
        if(Quantity <= 0)
        {
            quantityText.text = "";
            slotImage.sprite = null;
            itemtype = ItemType.NOTHING;
        }

        if (Quantity > 0)
        {
            quantityText.text = Quantity.ToString();
        }
        Debug.Log($"ITEM ADDED");
    }
}
