using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Item;
using TMPro;

using static Upgrades;


public class Inventory : MonoBehaviour
{
    public TextMeshProUGUI moneyearned;
    public Upgrades upgrades;

    public GameObject inventoryPanelContent;


    public AudioClip PowerUpAudio;
    //public AudioClip

    [HideInInspector]
    public List<InventorySlot> slots;

    //public List<ItemType> typesInUpgrade = new List<ItemType>();
    [HideInInspector]
    public int selectedSlot;

    //public GameObject slotPrefab;
    public GameObject inventoryPanel;

    Vector2 normalSlotScale;
    Vector2 largeSlotScale;


    InventorySlot[] slotsincontent;


    [HideInInspector]
    public AudioSource AS;

    public AudioClip collectedSound;

    UIElementAnimations UIAnims;
    //DECIDES WHEN TO FADE OUT
    float fadeOutTimer;
    public CanvasGroup moneyPanel;

    //int slotScale;
    //Inventory playerInventory;
    void Awake()
    {
        upgrades.LoadUpgrades();


        if (moneyPanel != null)
        {
            moneyPanel.alpha = .0f;
        }

        fadeOutTimer = .0f;
        slotsincontent = inventoryPanelContent.GetComponentsInChildren<InventorySlot>();
        normalSlotScale = new Vector2(1, 1);
        largeSlotScale = new Vector2(2, 2);
        AS = GetComponent<AudioSource>();
        UIAnims = GetComponent<UIElementAnimations>();
        selectedSlot = 0;
        InitialiseInventorySlots();
        //SelectSlot(0);
        //slotScale = 1;
        //itempicked.SetItem(ItemType.RED_GEM, 5);
        //AddItem(itempicked, 18);

        //MANUALLY SET THE VOLUME OF THE COLLECTIBLE CLIP ITSELD
        if (collectedSound != null)
        {
            float[] samples = new float[collectedSound.samples * collectedSound.channels];
            collectedSound.GetData(samples, 0);
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] *= 5;
            }
            collectedSound.SetData(samples, 0);
        }
        //

        //Debug.Log("init");

    }


   

    public void EmptyInventory()
    {
        foreach (var slot in slots)
        {
            slot.EmptySlot();
        }
    }

    public void InitialiseInventorySlots()
    {
        //FOR TEMPORARY TESTING, EMPTIES ALL THE SLOTS
        //foreach (SlotProperties slot in upgrades.slotProperty)
        //{
        //    upgrades.emptySlotProperty(slot);
        //}

        //float totalWidth = 0;


        InventorySlot[] slotsincontent = inventoryPanelContent.GetComponentsInChildren<InventorySlot>(); 

        foreach(InventorySlot slot in slotsincontent)
        {
            slot.gameObject.SetActive(false);
        }

        for (int i = 0; i < upgrades.slotProperty.Count; i++)
        {
            InventorySlot currentslot = slotsincontent[i];
            currentslot.gameObject.SetActive(true);

            InventorySlot inventorySlotComponent = currentslot.GetComponent<InventorySlot>();

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

            ////Debug.Log("InventorySlotCom")
            slots.Add(inventorySlotComponent);

            //// Adjust the position of the instantiated slot
            //RectTransform slotRect = slot.GetComponent<RectTransform>();
            //slotRect.anchoredPosition = new Vector2(totalWidth, 0);
            //totalWidth += slotRect.rect.width + 10;
            //// Adjust the spacing (10 in this case)
            //foreach (InventorySlot s in slots)
            //{
            //    RectTransform stRect = s.GetComponent<RectTransform>();
            //    stRect.anchoredPosition =
            //        new Vector2(
            //            stRect.anchoredPosition.x - (slotRect.rect.width * .85f),
            //            stRect.anchoredPosition.y);
            //}
            //// Customize or initialize your slot here if needed
        }

        //CHANGE THS SHOP
        FindObjectOfType<Shop>().CheckItemAvailability();
    }



    public void Update()
    {
        if (!upgrades.finishedLoaded)
        {
            return;
        }


        InventorySelection();

        ChangesInInventory();


        if(moneyPanel == null)
        {
            return;
        }
        if (fadeOutTimer > 0)
        {
            fadeOutTimer -= Time.deltaTime;

            if(fadeOutTimer <= 0)
            {
                UIAnims.FadeOutAnimation(moneyPanel, 5.0f);
            }
        }
    }



    void InventorySelection()
    {
        

        for (int i = 1; i < upgrades.slotProperty.Count + 1; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                selectedSlot = i - 1; // Adjust to zero-based index
                                      //SelectSlot(selectedSlot);
                foreach (var slot in slots)
                {
                    slot.GetComponent<RectTransform>().localScale = normalSlotScale;
                }
                slots[selectedSlot].GetComponent<RectTransform>().localScale = largeSlotScale;

                //if (!slots[selectedSlot].IsEmpty())
                //{
                //    Debug.Log($"SELECTED SLOT HAS ITEM: {slots[selectedSlot].itemtype}");
                //}
            }
        }
    }

    public void ChangesInInventory()
    {
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

        FindObjectOfType<Shop>().CheckItemAvailability();

        //SaveUpgradesOnQuit2();
    }

    

    //amount = amount of items added
    public bool AddItem(Item item, int amount)
    {
        bool successFullyAddedItem = false;

        //FIND THE SUITABLE SLOT THAT FUFILS THESEE CRITERIA
        //1. MUST BE THE SAME ITEMTYPE
        //2. THE QUANTITY INSIDE MUST BE LESS THAN THE STACKSIZE OF THAT ITEM
        InventorySlot SuitableSlot = slots.Find(template => 
        template.itemtype == item.type
        && template.Quantity < item.StackSize);

        //IF FOUND SUITABLE SLOT
        if (SuitableSlot != null)
        {
            AddItemToSlot(item, SuitableSlot, amount);
            successFullyAddedItem = true;
        }
        //IF DID NOT FINS
        else
        {
            foreach (InventorySlot slot in slots)
            {
                if (slot.itemtype == ItemType.NOTHING)
                {
                    AddItemToSlot(item, slot, amount);
                    successFullyAddedItem = true;

                    // If we've added the required amount, break out of the loop
                    //if (amount <= 0)
                    {
                        break;
                    }
                }
            }
        }
       
        slots[selectedSlot].GetComponent<RectTransform>().localScale = largeSlotScale;
        //
        return successFullyAddedItem;
    }


    void AddItemToSlot(Item item, InventorySlot slot, int amount)
    {
        int remainingCapacity = item.StackSize - slot.Quantity;
        // Check if the remaining capacity is greater than the amount to add
        int amountToAdd = Mathf.Min(amount, remainingCapacity);
        // Add the item to the slot
        slot.AddItem(item, amountToAdd);
        // Subtract the added amount from the total
        amount -= amountToAdd;
        PlayerPrefs.SetFloat("MoneyEarned", PlayerPrefs.GetFloat("MoneyEarned") + item.money);

        if (moneyearned != null)
        {
            moneyearned.text = $"${PlayerPrefs.GetFloat("MoneyEarned")}";   
        }
        AS.time = 0;
        AS.clip = collectedSound;
        AS.Play();

        if (moneyPanel != null)
        {

            if (UIAnims.fadeOutCoroutine != null)
            {
                StopCoroutine(UIAnims.fadeOutCoroutine);
            }
            fadeOutTimer = 2.0f;
            UIAnims.FadeInAnimation(moneyPanel, .5f);
            UIAnims.VibrateUI(moneyearned.gameObject, 1.0f,
                2.0f, moneyearned.gameObject.transform.position);
            //UIAnims.FadeOutAnimation(moneyPanel, fadeTimer);
        }


        Destroy(item.gameObject);
    }


    public void SelectSlot(int slotNumber)
    {
        // Reset colors of all slots
        foreach (var slot in slots)
        {
            slot.GetComponent<RectTransform>().localScale = normalSlotScale;
        }

        if (slotNumber >= 1 && slotNumber <= slots.Count)
        { 
            selectedSlot = slotNumber - 1; // Convert to 0-based index
                                           // Set the color of the selected slot
            slots[selectedSlot].GetComponent<RectTransform>().localScale = largeSlotScale;
        }
        
    }


    public void NewSlotBought()
    {
        for (int i = 0; i < upgrades.slotProperty.Count; i++)
        {
            InventorySlot currentslot = slotsincontent[i];
            currentslot.gameObject.SetActive(true);
            InventorySlot inventorySlotComponent = currentslot.GetComponent<InventorySlot>();
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

            if (i == upgrades.slotProperty.Count - 1)
            {
                slots.Add(inventorySlotComponent);
            }
        }
    }



    private void SaveUpgradesOnQuit2()
    {
        // Save upgrades when the game is about to close
        upgrades.SaveUpgrades();

        // Wait for a short time to ensure the data is saved before quitting
        //yield return new WaitForSeconds(1f);

        //// Quit the application
        //Application.Quit();
    }


    //private void OnApplicationQuit()
    //{
    //    // Save upgrades when the application is quitting
    //    StartCoroutine(SaveUpgradesOnQuit());
    //}

    //private IEnumerator SaveUpgradesOnQuit()
    //{
    //    // Save upgrades when the game is about to close
    //    upgrades.SaveUpgrades();

    //    // Wait for a short time to ensure the data is saved before quitting
    //    yield return new WaitForSeconds(1f);

    //    // Quit the application
    //    Application.Quit();
    //}

    //private void OnDestroy()
    //{
    //    // Unsubscribe from the application quit event
    //    Application.wantsToQuit -= SaveUpgradesOnQuit;
    //}

}







