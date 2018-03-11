using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds coins that are fixed in place
/// </summary>
public class CoinTile : BaseTile
{
    /// <summary>
    /// How far to raise the button by when on a raised floor
    /// </summary>
    [SerializeField, Tooltip("How much to raise this object when on an elevated floor")]
    float m_raisedDistance = 2.45f;

    /// <summary>
    /// Positions the coins based on the tile beneath it
    /// </summary>
    protected override void SpawnComponent()
    {
        GameObject tile = m_generator.GetTileAt(m_index);
        
        if (tile != null && tile.CompareTag("RaisedFloor")) {
            transform.position += Vector3.up * m_raisedDistance;
        }
    }
}
