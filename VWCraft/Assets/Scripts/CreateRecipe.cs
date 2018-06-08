using UnityEngine;
using System.Collections;

public class CreateRecipe : MonoBehaviour
{
    private RecipeDB database = new RecipeDB();
    public string dbID;
    public Recipe recipe;

    public void AddRecipeToRecipeDB()
    {
        database.AddRecipe(recipe);
    }
}
