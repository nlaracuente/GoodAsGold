using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Creates coins in columns and rows based on the total lengths for each
/// </summary>
public class LineCoinSpawner : MonoBehaviour
{
    /// <summary>
    /// Total columns
    /// </summary>
    [SerializeField, Tooltip("Total columns to create"), Range(1, 100)]
    int m_columns = 1;
    public int Colums { get { return m_columns; } }

    /// <summary>
    /// Total rows
    /// </summary>
    [SerializeField, Tooltip("Total rows to create"), Range(1, 100)]
    int m_rows = 1;
    public int Rows { get { return m_columns; } }

    /// <summary>
    /// How much space in between columns to add
    /// </summary>
    [SerializeField, Tooltip("Space between columns"), Range(0f, 5f)]
    float m_columnPadding = 1f;
    public float ColumnPadding { get { return m_columnPadding; } }

    /// <summary>
    /// How much space in between rows to add
    /// </summary>
    [SerializeField, Tooltip("Space between rows"), Range(0f, 5f)]
    float m_rowPadding = 1f;
    public float RowPadding { get { return m_rowPadding; } }

    /// <summary>
    /// How many units is the coin's width
    /// </summary>
    [SerializeField, Tooltip("Width in units"), Range(0f, 5f)]
    float m_coinWidth = 3f;
    public float CoinWidth { get { return m_coinWidth; } }

    /// <summary>
    /// How many units is the coin's height
    /// </summary>
    [SerializeField, Tooltip("Height in units"), Range(0f, 5f)]
    float m_coinHeight = 3f;
    public float CoinHeight { get { return m_coinHeight; } }

    /// <summary>
    /// How from the ground to spawn the coin
    /// </summary>
    [SerializeField, Range(0f, 100f)]
    float m_distanceToGround = 1f;
    public float DistanceToGround { get { return m_distanceToGround; } }

    /// <summary>
    /// The coin prefab
    /// </summary>
    [SerializeField]
    GameObject m_coinPrefab;

    /// <summary>
    /// Triggers the creation of the coins
    /// </summary>
    void Start()
    {
        SpawnCoins();
    }

    /// <summary>
    /// Spawns the coins around this object
    /// </summary>
    public void SpawnCoins()
    {
        // For some reason not all the coins get cleaned up (possibly due to how the editor script is calling
        // this spawn coins too many times in one update hence calling the RemoveCoins this many times
        RemoveCoins();
        RemoveCoins();
        RemoveCoins();

        // Used for centering the coins on the parent object
        float columnOffset = Mathf.Round((m_columns * m_coinWidth) * .5f);
        float rowOffset = Mathf.Round((m_rows * m_coinHeight) * .5f);

        for (int column = 0; column < m_columns; column++) {
            for (int row = 0; row < m_rows; row++) {
                string instanceName = string.Format("Coin_{0}_{1}", column, row);

                // Already exist
                if (transform.Find(instanceName) != null) {
                    continue;
                }

                GameObject instance = null;

                Vector3 position = new Vector3(
                    column + (m_columnPadding * column),
                    m_distanceToGround, 
                    row + (m_rowPadding * row)
                );

                position.x -= columnOffset;
                position.z -= rowOffset;

                if (!Application.isEditor) {
                    instance = Instantiate(m_coinPrefab, position, Quaternion.identity);
                } else {
                    instance = PrefabUtility.InstantiatePrefab(m_coinPrefab) as GameObject;
                    instance.transform.position = position;
                }
                
                instance.name = instanceName;
                instance.transform.SetParent(transform, false);
            }
        }
    }

    /// <summary>
    /// Destroys all the coins in th spawner to make room for new ones
    /// </summary>
    public void RemoveCoins()
    {
        foreach (Transform child in transform) {
            if (child.name == "CoinModel") {
                DestroyImmediate(child.gameObject, true);
            }
        }
    }
}
