using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LineCoinSpawner))]
public class LineCounSpawnerEditor : Editor
{
    LineCoinSpawner m_spawner;

    #if (UNITY_EDITOR)
    void OnEnable()
    {
        m_spawner = (LineCoinSpawner)target;
    }

    /// <summary>
    /// Triggers the coins to spawn around this object
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        m_spawner.SpawnCoins();
    }
    #endif
}
