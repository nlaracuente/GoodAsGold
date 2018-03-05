using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    MapGenerator m_mapGenerator;

    #if (UNITY_EDITOR)
    void OnEnable()
    {
        m_mapGenerator = (MapGenerator)target;
    }

    /// <summary>
    /// Triggers the coins to spawn around this object
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate")) {
            m_mapGenerator.GenerateMap(true);
        }
        //} else if (GUILayout.Button("Update")) {
        //    m_mapGenerator.GenerateMap();
        //}
    }
    #endif
}

