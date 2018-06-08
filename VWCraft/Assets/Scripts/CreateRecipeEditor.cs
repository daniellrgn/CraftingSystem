using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CreateRecipe))]
public class CreateRecipeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CreateRecipe myScript = (CreateRecipe)target;
        if (GUILayout.Button("Create Recipe"))
        {
            myScript.AddRecipeToRecipeDB();
        }
    }
}