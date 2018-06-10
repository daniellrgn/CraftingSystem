
[System.Serializable]
public class CraftRecipe : Recipe
{
    public bool shaped;

    public CraftRecipe(int recipeID, string created, int itemID, bool shaped, int[] needed) : base(recipeID, created, itemID, needed)
    {
        this.shaped = shaped;
    }

    public override int Evaluate(int[] input)
    {
        if (shaped && CheckItemsExact(input))
        {
            return itemID;
        }
        else if (!shaped && CheckItems(input))
        {
            return itemID;
        }
        else
        {
            return -1;
        }
    }
}