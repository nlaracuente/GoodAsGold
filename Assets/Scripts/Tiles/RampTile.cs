using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampTile : BaseTile
{
    /// <summary>
    /// A reference to the ramp model
    /// </summary>
    GameObject m_rampModel;

    /// <summary>
    /// Spawns ramp and rotates to face for first instance of a tile of the same type as 
    /// the tile the ramp is on thus linking that bottom of the ramp with a connecting floor of the same height
    /// </summary>
    protected override void SpawnComponent()
    {
        GameObject onTile = m_generator.GetTileAt(m_index);

        foreach (Vector3 point in GameManager.cardinalPoints) {
            GameObject tile = m_generator.GetTileAt(m_index + point);
            GameObject objectOnTile = m_generator.GetObjectAt(m_index + point);

            if (tile != null && tile.CompareTag(onTile.tag) ) {

                // Ignore tiles that have ramps on it
                if(objectOnTile != null && objectOnTile.CompareTag(tag)) {
                    continue;
                }

                transform.LookAt(tile.transform);
                break;
            }
        }
    }
}
