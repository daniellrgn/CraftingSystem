using UnityEngine;
using System.Collections;

public class CreateItem : MonoBehaviour
{
    private ItemDB database = new ItemDB();
    public string dbID;
    public Item item;

    public void AddItemToItemDB()
    {
        database.AddItem(item);
    }

}
