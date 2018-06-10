using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TableTool))]
public class TableToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TableTool myScript = (TableTool)target;
        if (GUILayout.Button("Create Table"))
        {
            myScript.AddTableToDatabase();
        }
    }
}