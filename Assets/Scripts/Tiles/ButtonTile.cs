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
    /// A reference to the floor prefab
    /// </summary>
    [SerializeField]
    GameObject m_floorPrefab;

    /// <summary>
    /// A reference to the raised floor prefab
    /// </summary>
    [SerializeField]
    GameObject m_raisedFloorPrefab;

    /// <summary>
    /// How far to raise the button by when on a raised floor
    /// </summary>
    [SerializeField, Tooltip("How far to raise the door when on a raised floor")]
    float m_raisedDistance = 2.45f;

    /// <summary>
    /// Creates the button with the proper floor underneath of it
    /// </summary>
    protected override void SpawnComponent()
    {
        GameObject button = Instantiate(m_buttonPrefab, transform);

        // Search for the first instance of a surrounding floor tile
        // This dictates which type of floor to create underneath of the button
        foreach (Vector3 point in GameManager.cardinalPoints) {
            GameObject tile = m_generator.GetTileAt(m_index + point);

            if (tile.CompareTag("Floor") || tile.CompareTag("RaisedFloor")) {
                // Raise the door and spawn the proper floor tile
                if (tile.CompareTag("RaisedFloor")) {
                    button.transform.position += Vector3.up * m_raisedDistance;
                    GameObject floor = Instantiate(m_raisedFloorPrefab, transform);
                    BaseTile floorTile = floor.GetComponent<BaseTile>();

                    // Have to run the update so that the tile creates itself
                    floorTile.Init(m_generator, (int)m_index.x, (int)m_index.z);
                    floorTile.Setup();

                    // Regular floor
                } else {
                    Instantiate(m_floorPrefab, transform);
                }

                break;
            }
        }
    }
}
