using Mono.Data.Sqlite;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Reflection;

public class ItemDB : MonoBehaviour
{
    public string fileName;

    private List<Item> itemData = new List<Item>();
    private Item[] itemArray;
    private string itemsAsJson;


    public string databaseName;
    /////////////////////////////////////////////////////////////////////////
    /// 
    /// 
    ///             Database Methods                                      
    /// 
    /// 
    /////////////////////////////////////////////////////////////////////////
    public void CreateTable(String tableName, List<TableAttribute> tableAttributes, String database)
    {
        if (database == "")
        {
            database = databaseName;
        }
        if (databaseName != "")
        {
            try
            {
                string conn = "URI=file:" + Application.dataPath + "/" + database; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();

                string sqlQuery = "CREATE TABLE " + tableName + " (";
                foreach (TableAttribute entry in tableAttributes)
                {
                    sqlQuery += entry.attribute + " " + entry.dataType;
                    if (entry.maxLength != 0)
                    {
                        sqlQuery += "(" + entry.maxLength.ToString() + ")";
                    }
                    sqlQuery += ",";
                }
                sqlQuery = sqlQuery.Substring(0, sqlQuery.Length - 1);//remove last comma
                sqlQuery += ")";
                dbcmd.CommandText = sqlQuery;
                dbcmd.ExecuteNonQuery();

                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());

            }
        }
        else
        {
            Debug.Log("Database name not specified");
        }
    }

    public void InsertItem(Item newItem, String database, String newTableName = "")
    {
        if (database == "")
        {
            print(database);
            database = databaseName;
            print(database);

        }
        if (database != "")
        {
            try
            {
                string conn = "URI=file:" + Application.dataPath + "/" + database; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string itemType = newItem.GetType().FullName;
                string serializedItem = JsonUtility.ToJson(newItem);

                string sql = "INSERT INTO ITEM (ID, type, SerialItem) VALUES (?,?,?)";
                dbcmd.CommandText = sql;
                var parameter = dbcmd.CreateParameter();
                parameter.ParameterName = "ID";
                parameter.Value = newItem.ID;
                dbcmd.Parameters.Add(parameter);

                parameter = dbcmd.CreateParameter();
                parameter.ParameterName = "type";
                parameter.Value = itemType;
                dbcmd.Parameters.Add(parameter);

                parameter = dbcmd.CreateParameter();
                parameter.ParameterName = "SerialObject";
                parameter.Value = serializedItem;
                dbcmd.Parameters.Add(parameter);
                dbcmd.Prepare();
                dbcmd.ExecuteNonQuery();
                if (newTableName != "")
                {
                    string sqlQuery = "INSERT INTO " + newTableName + "(";

                    var fields = newItem.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
                    foreach (var prop in fields)
                    {
                        sqlQuery += prop.Name + ",";
                    }
                    sqlQuery = sqlQuery.Substring(0, sqlQuery.Length - 1);
                    sqlQuery += ") VALUES (";
                    foreach (var prop in fields)
                    {
                        sqlQuery += "?,";
                    }
                    sqlQuery = sqlQuery.Substring(0, sqlQuery.Length - 1);
                    sqlQuery += ")";

                    dbcmd.CommandText = sqlQuery;
                    foreach (var prop in fields)
                    {
                        parameter = dbcmd.CreateParameter();
                        parameter.ParameterName = prop.Name;
                        parameter.Value = prop.GetValue(newItem);
                        dbcmd.Parameters.Add(parameter);
                    }
                    dbcmd.CommandText = sqlQuery;
                    dbcmd.ExecuteNonQuery();

                }

                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
        else
        {
            print("Database name not specified");
        }
    }

    public T GetItem<T>(int id, String joinTable = "") //get table name
    {
        if (databaseName != "")
        {
            try
            {
                string conn = "URI=file:" + Application.dataPath + "/" + databaseName; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery;
                if (joinTable != "")
                {
                    sqlQuery = "SELECT * FROM Item INNER JOIN " + joinTable + " on " + joinTable + ".ID = Item.ID  WHERE ID = " + id.ToString();
                }
                else
                {
                    sqlQuery = "SELECT * FROM ITEM WHERE ID = " + id.ToString();
                }
                dbcmd.CommandText = sqlQuery;
                IDataReader reader = dbcmd.ExecuteReader();

                reader.Read();
                string itemTypeString = reader[1].ToString();
                string serializedItem = reader[2].ToString();

                T item = JsonUtility.FromJson<T>(serializedItem);

                //need to create whatever item subclass from the reader return values
                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;
                reader.Close();
                reader = null;
                return item;
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                return default(T);

            }
        }
        else
        {
            print("Database name not specified");
            return default(T);
        }
    }



    public List<T> GetItemList<T>()
    {
        if (databaseName != "")
        {
            try
            {
                List<T> allItems = new List<T>();
                string conn = "URI=file:" + Application.dataPath + "/" + databaseName; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery = "SELECT * FROM ITEM";
                dbcmd.CommandText = sqlQuery;
                IDataReader reader = dbcmd.ExecuteReader();

                while (reader.Read())
                {
                    string itemTypeString = reader[1].ToString();
                    string serializedItem = reader[2].ToString();
                    T item = JsonUtility.FromJson<T>(serializedItem);
                    allItems.Add(item);
                }


                //need to create whatever item subclass from the reader return values
                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;
                reader.Close();
                reader = null;

                return allItems;
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                return default(List<T>);

            }
        }
        else
        {
            print("Database name not specified");
            return default(List<T>);
        }
    }

    public int RemoveItem(Item item, String database)
    {
        if (database == "")
        {
            database = databaseName;
        }
        try
        {
            string conn = "URI=file:" + Application.dataPath + "/" + database; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "DELETE FROM ITEM WHERE ID = @id";
            dbcmd.CommandText = sqlQuery;

            var parameter = dbcmd.CreateParameter();
            parameter.ParameterName = "@id";
            parameter.Value = item.ID;
            dbcmd.Parameters.Add(parameter);
            dbcmd.Prepare();
            dbcmd.ExecuteNonQuery();

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            return 0;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return -1;
        }
    }

    private Boolean TableExists(string tableName)
    {
        if (databaseName != "")
        {
            string conn = "URI=file:" + Application.dataPath + "/" + databaseName; //Path to database.
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(conn);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'NAME'";
            dbcmd.CommandText = sqlQuery;
            var parameter = dbcmd.CreateParameter();
            parameter.ParameterName = "NAME";
            parameter.Value = tableName;
            dbcmd.Parameters.Add(parameter);
            dbcmd.Prepare();
            int queryResult = dbcmd.ExecuteNonQuery();
            if (queryResult == 0)
            {
                return false;
            }
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

        }
        else
        {
            return false;
        }
        return true;
    }

    public void SetDatabaseName(string name)
    {
        databaseName = name;
    }

    public string GetDatabaseName()
    {
        return databaseName;
    }

    // Use this for initialization
    void Start()
    {

        if (databaseName != "")
        {
            itemArray = GetItemList<Item>().ToArray();
            LoadIDB();
        }
    }

    private void LoadIDB()
    {
        Item it;
        for (int i = 0; i < itemArray.Length; i++)
        {
            it = itemArray[i];
            Item addIt = new Item(it.ID, it.Title, it.Value, it.Stackable, it.Description, it.ImgPath);
            itemData.Add(addIt);
        }
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
    private Sprite Sprite;

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

    public Item()
    {
        ID = -1;

    }

    public Sprite GetSprite()
    {
        return Sprite;
    }

    public void SetSprite(Sprite newSprite)
    {
        Sprite = newSprite;
    }
}