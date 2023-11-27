using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Item;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        RED_GEM,
        GREEN_GEM,
        BLUE_GEM,
        NOTHING,
        
    }

    public List<Sprite> itemSprites = new List<Sprite>();
    public SpriteRenderer itemImage;


    public int money;
    public ItemType type { get; set; }
   public int StackSize { get; set; }


    List <Player> player = new List<Player>();
    GameObject gameManager;

    private void Awake()
    {
        itemImage = GetComponent<SpriteRenderer>();
        player.Add(GameObject.FindGameObjectWithTag("Player").GetComponent<Player>());
        gameManager = GameObject.FindGameObjectWithTag("GameMGT");

       

        //StackSize = 5;
        // Example of how to set the sprite based on the item type
        //SetItemSprite(type);
    }


    public void Update()
    {

        //RESPONSIBLE FOR THE COLLECTION OF ITEMS
        if (StackSize > 0)
        {
            foreach (Player player in player)
            {
                float distance = Vector2.Distance(player.gameObject.transform.position, transform.position);
                if (distance <= 2.0f
                    && !player.AIMode)
                {
                    //player.GetComponent<Inventory>().AddItem(this, 1);
                    gameManager.GetComponent<Inventory>().AddItem(this, 1);

                }
            }
        }
    }


    public void SetItem(ItemType itemType, int stacksize)
    {
        type = itemType;
        StackSize = stacksize;
        switch (type)
        {
            case ItemType.RED_GEM: 
                money = 100;
                break;
            case ItemType.BLUE_GEM:
                money = 300;
                break;
            case ItemType.GREEN_GEM:
                money = 200;
                break;
        }
        SetItemSprite(type);
    }


    public void SetItemSprite(ItemType itemType)
    {
        itemImage = GetComponent<SpriteRenderer>();
        // Assuming your itemSprites list is populated with sprites
        if (itemSprites.Count > (int)itemType)
        {
            itemImage.sprite = itemSprites[(int)itemType];
        }
        else
        {
            Debug.LogError($"Sprite for ItemType {itemType} is missing.");
        }
    }
}
