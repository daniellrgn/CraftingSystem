using Mono.Data.Sqlite;
using System.Data;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class ItemDB : MonoBehaviour
{
    public string fileName;
    private string itemsAsJson;


    public string databaseName;
    /////////////////////////////////////////////////////////////////////////
    /// 
    /// 
    ///             Database Methods                                      
    /// 
    /// 
    /////////////////////////////////////////////////////////////////////////
    

    //Creates table with =in the named database string, or the database attached to this script if null.
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

    //insert item within specified item table in database, and a new item table if the user chooses
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


    //gets list of all items within the ITEM table
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
                    string serializedItem = reader[2].ToString();
                    T item = JsonUtility.FromJson<T>(serializedItem);
                    if (item != null)
                    {
                        allItems.Add(item);
                    }
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

    //removes item from database
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
}

