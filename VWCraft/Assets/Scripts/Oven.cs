using System.Collections.Generic;
using UnityEngine;

public class Oven : MonoBehaviour, ICraftStation<CraftRecipe> {
    public int temp;
    public GameObject ovenCraftInterface;
    public GameObject inventory;
    public Inventory Input { get; set; }
    public List<CraftRecipe> recipeList { get; set; }
    public List<GameObject> map = new List<GameObject>();

    private RecipeDB recipeDB;
    private List<GameObject> inputs = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        recipeDB = GameObject.FindGameObjectWithTag("RecipeDB").transform.GetComponent<RecipeDB>();
        Input = ovenCraftInterface.transform.Find("Input").GetComponent<Inventory>();
    }

    public void OnTriggerEnter(Collider other)
    {
        ItemData itemData = other.GetComponent<ItemData>();
        if (itemData)
        {
            Input.TryAddItemByID(itemData.item.ID, itemData.GetAmount());
            inputs.Add(other.gameObject);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        ItemData itemData = other.GetComponent<ItemData>();
        if (itemData)
        {
            Input.RemoveItemByID(itemData.item.ID, itemData.GetAmount());
            inputs.Remove(other.gameObject);
        }
    }

    private void OnMouseDown()
    {
        Craft();
    }

    public void Craft() {
        List<Item> inputItems = Input.items;
        List<CraftRecipe> recipeList = recipeDB.GetRecipeList<CraftRecipe>();

        int[] allItemIDs = new int[inputItems.Count];
        for (int i = 0; i < inputItems.Count; i++)
        {
            allItemIDs[i] = inputItems[i].ID;
        }

        for (int i = 0; i < recipeList.Count; i++)
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
        RemoveFromInput();
        GameObject outputObject = Instantiate(map[5]);
        Vector3 offset = new Vector3(1, 1, 1);
        outputObject.transform.position = transform.position + offset;
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
                    itemData.SetAmount(itemData.GetAmount()-1);
                    Destroy(inputs[0]);
                    inputs.RemoveAt(0);
                }
                else
                {
                    Input.items[i] = new Item();
                    Destroy(inputs[0]);
                    inputs.RemoveAt(0);
                    Destroy(itemData.gameObject);
                }
            }
        }
    }
}