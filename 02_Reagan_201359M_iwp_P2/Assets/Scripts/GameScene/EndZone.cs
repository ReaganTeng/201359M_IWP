using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Upgrades;
public class EndZone : MonoBehaviour
{
    //public Upgrades upgrade;
    GameObject player;

    [HideInInspector]
    public MenuManager MM;
    [HideInInspector]
    public GameObject GameCompletePanel;

    Inventory invmanager;

    //float countdown;
    //bool countdown_started;
    public float money;
    void Awake()
    {
        money = 0;
        //countdown = 0;
        //countdown_started = false;
        player = GameObject.FindWithTag("Player");
        MM = GameObject.FindWithTag("GameMGT").GetComponent<MenuManager>();
        GameCompletePanel = MM.GameCompletePanel;
        //GameCompletePanel.SetActive(false);

        invmanager = GameObject.FindGameObjectWithTag("GameMGT").GetComponent<Inventory>();
    }


    void Update()
    {
        //if (countdown_started)
        //{
        //    countdown += 1 * Time.deltaTime;

        //    if(countdown >= 2.0f)
        //    {
        //        Debug.Log("QUIT");
        //        //Application.Quit();
        //    }
        //}
    }


    void emptyInventory()
    {
        foreach (InventorySlot slot in invmanager.slots)
        {
            if (slot.itemtype != Item.ItemType.BOMB
                && slot.itemtype != Item.ItemType.POTION
                && slot.itemtype != Item.ItemType.BULLET)
            {
                slot.itemtype = Item.ItemType.NOTHING;
                slot.Quantity = 0;
                slot.quantityText.text = "";
                slot.CurrentItem = null;
                slot.slotImage.sprite = null;
            }
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")
        && !collision.gameObject.GetComponent<Player>().AIMode
        && !GameCompletePanel.GetComponent<CanvasGroup>().interactable)
        {
            PlayerPrefs.SetInt("RoundsCompleted",
            PlayerPrefs.GetInt("RoundsCompleted") + 1);
            int roundsCompleted = PlayerPrefs.GetInt("RoundsCompleted");

            //REDUCE DAYS IF CERTAIN AMOUNT OF ROUNDS COMPLETED
            if (roundsCompleted >= 2)
            {
                PlayerPrefs.SetInt("RoundsCompleted", 0);
                PlayerPrefs.SetInt("DaysLeft",
                PlayerPrefs.GetInt("DaysLeft") - 1);
            }


            emptyInventory();

            //money += (int)slot.CurrentItem.money;
            money = PlayerPrefs.GetFloat("MoneyEarned");
            PlayerPrefs.SetFloat("GrossMoney", PlayerPrefs.GetFloat("GrossMoney") + money);
            GameCompletePanel.GetComponentInChildren<TextMeshProUGUI>().text = $"GAME OVER, YOU EARNED {money}\nDAYS LEFT:\n{PlayerPrefs.GetInt("DaysLeft")}";
            PlayerPrefs.SetInt("MoneyEarned", 0);




            MM.togglePanel(GameCompletePanel);
            //    }
            //}

            //EMPTY EVERYTHING IN INVENTORY
            //put in upgrade scriptableobject also
            //List<InventorySlot> slotsinupgrade = upgrade.slotsToTransfer;
            //foreach (InventorySlot slots in slotsinupgrade)
            //{
            //    upgrade.emptySlot(slots);
            //}
            //


            //GameCompletePanel.SetActive(true);
            //countdown_started = true;


            //Time.timeScale = 0;

        }
    }
}
