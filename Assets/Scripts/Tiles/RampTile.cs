using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampTile : BaseTile
{
    /// <summary>
    /// A reference to the floor prefab
    /// </summary>
    [SerializeField]
    GameObject m_floorPrefab;

    /// <summary>
    /// A reference to the ramp model
    /// </summary>
    GameObject m_rampModel;

    /// <summary>
    /// Spawns the floor and ramp models
    /// Rotates the ramp to face the first floor tile it finds around the 4 cardinal points
    /// </summary>
    protected override void SpawnComponent()
    {
        Instantiate(m_floorPrefab, transform);

        foreach (Vector3 point in GameManager.cardinalPoints) {
            GameObject tile = m_generator.GetTileAt(m_index + point);

            if(tile != null && tile.CompareTag("Floor")) {
                transform.LookAt(tile.transform);
                break;
            }
        }
    }
}
