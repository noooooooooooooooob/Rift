using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileMapCreator))]
public class TileCreator : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TileMapCreator generator = (TileMapCreator)target;

        if (GUILayout.Button("Generate Map"))
        {
            generator.GenerateMap();
        }
        if (GUILayout.Button("Clear Map"))
        {
            generator.ClearMap();
        }
    }
}
