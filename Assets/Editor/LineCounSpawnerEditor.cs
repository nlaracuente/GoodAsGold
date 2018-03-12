using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LineCoinSpawner))]
[CanEditMultipleObjects]
public class LineCounSpawnerEditor : Editor
{
    /// <summary>
    /// The following represent snapshots of the Spawner's values to avoid
    /// spawning the coins when the values have not changed
    /// </summary>
    int m_columns;
    int m_rows;
    float m_columPadding;
    float m_rowPadding;
    float m_coinWidth;
    float m_coinHeight;
    float m_distanceToGround;

    LineCoinSpawner m_spawner;

    #if (UNITY_EDITOR)
    void OnEnable()
    {
        m_spawner = (LineCoinSpawner)target;
        UpdateValues();
    }

    /// <summary>
    /// Triggers the coins to spawn around this object
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (ValuesChanged()) {
            UpdateValues();
            m_spawner.SpawnCoins();
        }
    }
    
    /// <summary>
    /// Updates the snapshot values
    /// </summary>
    void UpdateValues()
    {
        if (m_spawner == null) {
            return;
        }

        m_columns = m_spawner.Colums;
        m_rows = m_spawner.Rows;
        m_columPadding = m_spawner.ColumnPadding;
        m_rowPadding = m_spawner.RowPadding;
        m_coinWidth = m_spawner.CoinWidth;
        m_coinHeight = m_spawner.CoinHeight;
        m_distanceToGround = m_spawner.DistanceToGround;
    }

    /// <summary>
    /// Returns true if any of the values have changed
    /// </summary>
    /// <returns></returns>
    bool ValuesChanged()
    {
        if (m_spawner == null) {
            return false;
        }

        bool changed = 
            m_columns != m_spawner.Colums ||
            m_rows != m_spawner.Rows ||
            m_columPadding != m_spawner.ColumnPadding ||
            m_rowPadding != m_spawner.RowPadding ||
            m_coinWidth != m_spawner.CoinWidth ||
            m_coinHeight != m_spawner.CoinHeight ||
            m_distanceToGround != m_spawner.DistanceToGround;        

        return changed;
    }
    #endif
}
