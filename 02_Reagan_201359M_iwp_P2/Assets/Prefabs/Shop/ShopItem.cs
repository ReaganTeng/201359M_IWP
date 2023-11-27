using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    [System.Serializable]
    public class ShopItemData
    {
        public string itemName;
        public int itemID;
        public int price;

        // Additional properties or methods can be added here
    }
}
