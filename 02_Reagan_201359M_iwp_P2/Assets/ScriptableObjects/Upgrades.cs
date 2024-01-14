using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Item;

[CreateAssetMenu(fileName = "Upgrades", menuName = "Upgrades")]
public class Upgrades : ScriptableObject
{
    public int DamageBuff;
    public int HealthBuff;

    //public Inventory inventory;
    [System.Serializable]
    public class SlotProperties
    {
        public Item currentitem;
        public ItemType itemtype;
        public Sprite iconsprite;
        public int Quantity;
        public string quantitytext;
    }

    //public List<InventorySlot> slotsToTransfer = new List<InventorySlot>();

    public List<SlotProperties> slotProperty = new List<SlotProperties>();


    public void emptySlotProperty(SlotProperties slotP)
    {
        slotP.currentitem = null;
        slotP.itemtype = ItemType.NOTHING;
        slotP.Quantity = 0;
        slotP.iconsprite = null;
        if (slotP.Quantity > 0)
        {
            slotP.quantitytext = $"{slotP.Quantity}";
        }
        else
        {
            slotP.quantitytext = "";
        }
    }


    //public void emptySlot(InventorySlot slot)
    //{
    //    slot.CurrentItem = null;
    //    slot.itemtype = ItemType.NOTHING;
    //    slot.Quantity = 0;
    //    // Assuming slot has an Image component
    //    slot.slotImage.sprite = null;
    //    //slot.slotImage = null;
    //    //slot.quantityText.text = $"{slot.Quantity}";
    //    if (slot.Quantity > 0)
    //    {
    //        slot.quantityText.text = $"{slot.Quantity}";
    //    }
    //    else
    //    {
    //        slot.quantityText.text = "";
    //    }
    //    //slot.quantityText.text = $"{slot.Quantity}";
    //}
}
