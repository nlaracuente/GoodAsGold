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
    /// Positions the coins based on the tile beneath it
    /// </summary>
    protected override void SpawnComponent()
    {
        MatchFloorHeight();
    }
}
