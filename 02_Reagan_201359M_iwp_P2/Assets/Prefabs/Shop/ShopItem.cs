using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Dialogue;

public class ShopItem : MonoBehaviour
{
    //public enum ItemName
    //{
    //    HEALTH_BOOSTER
    //}


    //public enum ATTRIBUTE
    //{
    //    BOOSTER
    //}


    [System.Serializable]

    public class ShopItemData
    {
        public enum ShopItem
        {
            HEALTH_UPGRADE,
            PROFESSOR_CHARACTER,
            VETERAN_CHARACTER,
            BOMB,
            POTION,
            BULLET
        }


        public ShopItem shopItem;

        public string itemName;
        //public int itemID;
        public int price;
        public string itemDescription;


        // New properties for attribute upgrade
        //public string attributeToUpgrade;
        public int upgradeValue;

        //DECIDES WHAT ITEM TO ADD IN THE INVENTORY
        //public Item itemtoadd;

        public Sprite itemSprite;

        // Additional properties or methods can be added here

        //public void Initialize()
        //{
        //    //HARD CODE THE POTENTIAL FUNCTIONS HERE BASED ON WHAT DIALOGUEGIVE IT IS
        //    switch (shopItem)
        //    {
                
        //        default:
        //            break;
        //    }
        //}

        //void UpgradePlayerHealth()
        //{
            
        //}

    }





}
