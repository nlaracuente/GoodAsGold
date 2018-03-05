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
                Vector3 localForward = transform.InverseTransformDirection(transform.forward);
                Vector3 localTileForward = transform.InverseTransformDirection(tile.transform.forward);

                float dot = Vector3.Dot(localForward, localTileForward);

                // Ramp is facing the same or opposite direction as this tile
                if (tile.CompareTag("Ramp") && dot != 0) {
                    continue;
                }

                GameObject barrier = Instantiate(m_barrierPrefab, transform);
                barrier.transform.LookAt(tile.transform);
            }
        }
    }
}
