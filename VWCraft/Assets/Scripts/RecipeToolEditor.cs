using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RecipeTool))]
public class CreateRecipeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RecipeTool myScript = (RecipeTool)target;
        if (GUILayout.Button("Create Recipe"))
        {
            myScript.AddRecipeToRecipeDB();
        }

        if (GUILayout.Button("Destroy Recipe"))
        {
            myScript.RemoveRecipeFromRecipeDB();
        }
    }
}