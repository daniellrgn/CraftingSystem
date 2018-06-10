using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ItemTool))]
public class ItemToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ItemTool myScript = (ItemTool)target;
        if (GUILayout.Button("Create Item"))
        {
            myScript.AddItemToItemDB();
        }
        if(GUILayout.Button("Destroy Item"))
        {
            myScript.RemoveItemFromItemDB();
        }
    }
}