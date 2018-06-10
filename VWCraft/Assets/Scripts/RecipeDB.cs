using Mono.Data.Sqlite;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Reflection;

public class RecipeDB : MonoBehaviour
{

    public string fileName;
    public string databaseName;

    private List<CraftRecipe> recipeData = new List<CraftRecipe>();
    private CraftRecipe[] RecipeArray;
    private string RecipesAsJson;



    // Use this for initialization
    void Start()
    {
        //string filePath = Application.streamingAssetsPath + "/" + fileName;
        //RecipesAsJson = File.ReadAllText(filePath);
        //RecipeArray = RecipeJsonHelper.FromJson<CraftRecipe>(RecipesAsJson);
        RecipeArray = new CraftRecipe[] {new CraftRecipe(0, "Magic Sword", 5, false, new int[]{0, 1}),
                                         new CraftRecipe(1, "Potion", 1, false, new int[]{2, 6}),
                                         new CraftRecipe(2, "Sword", 0, false, new int[]{3, 4}),
                                         new CraftRecipe(3, "GreatSword", 7, true, new int[]{-1, 0, -1, -1, 3, -1, -1, 4, -1}),
                                         new CraftRecipe(4, "Something NEw", 5, false, new int[]{1,2,3 })};
        LoadRDB();
    }

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
                string recipeTypeString = reader[1].ToString();
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
                    string itemTypeString = reader[1].ToString();
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

    private void LoadRDB()
    {
        CraftRecipe recipe;

        foreach (CraftRecipe recipeItem in GetRecipeList<CraftRecipe>())
        {
            recipeData.Add(recipeItem);
        }
    }
}

[System.Serializable]
public class CraftRecipe : Recipe
{
    public bool shaped;

    public CraftRecipe(int recipeID, string created, int itemID, bool shaped, int[] needed) : base(recipeID, created, itemID, needed)
    {
        this.shaped = shaped;
    }

    public override int Evaluate(int[] input)
    {
        if (shaped && base.CheckItemsExact(input))
        {
            return base.itemID;
        }
        else if (!shaped && base.CheckItems(input))
        {
            return base.itemID;
        }
        else
        {
            return -1;
        }
    }
}
