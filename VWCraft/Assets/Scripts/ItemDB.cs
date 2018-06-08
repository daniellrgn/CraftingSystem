using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ItemDB : MonoBehaviour {
    public string fileName;

    private List<Item> itemData = new List<Item>();
    private Item[] itemArray;
    private string itemsAsJson;

    // Use this for initialization
    void Start()
    {
        if (fileName  != "")
        {
            string filePath = Application.streamingAssetsPath+ "/" + fileName;
            itemsAsJson = File.ReadAllText(filePath);
            itemArray = ItemJsonHelper.FromJson<Item>(itemsAsJson);
            LoadIDB();
        }
    }

    private void LoadIDB()
    {
        Item it;
        for (int i = 0; i < itemArray.Length; i++){
            it = itemArray[i];
            Item addIt = new Item(it.ID, it.Title, it.Value, it.Stackable, it.Description, it.ImgPath);
            itemData.Add(addIt);
        }
    }

    public Item GetItemByID(int ID)
    {
        for (int i = 0; i < itemData.Count; i++)
        {
            if (itemData[i].ID == ID)
            {
                return itemData[i];
            }
        }
            return null;
    }

    public List<Item> GetItemList()
    {
        return itemData;
    }

    public void AddItem(Item item)
    {

    }

    public int AddItemAsJson(string dbID, Item item)
    {
        Debug.Log(Application.streamingAssetsPath);
        string dbPath = Application.streamingAssetsPath + "/" + dbID + ".json";
        Debug.Log(dbPath);
        string curItemsAsJson;
        string newItemsAsJson;
        Item[] curItemArray;
        Item[] newItemArray;
        if (System.IO.File.Exists(dbPath))//append item
        {
            Debug.Log("FILE EXISTS");
            try
            {
                curItemsAsJson = File.ReadAllText(dbPath);
            }catch(Exception e)
            {
                Debug.Log("FAILED TO READ JSON");
                Debug.Log(e.ToString());
                return 1;
            }
            Debug.Log(curItemsAsJson);
            curItemArray = ItemJsonHelper.FromJson<Item>(curItemsAsJson);
            for (int i = 0; i < curItemArray.Length; i++)
            {
                if (item.ID == curItemArray[i].ID)
                {
                    Item[] itemError = { item };
                    string errorMessage = "CANNOT CREATE ITEM DUE TO ID CONFLICT @ ID="+curItemArray[i].ID+" :\n"
                                        + "(" + dbID + ") CONTENTS:\n"
                                        + curItemsAsJson + "\n"
                                        + "ITEM ADDED:\n"
                                        + ItemJsonHelper.ToJson<Item>(itemError, true);
                    Debug.Log(errorMessage);
                    return 1;
                }
            }
            newItemArray = new Item[curItemArray.Length + 1];
            System.Array.Copy(curItemArray, 0, newItemArray, 0, curItemArray.Length);
            newItemArray[curItemArray.Length] = item;
            newItemsAsJson = ItemJsonHelper.ToJson<Item>(newItemArray, true);
        }
        else//add first item
        {
            newItemArray = new Item[1];
            newItemArray[0] = item;
            newItemsAsJson = ItemJsonHelper.ToJson<Item>(newItemArray, true);
        }
        Debug.Log(newItemsAsJson);
        try
        {
            File.WriteAllText(dbPath, newItemsAsJson);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return 1;
        }
        return 0;
    }
}

[System.Serializable]
public class Item
{
    public int ID;
    public string Title;
    public int Value;
    public bool Stackable;
    public string Description;
    public string ImgPath;
    public Sprite Sprite;

    public Item(int id, string title, int value, bool stackable, string description, string imgPath)
    {
        ID = id;
        Title = title;
        Value = value;
        Stackable = stackable;
        Description = description;
        ImgPath = imgPath;
        Sprite = Resources.Load<Sprite>("Sprites/Items/" + imgPath);
    }

    public Item(int id, string title, int value, bool stackable, string description, Sprite sprite)
    {
        ID = id;
        Title = title;
        Value = value;
        Stackable = stackable;
        Description = description;
        Sprite = sprite;
    }

    public Item()
    {
        ID = -1;
    }
}

public static class ItemJsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
