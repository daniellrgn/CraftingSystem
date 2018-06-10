using UnityEngine;
using System.Collections;

public class DestroyItem : MonoBehaviour
{
    private ItemDB database = new ItemDB();
    public string databaseName;
    public Item item;

    public void RemoveItemFromItemDB()
    {
        database.RemoveItem(item, databaseName);
    }

}
