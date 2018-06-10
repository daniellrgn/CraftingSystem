using UnityEngine;

public class RecipeTool : MonoBehaviour
{
    public GameObject recipeDB;
    public string databaseName;
    public Recipe recipe;
    private RecipeDB database;

    public void AddRecipeToRecipeDB()
    {
        database = recipeDB.GetComponent<RecipeDB>();
        database.InsertRecipe(recipe, databaseName);
    }

    public void RemoveRecipeFromRecipeDB()
    {
        database = recipeDB.GetComponent<RecipeDB>();
        database.RemoveRecipe(recipe, databaseName);
    }
}
