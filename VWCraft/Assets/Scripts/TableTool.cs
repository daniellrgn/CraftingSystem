using UnityEngine;
using System.Collections.Generic;

public class TableTool : MonoBehaviour
{
    private ItemDB database = null;//new ItemDB();
    public string databaseName;
    public string tableName;
    public List<TableAttribute> tableAttributes = new List<TableAttribute>();

    public void AddTableToDatabase()
    {
        database = new ItemDB();

        database.CreateTable(tableName, tableAttributes, databaseName);
    }
}
