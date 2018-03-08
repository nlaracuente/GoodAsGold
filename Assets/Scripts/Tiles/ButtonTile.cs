using System;
using UnityEngine;

/// <summary>
/// Handles the creation of buttons with the appropriate floor underneath
/// </summary>
public class ButtonTile : BaseTile
{
    /// <summary>
    /// A reference to the button prefab
    /// </summary>
    [SerializeField]
    GameObject m_buttonPrefab;

    /// <summary>
    /// How far to raise the button by when on a raised floor
    /// </summary>
    [SerializeField, Tooltip("How far to raise the door when on a raised floor")]
    float m_raisedDistance = 2.45f;

    /// <summary>
    /// Spawns the button placing ontop of the floor it is currently on
    /// </summary>
    protected override void SpawnComponent()
    {
        GameObject button = Instantiate(m_buttonPrefab, transform);
        GameObject tile = m_generator.GetTileAt(m_index);

        // Raise the door and spawn the proper floor tile
        if (tile != null && tile.CompareTag("RaisedFloor")) {
            button.transform.position += Vector3.up * m_raisedDistance;
        }
    }
}
