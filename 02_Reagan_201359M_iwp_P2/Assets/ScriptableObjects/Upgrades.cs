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

    [HideInInspector]
    public bool finishedLoaded;

    //RESET WHEN GAME OVER
    public void Reset()
    {
        DamageBuff = 0; 
        HealthBuff = 0; 
        WonGame = false;


        //RESET INVENTORY
        //for (int i = slotProperty.Count - 1; i >= 0; i--)
        //{
        //    emptySlotProperty(slotProperty[i]);
        //    // Alternatively, you can remove the line above and just do the following:
        //    // slotProperty.RemoveAt(i);
        //}

        slotProperty.Clear();


        for (int i = 0; i < 4; i++)
        {
            SlotProperties sp = new SlotProperties();
            slotProperty.Add(sp);
            emptySlotProperty(sp);

            //inventoryManager.NewSlotBought();
            //DeductPrice(item);
        }

        SaveUpgrades();
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

    [System.Serializable]
    public class UpgradesData
    {
        public List<SlotProperties> slotProp;
    }

    // Function to save upgrades data to PlayerPrefs
    public void SaveUpgrades()
    {
        //List<SlotProperties> SP = slotProperty;
        //foreach (InventorySlot slot in SP.slots)
        //{
        //    if (slot.itemtype != Item.ItemType.BOMB
        //        && slot.itemtype != Item.ItemType.POTION
        //        && slot.itemtype != Item.ItemType.BULLET)
        //    {
        //        slot.itemtype = Item.ItemType.NOTHING;
        //        slot.Quantity = 0;
        //        slot.quantityText.text = "";
        //        slot.CurrentItem = null;
        //        slot.slotImage.sprite = null;
        //    }
        //}

       

        // Serialize the upgrades data to a JSON string
        UpgradesData upgradesData = new UpgradesData
        {
            slotProp = slotProperty
        };

        string upgradesDataJson = JsonUtility.ToJson(upgradesData);

        // Save other individual variables
        PlayerPrefs.SetInt("DamageBuff", DamageBuff);
        PlayerPrefs.SetInt("HealthBuff", HealthBuff);
        PlayerPrefs.SetInt("WonGame", WonGame ? 1 : 0);

        // Save the JSON string
        PlayerPrefs.SetString("UpgradesData", upgradesDataJson);
        //Debug.Log($"SLOTTY {upgradesDataJson}");

        // Make sure to call PlayerPrefs.Save() to persist the changes immediately
        PlayerPrefs.Save();

        Debug.Log($"UPGRADES SAVED {PlayerPrefs.GetString("UpgradesData")}");
    }



    // Function to load upgrades data from PlayerPrefs
    public void LoadUpgrades()
    {
        finishedLoaded = false;

        //if (
        //    !PlayerPrefs.HasKey("DamageBuff") ||
        //    !PlayerPrefs.HasKey("HealthBuff") ||
        //    !PlayerPrefs.HasKey("UpgradesData")
        //)
        //{
        //    PlayerPrefs.SetInt("DamageBuff", 0);
        //    PlayerPrefs.SetInt("HealthBuff", 0);
        //    PlayerPrefs.SetString("UpgradesData", "");
        //}

        DamageBuff = 0;
        HealthBuff = 0;
        WonGame = false;
        for (int i = 0; i < slotProperty.Count; i++)
        {
            //SlotProperties sp = new SlotProperties();
            emptySlotProperty(slotProperty[i]);
        }



        DamageBuff = PlayerPrefs.GetInt("DamageBuff", 0);
        HealthBuff = PlayerPrefs.GetInt("HealthBuff", 0);
        WonGame = PlayerPrefs.GetInt("WonGame", 0) == 1;

        // Deserialize the JSON string back to UpgradesData
        string upgradesDataJson = PlayerPrefs.GetString("UpgradesData", "");
        UpgradesData upgradesData = JsonUtility.FromJson<UpgradesData>(upgradesDataJson);

        // Check if upgradesData is not null
        if (upgradesData != null)
        {
            slotProperty = upgradesData.slotProp;
            //Debug.Log($"SLOTTY {slotProperty}");
        }

        //SaveUpgrades();

        finishedLoaded = true;
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
