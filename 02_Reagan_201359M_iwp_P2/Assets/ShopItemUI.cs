using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public Image ItemImage;
    public TextMeshProUGUI ItemName;
    public TextMeshProUGUI ItemPrice;
    public Button buyButton;
    public Button desButton;


    public GameObject Notice;

    [HideInInspector] public ShopItem.ShopItemData.ShopItem itemtobuy;
    //[HideInInspector]
    //public bool canbuy = true;

    //public void Awake()
    //{
    //    Notice.SetActive(false);
    //}

    //CHECK WHETHER ITEM IS AVAILABLE TO BUY
    public void ItemAvaliability(bool avaliableToBuy, string reasonCannotBuy = "")
    {

        if(!avaliableToBuy)
        {
            Notice.SetActive(true);
            Notice.GetComponentInChildren<TextMeshProUGUI>().text = reasonCannotBuy;
            buyButton.enabled = false;
            
        }
        else
        {
            Notice.SetActive(false);
            buyButton.enabled = true;
        }
    }




    // Start is called before the first frame update

}
