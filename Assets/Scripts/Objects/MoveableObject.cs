using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Objects that can be pushed by the player
/// </summary>
public class MoveableObject : BaseObject
{
    protected override void OnSetup()
    {
        RaiseObjectOntopOfCurrentTile();
    }
}
