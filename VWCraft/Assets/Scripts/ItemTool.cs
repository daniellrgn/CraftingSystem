using UnityEngine;

public class ItemTool : MonoBehaviour
{
    private ItemDB database = null; //new ItemDB();
    public string databaseName;
    public Item item;

    public void AddItemToItemDB()
    {
        database = new ItemDB();
        database.InsertItem(item, databaseName);
    }
    public void RemoveItemFromItemDB()
    {
        database = new ItemDB();
        database.RemoveItem(item, databaseName);
    }
}
