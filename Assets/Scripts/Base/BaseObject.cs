using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseObject : MonoBehaviour, ISpawnable
{
    /// <summary>
    /// Array index position this tile exist within the spanwed tiles
    /// </summary>
    protected Vector3 m_index = Vector3.zero;
    public Vector3 Index { get { return m_index; } set { m_index = value; } }

    /// <summary>
    /// True if this a place where the player or moveable tile can walk into
    /// </summary>
    [SerializeField]
    protected bool m_isWalkable = true;
    public bool IsWalkable { get { return m_isWalkable; } set { m_isWalkable = value; } }

    /// <summary>
    /// Sets references
    /// The position within the array has to be set again as it is lost in play mode
    /// </summary>
    void Awake()
    {
        m_index.Set(
            transform.position.x / GameManager.tileXSize,
            0f,
            transform.position.z / GameManager.tileZSize
        );
    }

    /// <summary>
    /// Instatiates the components that make up this tile and runs their setup logic
    /// </summary>
    /// <param name="generator"></param>
    public virtual void Setup()
    {
        OnSetup();
    }

    /// <summary>
    /// Instatiates all components
    /// Orients components as needed
    /// Runs any additional logic to configure this tile
    /// </summary>
    protected abstract void OnSetup();

    /// <summary>
    /// Ensures this object rests ontop of the tile it is currently on
    /// </summary>
    /// <param name="obj"></param>
    protected void RaiseObjectOntopOfCurrentTile()
    {
        BaseTile tile = MapController.instance.GetTileAt(m_index);

        if (tile != null) {
            transform.position += Vector3.up * tile.ObjectRaiseDistance;
        }
    }
}
