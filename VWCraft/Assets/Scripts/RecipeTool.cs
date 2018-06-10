using UnityEngine;
using System.Collections;

public class RecipeTool : MonoBehaviour
{
    private RecipeDB database = new RecipeDB();
    public string databaseName;
    public Recipe recipe;

    public void AddRecipeToRecipeDB()
    {
        database.InsertRecipe(recipe, databaseName);
    }

    public void RemoveRecipeFromRecipeDB()
    {
        database.RemoveRecipe(recipe, databaseName);
    }
}
