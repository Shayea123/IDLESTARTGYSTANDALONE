using IdleStrategyKit;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LoadAndSave), true)]
public class LoadAndSaveEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LoadAndSave loadAndSave = (LoadAndSave)target;
        if (GUILayout.Button("Delete Saves"))
        {
            loadAndSave.ResetData();
        }

        if (GUILayout.Button("Open Saves Path"))
        {
            loadAndSave.OpenSavePath();
        }
    }
}