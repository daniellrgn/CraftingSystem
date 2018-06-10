using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

    public List<Item> items = new List<Item>();
    public List<GameObject> slots = new List<GameObject>();
    public GameObject inventorySlot;
    public GameObject inventoryItem;
    public int numSlots;
    public bool isOutput;

    GameObject inventoryPanel;
    GameObject slotPanel;
    ItemDB itemDB;
    ItemDB playerItems;

    private void Start()
    {
        itemDB = GameObject.FindGameObjectWithTag("ItemDB").GetComponent<ItemDB>();
        inventoryPanel = transform.GetChild(0).gameObject;
        slotPanel = inventoryPanel.transform.GetChild(0).gameObject;

        for (int i = 0; i < numSlots; i++)
        {
            items.Add(new Item());
            slots.Add(Instantiate(inventorySlot));
            slots[i].GetComponent<Slot>().slotNum = i;
            slots[i].transform.SetParent(slotPanel.transform, false);
        }

        // Adds items to inventory for testing.
        playerItems = GetComponent<ItemDB>();
        if (playerItems.fileName != "")
        {
            List<Item> playerItemList = itemDB.GetItemList<Item>();
            for (int i = 0; i < playerItemList.Count; i++)
            {
                TryAddItemByID(playerItemList[i].ID, 10, playerItems);
            }           
        }
        //
    }

    // Add an item to the first available inventory UI slot, or increment the stack.
    public bool TryAddItemByID(int ID, int amount = 1, ItemDB dB = null)
    {
        if (dB == null)
        {
            dB = itemDB;
        }
        Item itemToAdd = dB.GetItem<Item>(ID);
        int idx = GetItemIndexByID(ID);
        bool success = false;

        if (itemToAdd.Stackable && idx >= 0)
        {
            ItemData existData = GetSlotByIndex(idx).GetItemData();
            existData.SetAmount(existData.amount + amount);
            success = true;
        }
        else
        {
            // Create and add a new Item object
            for (int i = 0; i < items.Count && amount > 0; i++)
            {
                if (items[i].ID <= -1)
                {
                    success = true;
                    items[i] = itemToAdd;
                    GameObject itemObj = Instantiate(inventoryItem);
                    ItemData newData = itemObj.GetComponent<ItemData>();
                    newData.item = itemToAdd;
                    newData.slot = i;
                    newData.inv = this;
                    newData.SetAmount(1);
                    itemObj.transform.SetParent(slots[i].transform, false);
                    itemObj.transform.position = itemObj.transform.parent.position; // Set object to slot position
                    if (itemToAdd.GetSprite() == null)
                    {
                        itemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/" + itemToAdd.ImgPath);
                    }
                    else
                    {
                        itemObj.GetComponent<Image>().sprite = itemToAdd.GetSprite();
                    }
                    if (itemToAdd.Stackable)
                    {
                        newData.SetAmount(amount);
                        break;
                    }
                    amount--;
                }
            }
        }
        return success;
    }

    public bool TryAddItemAtIndex(int ID, int index, int amount = 1, ItemDB dB = null)
    {
        if (dB == null)
        {
            dB = itemDB;
        }

        bool success = false;

        if (index >= 0 && index < slots.Count && amount > 0)
        {
            Item itemToAdd = dB.GetItem<Item>(ID);
            ItemData existData = GetSlotByIndex(index).GetItemData();
            if (existData)
            {
                if (itemToAdd.Stackable && itemToAdd.ID == existData.item.ID)
                {
                    existData.SetAmount(existData.amount + amount);
                    success = true;
                }
            }
            else
            {
                // Create and add a new Item object
                if (items[index].ID == -1)
                {
                    items[index] = itemToAdd;
                    GameObject itemObj = Instantiate(inventoryItem);
                    ItemData newData = itemObj.GetComponent<ItemData>();
                    newData.item = itemToAdd;
                    newData.slot = index;
                    newData.inv = this;
                    newData.SetAmount(1);
                    itemObj.transform.SetParent(slots[index].transform, false);
                    itemObj.transform.position = itemObj.transform.parent.position; // Set object to slot position
                    if (itemToAdd.GetSprite() == null)
                    {
                        itemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/" + itemToAdd.ImgPath);
                    }
                    else
                    {
                        itemObj.GetComponent<Image>().sprite = itemToAdd.GetSprite();
                    }

                    if (amount == 1 || itemToAdd.Stackable)
                    {
                        newData.SetAmount(amount);
                        success = true;
                    }
                }
            }
        }
        return success;
    }

    public int RemoveItemAtIndex(int index, int amountToRemove = 1)
    {
        Slot slot = GetSlotByIndex(index);
        ItemData itemData = slot.GetItemData();
        int currentAmount = itemData.amount;
        if (amountToRemove >= 0 && amountToRemove < currentAmount)
        {
            itemData.SetAmount(currentAmount - amountToRemove);
            return amountToRemove;
        }
        else if (amountToRemove == currentAmount)
        {

            Destroy(slot.transform.GetChild(1).gameObject);
            slot.RemoveItem();
            return currentAmount;
        }
        else
        {
            return 0;
        }
    }

    public int RemoveItemByID(int ID, int amountToRemove = 1)
    {
        int currentAmount = CountItemByID(ID);
        if (amountToRemove >=0 && amountToRemove <= currentAmount)
        {
            int index = GetItemIndexByID(ID);
            while(index >= 0 && amountToRemove > 0)
            {
                int amount = Mathf.Min(amountToRemove, GetSlotByIndex(index).GetItemData().GetAmount());
                RemoveItemAtIndex(index, amount);
                amountToRemove -= amount;
                index = GetItemIndexByID(ID);
            }
            return amountToRemove;
        }
        else
        {
            return 0;
        }

    }

    public int CountItemByID(int ID)
    {
        int total = 0;
        for (int i = 0; i < slots.Count; i++)
        {
            ItemData itemData = GetSlotByIndex(i).GetItemData();
            if (itemData && itemData.item.ID == ID)
            {
                total += itemData.GetAmount();
            }
        }
        return total;
    }

    public Slot GetSlotByIndex(int index)
    {
        return slots[index].GetComponent<Slot>();
        
    }

    // Finds the first index of an Item and returns its index if found, or -1 on failure.

    public int GetItemIndexByID (int ID)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].ID == ID)
            {
                return i;
            }
        }
        return -1;
    }
}
