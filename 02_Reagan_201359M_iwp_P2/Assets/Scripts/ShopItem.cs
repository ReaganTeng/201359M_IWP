using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Dialogue;
using System;
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

   

    [Serializable]

    public class ShopItemData
    {
        public enum ShopItem
        {
            HEALTH_UPGRADE,
            DAMAGE_UPGRADE,
            INVENTORY_UPGRADE,

            PROFESSOR_CHARACTER,
            VETERAN_CHARACTER,
            BOMB,
            POTION,
            BULLET
        }

        //public bool unlimited;
        //public int StockLimit;
        //public CanvasGroup cg;

        public ShopItem shopItem;

        public string itemName;
        //public int itemID;
        public int price;
        public string itemDescription;

        public Sprite itemSprite;

       
        void Awake()
        {
            //cg = GetComponent<CanvasGroup>();

            //if(unlimited) 
            //{
            //    StockLimit = (int)Mathf.Infinity;
            //}
        }


        //public void OutOfStock()
        //{
        //    //GetComponent<CanvasGroup>().alpha = .2f;
        //    cg.alpha = .2f;
        //    cg.interactable = false;
        //}
    }





}
