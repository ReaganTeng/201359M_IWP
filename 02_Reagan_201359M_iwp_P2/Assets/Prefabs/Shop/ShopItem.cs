using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public string itemName;
        public int itemID;
        public int price;


        // New properties for attribute upgrade
        public string attributeToUpgrade;
        public int upgradeValue;

        //DECIDES WHAT ITEM TO ADD IN THE INVENTORY
        public Item itemtoadd;

        // Additional properties or methods can be added here
    }
}
