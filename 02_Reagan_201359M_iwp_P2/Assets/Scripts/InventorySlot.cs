using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;
using TMPro;
using static Item;

public class InventorySlot : MonoBehaviour
{
    [HideInInspector]
    public Item CurrentItem;
    [HideInInspector]
    public ItemType itemtype;
    [HideInInspector]
    public int Quantity;

    public Image slotImage ;
    public TextMeshProUGUI quantityText;

   
    void Awake()
    {
        
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
    }

    public bool IsStackable(Item item)
    {
        return !IsEmpty() &&
            itemtype == item.type 
            && Quantity < item.StackSize;
    }

    public void AddItem(Item item, int amount)
    {
        
        // Set the sprite of the Image component to the item's itemimage
        if (slotImage != null)
        {
            slotImage.sprite = item.itemImage.sprite;
            Color color = slotImage.color;
            color.a = 1.0f;
            slotImage.color = color;
            //Debug.Log($"SLOT IMG IS {slotImage.sprite} SLOT");
            //Debug.Log($"SLOT ALPHA IS IS {slotImage.color.a} SLOT");
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
            //Debug.Log("SLOT ");
        }

        if (Quantity > 0)
        {
            quantityText.text = $"{Quantity}";
        }
        //Debug.Log($"ITEM ADDED");
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
        //Debug.Log("SLOT REMOVED");
        
        if(Quantity <= 0)
        {
            quantityText.text = "";
            slotImage.sprite = null;
            Color color = slotImage.color;
            color.a = 0.0f;
            slotImage.color = color;
            itemtype = ItemType.NOTHING;
            GetComponent<Image>().color = Color.white;
        }

        if (Quantity > 0)
        {
            quantityText.text = $"{Quantity}";
        }
        //Debug.Log($"ITEM ADDED");
    }
}
