
[System.Serializable]
public class TableAttribute {
    public string attribute;  //table attributes
    public string dataType;   //datatype of each attribute
    public int maxLength;     //max len for text type attributes

    // Use this for initialization
    public TableAttribute(string attribute, string dataType, int maxLength)
    {
        this.attribute = attribute;
        this.dataType = dataType;
        this.maxLength = maxLength;
    }
    public TableAttribute(string attribute, string dataType)
    {
        this.attribute = attribute;
        this.dataType = dataType;
    }
}
