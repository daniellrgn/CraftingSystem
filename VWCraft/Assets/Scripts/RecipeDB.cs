using Mono.Data.Sqlite;
using System.Data;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RecipeDB : MonoBehaviour
{
    public string databaseName;
    private string RecipesAsJson;

    //gets specified recipe from database
    public T getRecipe<T>(int recipeID)
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
                string sqlQuery = "SELECT * FROM Recipe INNER JOIN RecipeInput on RecipeInput.ID = Recipe.ID INNER JOIN RecipeOutput on RecipeOutput = Recipe.ID WHERE ID = " + recipeID.ToString();
                dbcmd.CommandText = sqlQuery;
                IDataReader reader = dbcmd.ExecuteReader();

                reader.Read();
                string serializedRecipe = reader[2].ToString();
                T recipe = JsonUtility.FromJson<T>(serializedRecipe);

                //need to create whatever item subclass from the reader return values
                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;
                reader.Close();
                reader = null;

                return recipe;
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                return default(T);
            }
        }
        Debug.Log("No database name specified when retrieving recipe");
        return default(T);
    }

    //inserts recipe within database
    public void InsertRecipe(Recipe newRecipe, String database)
    {
        if(database == "")
        {
            database = databaseName;
        }
        if (database != "")
        {
            try {
                string conn = "URI=file:" + Application.dataPath + "/" + database; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();

                string recipeType = newRecipe.GetType().FullName;

                string serializedItem = JsonUtility.ToJson(newRecipe);


                string sql = "INSERT INTO Recipe (ID, type, SerialRecipe) VALUES (?,?,?)";
                dbcmd.CommandText = sql;
                var parameter = dbcmd.CreateParameter();
                parameter.ParameterName = "ID";
                parameter.Value = newRecipe.recipeID;
                dbcmd.Parameters.Add(parameter);

                parameter = dbcmd.CreateParameter();
                parameter.ParameterName = "type";
                parameter.Value = recipeType;
                dbcmd.Parameters.Add(parameter);

                parameter = dbcmd.CreateParameter();
                parameter.ParameterName = "SerialRecipe";
                parameter.Value = serializedItem;
                dbcmd.Parameters.Add(parameter);
                dbcmd.Prepare();
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

    }

    //retrieves recipe list
    public List<T> GetRecipeList<T>()
    {
        if (databaseName != "")
        {
            try
            {
                List<T> allRecipes = new List<T>();
                string conn = "URI=file:" + Application.dataPath + "/" + databaseName; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();
                string sqlQuery = "SELECT * FROM Recipe";
                dbcmd.CommandText = sqlQuery;
                IDataReader reader = dbcmd.ExecuteReader();

                while (reader.Read())
                {
                    string serializedRecipe = reader[2].ToString();
                    T recipe = JsonUtility.FromJson<T>(serializedRecipe);
                    if (recipe != null)
                    {
                        allRecipes.Add(recipe);
                    }
                }


                //need to create whatever item subclass from the reader return values
                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;
                reader.Close();
                reader = null;

                return allRecipes;
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                return default(List<T>);

            }
        }
        return default(List<T>);
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

    //removes recipe from database
    public int RemoveRecipe(Recipe recipe, String database)
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
            string sqlQuery = "DELETE FROM RECIPE WHERE ID = @id";
            dbcmd.CommandText = sqlQuery;

            var parameter = dbcmd.CreateParameter();
            parameter.ParameterName = "@id";
            parameter.Value = recipe.recipeID;
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

    public void SetDatabaseName(string name)
    {
        databaseName = name;
    }

    public string GetDatabaseName()
    {
        return databaseName;
    }
}
