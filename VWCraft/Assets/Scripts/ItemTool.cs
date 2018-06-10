using UnityEngine;
using System.Collections;

public class ItemTool : MonoBehaviour
{
    private ItemDB database = new ItemDB();
    public string dbID;
    public Item item;

    public void AddItemToItemDB()
    {
        if(dbID == "")
        {
            dbID = "VOID";
        }
        database.AddItemAsJson(dbID, item);
    }
    public void RemoveItemFromItemDB()
    {
        if (dbID == "")
        {
            dbID = "VOID";
        }
        database.RemoveItemAsJson(dbID, item);
    }
}
