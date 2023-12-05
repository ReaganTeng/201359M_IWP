using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Item;
using TMPro;

public class Inventory : MonoBehaviour
{
    public TextMeshProUGUI moneyearned;


    public List<InventorySlot> slots;
    public int selectedSlot;

    Inventory playerInventory;
    public void Awake()
    {
        //itempicked.SetItem(ItemType.RED_GEM, 5);
        //AddItem(itempicked, 18);
    }

    public void Update()
    {
        for (int i = 1; i <= 3; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                selectedSlot = i - 1; // Adjust to zero-based index
                                      //SelectSlot(selectedSlot);
                foreach (var slot in slots)
                {
                    slot.GetComponent<Image>().color = Color.white;
                }
                slots[selectedSlot].GetComponent<Image>().color = Color.red;

                if (!slots[selectedSlot].IsEmpty())
                {
                    Debug.Log($"SELECTED SLOT HAS ITEM: {slots[selectedSlot].itemtype}");
                }
            }
        }
    }


    public void LoadInventory()
    {

    }

    //amount = amount of items added
    public void AddItem(Item item, int amount)
    {
        foreach (var slot in slots)
        {
            // If the quantity in the slot is less than the stack size of the item
            if ((slot.itemtype != ItemType.NOTHING 
                && slot.Quantity < item.StackSize
                && slot.itemtype == item.type)
                || slot.itemtype == ItemType.NOTHING
                )
            {
                //if (slot.CurrentItem != null)
                //{
                //    Debug.Log($"SLOT TYPE: {slot.itemtype}");
                //}
                int remainingCapacity = item.StackSize - slot.Quantity;
                // Check if the remaining capacity is greater than the amount to add
                int amountToAdd = Mathf.Min(amount, remainingCapacity);

                // Add the item to the slot
                slot.AddItem(item, amountToAdd);
                // Subtract the added amount from the total
                amount -= amountToAdd;

                PlayerPrefs.SetInt("MoneyEarned", PlayerPrefs.GetInt("MoneyEarned") + item.money);
                moneyearned.text = $"{PlayerPrefs.GetInt("MoneyEarned")}";

                Destroy(item.gameObject);
                // If we've added the required amount, break out of the loop
                if (amount <= 0)
                {
                    break;
                }
            }
        }
    }


    public void SelectSlot(int slotNumber)
    {

        // Reset colors of all slots
        foreach (var slot in slots)
        {
            slot.GetComponent<Image>().color = Color.white;
        }

        
        if (slotNumber >= 1 && slotNumber <= slots.Count)
        {

            selectedSlot = slotNumber - 1; // Convert to 0-based index
                                           // Set the color of the selected slot
            slots[selectedSlot].GetComponent<Image>().color = Color.red;
        }
        else
        {
            //Debug.Log($"Invalid slot number. Please choose a slot between 1 and {slots.Count}.");
        }
    }


    public void DisplayInventory()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            //Debug.Log($"Slot {i + 1}: {slots[i].Quantity} {slots[i].CurrentItem.type}");
        }
    }
}
