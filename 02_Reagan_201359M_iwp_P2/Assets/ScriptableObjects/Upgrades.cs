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

    //[HideInInspector]
    public bool WonGame;


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

    public List<SlotProperties> slotProperty = new List<SlotProperties>();


    public void Reset()
    {
        DamageBuff = 0; 
        HealthBuff = 0; 
        WonGame = false;

        //RESET INVENTORY
        for (int i = slotProperty.Count - 1; i >= 0; i--)
        {
            emptySlotProperty(slotProperty[i]);
            // Alternatively, you can remove the line above and just do the following:
            // slotProperty.RemoveAt(i);
        }

        for (int i = 0; i < 4; i++)
        {
            Upgrades.SlotProperties sp = new Upgrades.SlotProperties();
            sp.itemtype = ItemType.NOTHING;
            slotProperty.Add(sp);
            //inventoryManager.NewSlotBought();
            //DeductPrice(item);
        }

    }

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


    // Function to save upgrades data to PlayerPrefs
    public void SaveUpgrades()
    {
        PlayerPrefs.SetInt("DamageBuff", DamageBuff);
        PlayerPrefs.SetInt("HealthBuff", HealthBuff);
        PlayerPrefs.SetInt("WonGame", WonGame ? 1 : 0);

        // Serialize the list of SlotProperties to JSON
        string slotPropertyJson = JsonUtility.ToJson(slotProperty);
        PlayerPrefs.SetString("SlotProperty", slotPropertyJson);

        PlayerPrefs.Save();
    }

    [System.Serializable]
    public class UpgradesData
    {
        public List<SlotProperties> slotProperty;
    }

    // Function to load upgrades data from PlayerPrefs
    public void LoadUpgrades()
    {
        DamageBuff = PlayerPrefs.GetInt("DamageBuff", 0);
        HealthBuff = PlayerPrefs.GetInt("HealthBuff", 0);
        WonGame = PlayerPrefs.GetInt("WonGame", 0) == 1;

        // Deserialize the JSON string back to UpgradesData
        string upgradesDataJson = PlayerPrefs.GetString("UpgradesData", "");
        UpgradesData upgradesData = JsonUtility.FromJson<UpgradesData>(upgradesDataJson);

        // Check if upgradesData is not null
        if (upgradesData != null)
        {
            slotProperty = upgradesData.slotProperty;
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
