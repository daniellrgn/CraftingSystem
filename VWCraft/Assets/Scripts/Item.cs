using UnityEngine;

[System.Serializable]
public class Item
{
    public int ID;
    public string Title;
    public int Value;
    public bool Stackable;
    public string Description;
    public string ImgPath;
    private Sprite Sprite;

    public Item(int id, string title, int value, bool stackable, string description, string imgPath)
    {
        ID = id;
        Title = title;
        Value = value;
        Stackable = stackable;
        Description = description;
        ImgPath = imgPath;
        Sprite = Resources.Load<Sprite>("Sprites/Items/" + imgPath);
    }

    public Item()
    {
        ID = -1;

    }

    public Sprite GetSprite()
    {
        return Sprite;
    }

    public void SetSprite(Sprite newSprite)
    {
        Sprite = newSprite;
    }
}
