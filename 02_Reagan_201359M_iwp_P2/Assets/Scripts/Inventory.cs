using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Item;
using TMPro;
using System.Linq;
using UnityEditor;
using static Upgrades;

public class Inventory : MonoBehaviour
{
    public TextMeshProUGUI moneyearned;
    public Upgrades upgrades;

    [HideInInspector]
    public List<InventorySlot> slots;

    //public List<ItemType> typesInUpgrade = new List<ItemType>();

    public int selectedSlot;

    public GameObject slotPrefab;
    public GameObject inventoryPanel;



    //Inventory playerInventory;
    void Awake()
    {
        //itempicked.SetItem(ItemType.RED_GEM, 5);
        //AddItem(itempicked, 18);
        selectedSlot = 0;
        InstantiateInventorySlots();
        //SelectSlot(0);
    }

   


    void InstantiateInventorySlots()
    {
        foreach (SlotProperties slot in upgrades.slotProperty)
        {
            upgrades.emptySlotProperty(slot);
        }

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

            if (!inventorySlotComponent.IsEmpty()
                && inventorySlotComponent.slotImage != null)
            {
                Color color = inventorySlotComponent.slotImage.color;
                color.a = 1.0f;
                inventorySlotComponent.slotImage.color = color;
                //sloticon.GetComponent<Image>().sprite = item.itemImage.sprite;
            }
            else
            {
                Color color = inventorySlotComponent.slotImage.color;
                color.a = 0.0f;
                inventorySlotComponent.slotImage.color = color;
            }

            //Debug.Log("InventorySlotCom")
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
                        stRect.anchoredPosition.x - (slotRect.rect.width * .85f),
                        stRect.anchoredPosition.y);
            }
            // Customize or initialize your slot here if needed
        }
    }



    public void Update()
    {

        //EMPTY ALL THE SLOTS FOR TESTING PURPOSES
        //if (Input.GetKey(KeyCode.K))
        //{
        //    foreach (SlotProperties slot in upgrades.slotProperty)
        //    {
        //        upgrades.emptySlotProperty(slot);
        //    }
        //}
        //   

        List<SlotProperties> slotprops = upgrades.slotProperty;
        for (int i = 0; i < slotprops.Count; i++)
        {
            if (slotprops[i].Quantity != slots[i].Quantity
                || slotprops[i].itemtype != slots[i].itemtype
                || slotprops[i].currentitem != slots[i].CurrentItem
                || slotprops[i].iconsprite != slots[i].slotImage.sprite)
            {

                SlotProperties slotprop = slotprops[i];
                slotprop.currentitem = slots[i].CurrentItem;
                slotprop.itemtype = slots[i].itemtype;
                slotprop.Quantity = slots[i].Quantity;
                slotprop.iconsprite = slots[i].slotImage.sprite;
                //typesInUpgrade[i] = slots[i].itemtype;
                if (slots[i].Quantity > 0)
                {
                    //Debug.Log("ADDING TO UPGRADES");
                    slotprop.quantitytext = $"{slotprop.Quantity}";
                }
                else
                {
                    slotprop.quantitytext = "";
                }
            }
            //Debug.Log($"ITEM NOW IS {slotsinupgrade[i].itemtype} {slotsinupgrade[i].Quantity} {slotsinupgrade[i].slotImage.sprite}");
        }



        for (int i = 1; i < slots.Count; i++)
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
                moneyearned.text = $"{PlayerPrefs.GetFloat("MoneyEarned")}";

                Destroy(item.gameObject);
                // If we've added the required amount, break out of the loop
                if (amount <= 0)
                {
                    break;
                }
            }
            //continue;
        }

        //foreach(InventorySlot slot in slots)
        //{
        //    slots.Remove(slot);
        //    Destroy(slot.gameObject);
        //}
        //InstantiateInventorySlots();

        slots[selectedSlot].GetComponent<Image>().color = Color.red;
        //EditorUtility.SetDirty(upgrades);
        //AssetDatabase.SaveAssets();
        //
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







