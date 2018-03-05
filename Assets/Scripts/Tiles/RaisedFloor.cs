using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaisedFloor : BaseTile
{
    /// <summary>
    /// A reference to the wall prefab
    /// </summary>
    [SerializeField]
    GameObject m_wallPrefab;

    /// <summary>
    /// A reference to the barrier prefab
    /// </summary>
    [SerializeField]
    GameObject m_barrierPrefab;

    /// <summary>
    /// Spawns the wall tile and barriers surrounding it where every it faces a floor tile
    /// </summary>
    protected override void SpawnComponent()
    {
        Instantiate(m_wallPrefab, transform);

        // Spawn barriers for each side adjacent to a floor or ramp tile
        foreach(Vector3 point in GameManager.cardinalPoints) {
            GameObject tile = m_generator.GetTileAt(m_index + point);

            if(tile != null && (tile.CompareTag("Floor") || tile.CompareTag("Ramp"))) {                
                Vector3 tileForwardVector = transform.InverseTransformDirection(tile.transform.forward);
                
                // We want to avoid blocking off ramps that are rotated to align with this tile
                // This means that if the ramp's forward is in the same direction or opposite to the
                // current cardinal point, then the ramp is connecting with this tile so we skip it
                if (tile.CompareTag("Ramp")) {
                    if (tileForwardVector == point || tileForwardVector == -point) {
                        continue;
                    }
                }

                GameObject barrier = Instantiate(m_barrierPrefab, transform);
                barrier.transform.LookAt(tile.transform);
            }
        }
    }
}
