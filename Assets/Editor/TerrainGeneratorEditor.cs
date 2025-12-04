using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TerrainGenerator terrainGenerator = (TerrainGenerator)target;

        DrawDefaultInspector();

        if (terrainGenerator.objectPlacementData != null)
        {
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Preview Object Placement"))
            {
                terrainGenerator.PreviewObjectPlacement();
            }

            if (GUILayout.Button("Clear Preview Objects"))
            {
                ClearObjectsByName("Preview Objects");
            }

            if (GUILayout.Button("Place Objects on Spawn Chunks"))
            {
                terrainGenerator.PlaceObjectsOnSpawnChunks();
            }

            if (GUILayout.Button("Place Objects on Active Chunks"))
            {
                terrainGenerator.PlaceObjectsOnActiveChunks();
            }

            if (GUILayout.Button("Clear All Placed Objects"))
            {
                terrainGenerator.ClearAllPlacedObjects();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Assign an ObjectPlacementData asset to enable placement tools.", MessageType.Warning);
        }
    }

    private void ClearObjectsByName(string objectName)
    {
        Transform parent = GameObject.Find(objectName)?.transform;
        if (parent != null)
        {
            DestroyImmediate(parent.gameObject);
        }
    }
}