using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCoinSpawnerObject : BaseObject
{
    protected override void OnSetup()
    {
        BaseTile onTile = MapController.instance.GetTileAt(m_index);

        foreach (Vector3 point in GameManager.cardinalPoints) {
            BaseTile tile = MapController.instance.GetTileAt(m_index + point);

            // Rotate to look at the wall
            if (tile != null && tile.CompareTag("Wall")) {
                transform.LookAt(tile.transform);
                break;
            }
        }

        RaiseObjectOntopOfCurrentTile();
    }
}
