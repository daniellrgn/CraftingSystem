using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Oven : MonoBehaviour, ICraftStation {
    public int temp;
    public GameObject ovenCraftInterface;
    public GameObject inventory;
    public Inventory Input { get; set; }
    public Inventory Output { get; set; }
    public List<GameObject> map = new List<GameObject>();

    private ItemDB itemDB;
    private RecipeDB recipeDB;
    private List<GameObject> inputs = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        itemDB = GameObject.FindGameObjectWithTag("ItemDB").transform.GetComponent<ItemDB>();
        recipeDB = GameObject.FindGameObjectWithTag("RecipeDB").transform.GetComponent<RecipeDB>();
        Input = ovenCraftInterface.transform.Find("Input").GetComponent<Inventory>();
        Output = ovenCraftInterface.transform.Find("Output").GetComponent<Inventory>();
    }

    public void OnTriggerEnter(Collider other)
    {
        ItemData itemData = other.GetComponent<ItemData>();
        if (itemData)
        {
            Input.AddItemByID(itemData.item.ID, itemData.GetAmount());
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
        List<CraftRecipe> recipeList = recipeDB.GetRecipeList();

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
