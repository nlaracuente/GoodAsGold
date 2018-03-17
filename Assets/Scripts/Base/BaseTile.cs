using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A spawnable is an object that is spanwed by the map generator
/// Spawnable contains reference to where on the map array they are located
/// and whether or not they contain an object ontop of them
/// </summary>
public abstract class BaseTile : MonoBehaviour, ISpawnable
{
    /// <summary>
    /// True if this a place where the player or moveable tile can walk into
    /// </summary>
    [SerializeField]
    protected bool m_isWalkable = true;
    public bool IsWalkable { get { return m_isWalkable; } set { m_isWalkable = value; } }

    /// <summary>
    /// Array index position this tile exist within the spanwed tiles
    /// </summary>
    protected Vector3 m_index = Vector3.zero;

    /// <summary>
    /// Position within the map's array where this is located at
    /// </summary>
    public Vector3 Index { get { return m_index; } set { m_index = value; } }

    /// <summary>
    /// How much to raise the tile to place it above a raised floor
    /// </summary>
    [SerializeField, Tooltip("How much to raise objects on this tile")]
    protected float m_objectRaiseDistance = 2.49f;
    public float ObjectRaiseDistance { get { return m_objectRaiseDistance; } }

    /// <summary>
    /// A reference to the object that is on this tile
    /// </summary>
    BaseObject m_objectOnTile;
    public BaseObject ObjectOnTile { get { return m_objectOnTile; } set { m_objectOnTile = value; } }


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
    /// Raises this object to be on top of the floor tile that is underneath
    /// </summary>
    protected void MatchFloorHeight()
    {
        BaseTile floorTile = MapController.instance.GetTileAt(m_index);

        if (floorTile != null && floorTile.CompareTag("RaisedFloor")) {
            transform.position += Vector3.up * m_objectRaiseDistance;
        }
    }
}
