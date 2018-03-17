using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampObject : BaseObject
{
    /// <summary>
    /// A reference to the ramp model
    /// </summary>
    GameObject m_rampModel;

    /// <summary>
    /// Spawns ramp and rotates to face for first instance of a tile of the same type as 
    /// the tile the ramp is on thus linking that bottom of the ramp with a connecting floor of the same height
    /// </summary>
    protected override void OnSetup()
    {
        BaseTile onTile = MapController.instance.GetTileAt(m_index);

        foreach (Vector3 point in GameManager.cardinalPoints) {
            BaseTile tile = MapController.instance.GetTileAt(m_index + point);

            // Tile must be of the same type as the one the ramp is currently on
            if (tile != null && tile.CompareTag(onTile.tag) ) {
                
                // Ignore tiles that have ramps on it
                if(tile.ObjectOnTile != null && tile.ObjectOnTile.CompareTag(tag)) {
                    continue;
                }

                transform.LookAt(tile.transform);
                break;
            }
        }

        RaiseObjectOntopOfCurrentTile();
    }
}
