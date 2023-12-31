using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Item;

[CreateAssetMenu(fileName = "Upgrades", menuName = "Upgrades")]
public class Upgrades : ScriptableObject
{
    //public Inventory inventory;

    public List<InventorySlot> slotsToTransfer = new List<InventorySlot>();


    public void emptySlot(InventorySlot slot)
    {
        slot.CurrentItem = null;
        slot.itemtype = ItemType.NOTHING;
        slot.Quantity = 0;
        // Assuming slot has an Image component
        slot.slotImage = null;
        slot.quantityText.text = "";
        //slot.quantityText.text = $"{slot.Quantity}";
    }
}
