using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TerrainGenerator tGen = (TerrainGenerator)target;

        if (DrawDefaultInspector()) {
            if (tGen.autoUpdate) {
                tGen.GenerateMap();
            }
        }

        if (GUILayout.Button("Generate")) {
            tGen.GenerateMap();
        }
    }
}
