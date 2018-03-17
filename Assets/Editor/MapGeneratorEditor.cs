using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapController))]
public class MapGeneratorEditor : Editor
{
    MapController m_mapGenerator;

    #if (UNITY_EDITOR)
    void OnEnable()
    {
        m_mapGenerator = (MapController)target;
    }

    /// <summary>
    /// Triggers the coins to spawn around this object
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate")) {
            m_mapGenerator.GenerateMap(true);
        } else if (GUILayout.Button("Update")) {
            m_mapGenerator.GenerateMap();
        }
        GUILayout.EndHorizontal();
    }
#endif
}

