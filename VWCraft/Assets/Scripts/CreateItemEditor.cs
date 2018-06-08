using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CreateItem))]
public class CreateItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CreateItem myScript = (CreateItem)target;
        if (GUILayout.Button("Create Item"))
        {
            myScript.AddItemToItemDB();
        }
    }
}