using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftButton : MonoBehaviour, ICraftStation<CraftRecipe> {
    public Button button;
    public List<CraftRecipe> recipeList { get; set; }
    public Inventory Input { get; set; }
    public Inventory Output { get; set; }

    ItemDB itemDB;
    RecipeDB recipeDB;

    private void Start()
    {
        button.onClick.AddListener(Craft);
        itemDB = GameObject.FindGameObjectWithTag("ItemDB").transform.GetComponent<ItemDB>();
        recipeDB = transform.parent.GetChild(0).GetComponent<RecipeDB>();
        recipeList = recipeDB.GetRecipeList<CraftRecipe>();
        Input = transform.parent.GetChild(2).GetComponent<Inventory>();
        Output = transform.parent.GetChild(3).GetComponent<Inventory>();
    }

    public void Craft()
    {
        List<Item> inputItems = Input.items;
        int[] allItemIDs = new int[inputItems.Count];
        for (int i = 0; i < inputItems.Count; i++)
        {
            allItemIDs[i] = inputItems[i].ID;
        }

        for(int i = 0; i < recipeList.Count; i++)
        {
            int result = recipeList[i].Evaluate(allItemIDs);
            if (result >= 0)
            {
                TryCraftingItem(result);
            }
        }
    }

    public void TryCraftingItem(int itemID)
    {
        if (Output.items[0] != null && (Output.items[0].ID == -1 || Output.items[0].Stackable))
        {
            RemoveFromInput();
        }
        Output.TryAddItemByID(itemID, dB:itemDB);
    }

    private void RemoveFromInput()
    {
        for (int i = 0; i < Input.items.Count; i++)
        {
            if (Input.items[i].ID >= 0)
            {
                ItemData itemData = Input.slots[i].transform.GetChild(1).GetComponent<ItemData>();
                if (Input.items[i].Stackable && itemData.amount > 1)
                {
                    itemData.amount--;
                    itemData.UpdateAmountDisplay();
                }
                else
                {
                    Input.items[i] = new Item();
                    Destroy(itemData.gameObject);
                }
            }
        }
    }
}
