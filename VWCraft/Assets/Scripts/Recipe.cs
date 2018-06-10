using System.Collections.Generic;

[System.Serializable]
public class Recipe {
    public int recipeID;
    public string created;
    public int itemID;
    public int[] needed;

    public Recipe(int recipeID, string created, int itemID, int[] needed)
    {
        this.recipeID = recipeID;
        this.created = created;
        this.itemID = itemID;
        this.needed = needed;
    }

    public bool CheckItems(int[] input)
    {
        List<int> items = new List<int>();

        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] >= 0)
            {
                items.Add(input[i]);
            }
        }

        if (items.Count != needed.Length)
        {
            return false;
        }

        for (int i = 0; i < needed.Length; i++)
        {
            if (!items.Contains(needed[i]))
            {
                return false;
            }
        }

        return true;
    }

    public bool CheckItemsExact(int[] input)
    {
        int i = 0;
        int n = 0;
        
        // Find first item instance in each array
        while(n < needed.Length && needed[n] < 0)
        {
            n++;
        }
        while(i < input.Length && input[i] < 0)
        {
            i++;
        }

        while (n < needed.Length && i < input.Length)
        {
            if (needed[n] != input[i])
            {
                return false;
            }
            n++;
            i++;
        }

        int c = 0;
        int[] check;
        if (n < needed.Length)
        {
            c = n;
            check = needed;
        }
        else
        {
            c = i;
            check = input;
        }

        while (c < check.Length)
        {
            if (check[c] >= 0)
            {
                return false;
            }
            c++;
        }
        return true;
    }

    virtual public int Evaluate(int[] input)
    {
        if (CheckItems(input))
        {
            return itemID;
        }
        else
        {
            return -1;
        }
    }
}
