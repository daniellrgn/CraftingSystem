using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootInventory : Inventory {

    GameObject lootPanel;
    GameObject lootSlotPanel;

    public ItemDB idb;

    private GameObject _player;


    // Use this for initialization
    void Start () {
        lootPanel = GameObject.Find("Loot Panel");
        lootSlotPanel = lootPanel.transform.Find("Slot Panel").gameObject;

        for (int i = 0; i < numSlots; i++)
        {
            items.Add(new Item());
            slots.Add(Instantiate(inventorySlot));
            slots[i].transform.SetParent(lootSlotPanel.transform, false);
            slots[i].GetComponent<Slot>().slotNum = i;
        }
    }
	

    public void AddItem(int ID)
    {
        Item itemToAdd = idb.GetItem<Item>(ID);
        int index = AtIndex(itemToAdd);
        if (index > -1)//item exists in inventory
        {
            if (itemToAdd.Stackable)
            {
                ItemData iData = slots[index].GetComponent<Slot>().GetItemData();
                iData.amount++;
                iData.transform.GetChild(0).GetComponent<Text>().text = iData.amount.ToString();
            }
        }
        else//item does not exist in inventory
        {
            for (int i = 0; i < items.Count; i++)//find first open slot for the item
            {
                if (items[i].ID == -1)
                {
                    items[i] = itemToAdd;
                    GameObject itemObj = Instantiate(inventoryItem);
                    ItemData newData = itemObj.GetComponent<ItemData>();
                    newData.item = itemToAdd;
                    newData.slot = i;
                    newData.inv = this;
                    newData.SetAmount(1);
                    itemObj.transform.SetParent(slots[i].transform, false);
                    itemObj.transform.position = itemObj.transform.parent.position;
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
                        newData.amount = 1;
                    }
                    break;
                }
            }
        }
    }


    public int AtIndex(Item item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].ID == item.ID)
            {
                return i;
            }
        }
        return -1;
    }

    public void toggleActive()
    {
        lootPanel.SetActive(!lootPanel.activeSelf);
    }

    public bool getActive()
    {
        return lootPanel.activeSelf;
    }

    public GameObject getLootSlotPanel()
    {
        return lootSlotPanel;
    }
}
