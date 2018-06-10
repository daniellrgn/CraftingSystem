using UnityEngine;
using System.Collections;

public class RecipeTool : MonoBehaviour
{
    private RecipeDB database = new RecipeDB();
    public string dbID;
    public Recipe recipe;

    public void AddRecipeToRecipeDB()
    {
        if(dbID == "")
        {
            dbID = "VOID";
        }
        database.AddRecipeAsJson(dbID, recipe);
    }

    public void RemoveRecipeFromRecipeDB()
    {
        if (dbID == "")
        {
            dbID = "VOID";
        }
        database.RemoveRecipeAsJson(dbID, recipe);
    }
}
