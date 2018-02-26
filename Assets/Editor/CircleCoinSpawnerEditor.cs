using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Invokes the creations of the coins
/// </summary>
[CustomEditor(typeof(CircleCoinSpawner))]
[CanEditMultipleObjects]
public class CircleCoinSpawnerEditor : Editor
{
    CircleCoinSpawner m_spawner;

    #if (UNITY_EDITOR)
    void OnEnable()
    {
        m_spawner = (CircleCoinSpawner)target;
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
