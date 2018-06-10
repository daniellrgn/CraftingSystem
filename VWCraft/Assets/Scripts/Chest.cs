using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour {

    public GameObject lootInventory;
    public List<Item> items = new List<Item>();
    public float minDistance = 5.0f;

    private GameObject _player;
    private ItemDB idb;

    public void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        //lootInventory = GameObject.Find("LootInventory");
        idb = GameObject.FindGameObjectWithTag("ItemDB").GetComponent<ItemDB>();
        AddItem(0);
        AddItem(1);
        AddItem(3);

    }


    public IEnumerator OnMouseOver()
    {
        if (Input.GetButtonDown("Use"))
        {
            Vector3 objPos = gameObject.transform.position;
            Vector3 playerPos = _player.transform.position;
            if (Vector3.Distance(objPos, playerPos) < minDistance && lootInventory.activeInHierarchy == false)
            {
                lootInventory.SetActive(true);
                yield return new WaitForSeconds(0.05f);
                GameObject.FindGameObjectWithTag("lootUITitle").GetComponent<Text>().text = gameObject.name;
                foreach (Item item in items)
                {
                    lootInventory.GetComponent<LootInventory>().AddItem(item.ID);
                }
            }
        }
        else if (Input.GetButtonDown("VendorDebug") && lootInventory.activeInHierarchy == false)
        {
            lootInventory.SetActive(true);
            List<Item> allItems = idb.GetItemList<Item>();
            yield return new WaitForSeconds(0.2f);
            GameObject.FindGameObjectWithTag("lootUITitle").GetComponent<Text>().text = gameObject.name;


            foreach (Item possibleItem in allItems)
            {
                if (possibleItem.Stackable)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        lootInventory.GetComponent<LootInventory>().AddItem(possibleItem.ID);
                    }
                }
                else
                {
                    lootInventory.GetComponent<LootInventory>().AddItem(possibleItem.ID);
                }
            }
        }
    }

    public void AddItem(int ID)
    {
        Item newItem = idb.GetItem<Item>(ID);
        items.Add(newItem);
    }
}
