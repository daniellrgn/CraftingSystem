using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class RecipeDB : MonoBehaviour {

    public string fileName;

    private List<CraftRecipe> recipeData = new List<CraftRecipe>();
    private CraftRecipe[] RecipeArray;
    private string RecipesAsJson;

    // Use this for initialization
    void Start()
    {
        string filePath = Application.streamingAssetsPath + "/" + fileName;
        RecipesAsJson = File.ReadAllText(filePath);
        //RecipeArray = RecipeJsonHelper.FromJson<CraftRecipe>(RecipesAsJson);
        RecipeArray = new CraftRecipe[] {new CraftRecipe(0, "Magic Sword", 5, false, new int[]{0, 1}),
                                         new CraftRecipe(1, "Potion", 1, false, new int[]{2, 6}),
                                         new CraftRecipe(2, "Sword", 0, false, new int[]{3, 4}),
                                         new CraftRecipe(3, "GreatSword", 7, true, new int[]{-1, 0, -1, -1, 3, -1, -1, 4, -1}),
                                         new CraftRecipe(4, "Something NEw", 5, false, new int[]{1,2,3 })};
        LoadRDB();
    }

    private void LoadRDB()
    {
        CraftRecipe recipe;
        for (int i = 0; i < RecipeArray.Length; i++)
        {
            recipe = RecipeArray[i];
            //CraftRecipe addRecipe = new CraftRecipe(recipe.recipeID, recipe.created, recipe.itemID, recipe.shaped, recipe.needed);
            recipeData.Add(recipe);
        }
    }

    public Recipe GetRecipeByID(int ID)
    {
        for (int i = 0; i < recipeData.Count; i++)
        {
            if (recipeData[i].recipeID == ID)
            {
                return recipeData[i];
            }
        }
        return null;
    }

    public List<CraftRecipe> GetRecipeList()
    {
        return recipeData;
    }

    public int AddRecipeAsJson(string dbID, Recipe recipe)
    {
        Debug.Log(Application.streamingAssetsPath);
        string dbPath = Application.streamingAssetsPath + "/" + dbID + ".json";
        Debug.Log(dbPath);
        string curRecipesAsJson;
        string newRecipesAsJson;
        Recipe[] curRecipeArray;
        Recipe[] newRecipeArray;
        if (System.IO.File.Exists(dbPath))//append Recipe
        {
            Debug.Log("FILE EXISTS");
            try
            {
                curRecipesAsJson = File.ReadAllText(dbPath);
            }
            catch (Exception e)
            {
                Debug.Log("FAILED TO READ JSON");
                Debug.Log(e.ToString());
                return 1;
            }
            Debug.Log(curRecipesAsJson);
            curRecipeArray = RecipeJsonHelper.FromJson<Recipe>(curRecipesAsJson);
            for (int i = 0; i < curRecipeArray.Length; i++)
            {
                if (recipe.recipeID == curRecipeArray[i].recipeID)
                {
                    Recipe[] RecipeError = { recipe };
                    string errorMessage = "CANNOT CREATE RECIPE DUE TO ID CONFLICT @ ID=" + curRecipeArray[i].recipeID + " :\n"
                                        + "(" + dbID + ") CONTENTS:\n"
                                        + curRecipesAsJson + "\n"
                                        + "RECIPE ADDED:\n"
                                        + RecipeJsonHelper.ToJson<Recipe>(RecipeError, true);
                    Debug.Log(errorMessage);
                    return 1;
                }
            }
            newRecipeArray = new Recipe[curRecipeArray.Length + 1];
            System.Array.Copy(curRecipeArray, 0, newRecipeArray, 0, curRecipeArray.Length);
            newRecipeArray[curRecipeArray.Length] = recipe;
            newRecipesAsJson = RecipeJsonHelper.ToJson<Recipe>(newRecipeArray, true);
        }
        else//add first Recipe
        {
            newRecipeArray = new Recipe[1];
            newRecipeArray[0] = recipe;
            newRecipesAsJson = RecipeJsonHelper.ToJson<Recipe>(newRecipeArray, true);
        }
        Debug.Log(newRecipesAsJson);
        try
        {
            File.WriteAllText(dbPath, newRecipesAsJson);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return 1;
        }
        return 0;
    }

    public void AddRecipe(Recipe item)
    {

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

    override public int Evaluate(int[] input)
    {
        if (shaped && CheckItemsExact(input))
        {
            return itemID;
        }
        else if (!shaped && CheckItems(input))
        {
            return itemID;
        }
        else
        {
            return -1;
        }
    }
}

public static class RecipeJsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Recipes;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Recipes = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Recipes = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Recipes;
    }
}
