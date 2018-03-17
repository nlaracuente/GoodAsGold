using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moveable tiles are tiles that contain an object that the player can push or pull
/// </summary>
public class BlockObject : BaseObject
{
    /// <summary>
    /// Make the tile match the height of the floor it is on
    /// </summary>
    protected override void OnSetup()
    {
        RaiseObjectOntopOfCurrentTile();
    }
}
