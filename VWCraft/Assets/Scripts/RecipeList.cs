using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeList : MonoBehaviour {
    public GameObject button;
    public Transform contentPanel;
    public GameObject playerInventory;
    public GameObject craftingInput;


    private Inventory inventory;
    private Inventory input;
    private RecipeDB recipeDB;
    private List<CraftRecipe> recipeList;

    // Use this for initialization
    void Start () {
        contentPanel = transform.GetChild(0).GetChild(0).transform;
        recipeDB = GameObject.FindGameObjectWithTag("RecipeDB").GetComponent<RecipeDB>();
        recipeList = recipeDB.GetRecipeList<CraftRecipe>();
        inventory = playerInventory.GetComponent<Inventory>();
        input = craftingInput.GetComponent<Inventory>();
        AddButtons();
	}
	
	private void AddButtons()
    {
        for (int i = 0; i < recipeList.Count; i++)
        {
            Recipe recipe = recipeList[i];
            GameObject newButton = Instantiate(button);
            newButton.transform.SetParent(contentPanel);
            newButton.transform.localScale = new Vector3(1, 1, 1);
            RecipeButton recipeButton = newButton.GetComponent<RecipeButton>();
            recipeButton.Setup(recipe, this);
        }
    }

    public void TrySetUp(Recipe recipe)
    {
        int[] itemIDs = recipe.needed;
        for (int i = 0; i < itemIDs.Length; i++)
        {
            int num = inventory.CountItemByID(itemIDs[i]);
            if (itemIDs[i] >= 0 && num > 0)
            {
                if (input.TryAddItemAtIndex(itemIDs[i], i))
                {
                    inventory.RemoveItemByID(itemIDs[i]);
                }
            }
        }
    }
}
