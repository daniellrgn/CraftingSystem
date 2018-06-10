using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class TableAttribute {
    public string attribute;
    public string dataType;
    public int maxLength;

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
