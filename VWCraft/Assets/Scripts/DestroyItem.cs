using UnityEngine;
using System.Collections;

public class DestroyItem : MonoBehaviour
{
    private ItemDB database = new ItemDB();
    public string dbID;
    public Item item;

    public void RemoveItemFromItemDB()
    {
        if (dbID == "")
        {
            dbID = "VOID";
        }
        database.RemoveItemAsJson(dbID, item);
    }

}
