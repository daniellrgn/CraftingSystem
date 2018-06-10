using UnityEngine;

public class ItemTool : MonoBehaviour
{
    private GameObject itemDB;
    public string databaseName;
    public Item item;
    private ItemDB database;
    public void AddItemToItemDB()
    {
        database = itemDB.GetComponent<ItemDB>();
        database.InsertItem(item, databaseName);
    }
    public void RemoveItemFromItemDB()
    {
        database = itemDB.GetComponent<ItemDB>();
        database.RemoveItem(item, databaseName);
    }
}
