using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateLevelChunkData))]
public class LevelChunkEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GenerateLevelChunkData script = (GenerateLevelChunkData)target;

        if (GUILayout.Button("Generate Level Data"))
        {
            script.GetLevelData(); // Call the function from GenerateLevelChunkData script
        }
    }
}
