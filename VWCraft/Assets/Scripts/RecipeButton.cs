using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeButton : MonoBehaviour {

    public Button buttonComponent;
    public Text label;
    public RecipeList recipeList;

    private Recipe recipe;

    private void Start()
    {
        buttonComponent = GetComponent<Button>();
        buttonComponent.onClick.AddListener(HandleClick);
        label = transform.GetChild(0).GetComponent<Text>();
        recipeList = transform.parent.parent.parent.GetComponent<RecipeList>();
    }

    public void Setup(Recipe currentRecipe, RecipeList currentRecipeList)
    {
        recipe = currentRecipe;
        label.text = currentRecipe.created;
        recipeList = currentRecipeList;
    }

    public void HandleClick()
    {
        recipeList.TrySetUp(recipe);
    }
}
