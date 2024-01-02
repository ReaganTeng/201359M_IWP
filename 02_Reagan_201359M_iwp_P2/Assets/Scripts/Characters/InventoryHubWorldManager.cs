using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Item;
using TMPro;
using static Upgrades;

public class InventoryHubWorldManager : MonoBehaviour
{
    //public TextMeshProUGUI moneyearned;
    public Upgrades upgrades;

    public GameObject inventoryPanel;
    public GameObject slotPrefab;


    [HideInInspector]
    public List<InventorySlot> slots;
    //public int selectedSlot;
    //[HideInInspector]
    //int numSlots;

    //Inventory playerInventory;
    public void Awake()
    {
        //FOR TESTING PURPOSES
        //foreach (InventorySlot slot in upgrades.slotsToTransfer)
        //{
        //    upgrades.emptySlot(slot);
        //}
        //
        InstantiateInventorySlots();
        //SelectSlot(0);
    }


    public void InstantiateInventorySlots()
    {
        //EMPTY ALL THE SLOTS FOR TESTING PURPOSES
        foreach (SlotProperties slot in upgrades.slotProperty)
        {
            upgrades.emptySlotProperty(slot);
        }
        //

        //for(int i = 0; i < upgrades.slotsToTransfer.Count; i++) 
        //{
        //    typesInUpgrade.Add(upgrades.slotsToTransfer[i].itemtype);
        //}

        float totalWidth = 0;

        //for (int i = 0; i < upgrades.slotsToTransfer.Count; i++)
        //{
        //    Debug.Log($"QUANTITY {upgrades.slotsToTransfer[i].Quantity} {upgrades.slotsToTransfer[i].itemtype}");
        //}

        for (int i = 0; i < upgrades.slotProperty.Count; i++)
        {
            GameObject slot = Instantiate(slotPrefab, inventoryPanel.GetComponent<RectTransform>());
            InventorySlot inventorySlotComponent = slot.GetComponent<InventorySlot>();

            SlotProperties slot2Transfer = upgrades.slotProperty[i];
            inventorySlotComponent.CurrentItem = slot2Transfer.currentitem;
            inventorySlotComponent.Quantity = slot2Transfer.Quantity;
            inventorySlotComponent.itemtype = slot2Transfer.itemtype;
            inventorySlotComponent.quantityText.text = slot2Transfer.quantitytext;
            inventorySlotComponent.slotImage.sprite = slot2Transfer.iconsprite;

            slots.Add(inventorySlotComponent);

            // Adjust the position of the instantiated slot
            RectTransform slotRect = slot.GetComponent<RectTransform>();
            slotRect.anchoredPosition = new Vector2(totalWidth, 0);
            totalWidth += slotRect.rect.width + 10;
            // Adjust the spacing (10 in this case)
            foreach (InventorySlot s in slots)
            {
                RectTransform stRect = s.GetComponent<RectTransform>();
                stRect.anchoredPosition =
                    new Vector2(
                        stRect.anchoredPosition.x - (slotRect.rect.width * .75f),
                        stRect.anchoredPosition.y);
            }
            // Customize or initialize your slot here if needed
        }
    }

    public void Update()
    {
        //for (int i = 1; i <= 3; i++)
        //{
        //    if (Input.GetKeyDown(i.ToString()))
        //    {
        //        selectedSlot = i - 1; // Adjust to zero-based index
        //                              //SelectSlot(selectedSlot);
        //        foreach (var slot in slots)
        //        {
        //            slot.GetComponent<Image>().color = Color.white;
        //        }
        //        slots[selectedSlot].GetComponent<Image>().color = Color.red;

        //        if (!slots[selectedSlot].IsEmpty())
        //        {
        //            Debug.Log($"SELECTED SLOT HAS ITEM: {slots[selectedSlot].itemtype}");
        //        }
        //    }
        //}
    }




    //amount = amount of items added
    public void AddItem(Item item, int amount)
    {
        foreach (InventorySlot slot in slots)
        {
            // If the quantity in the slot is less than the stack size of the item
            if ((slot.itemtype != ItemType.NOTHING
                && slot.Quantity < item.StackSize
                && slot.itemtype == item.type)
                || slot.itemtype == ItemType.NOTHING
                )
            {
                int remainingCapacity = item.StackSize - slot.Quantity;
                // Check if the remaining capacity is greater than the amount to add
                int amountToAdd = Mathf.Min(amount, remainingCapacity);
                // Add the item to the slot
                slot.AddItem(item, amountToAdd);
                // Subtract the added amount from the total
                amount -= amountToAdd;

                PlayerPrefs.SetFloat("MoneyEarned", PlayerPrefs.GetFloat("MoneyEarned") + item.money);
                

                Destroy(item.gameObject);
                // If we've added the required amount, break out of the loop
                if (amount <= 0)
                {
                    break;
                }
            }
            //continue;
        }

        List<SlotProperties> slotprops = upgrades.slotProperty;
        for (int i = 0; i < slotprops.Count; i++)
        {
            SlotProperties slotprop = slotprops[i];
            slotprop.currentitem = slots[i].CurrentItem;
            slotprop.itemtype = slots[i].itemtype;
            slotprop.Quantity = slots[i].Quantity;
            slotprop.iconsprite = slots[i].slotImage.sprite;
            //typesInUpgrade[i] = slots[i].itemtype;
            if (slots[i].Quantity > 0)
            {
                slotprop.quantitytext = $"{slots[i].Quantity}";
            }
            else
            {
                slotprop.quantitytext = "";
            }
            //Debug.Log($"ITEM NOW IS {slotsinupgrade[i].itemtype} {slotsinupgrade[i].Quantity} {slotsinupgrade[i].slotImage.sprite}");
        }

        //
    }



}
