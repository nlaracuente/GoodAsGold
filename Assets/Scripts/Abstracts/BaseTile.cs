using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all spawnable tiles instantiated by the map generator
/// Tiles are made up of prefabs that make up the "components" of this specific tile
/// Each tile is then asked to "setup" in order to orient itself and/or perform 
/// any other action to setup the tile
/// </summary>
public abstract class BaseTile : MonoBehaviour
{
    /// <summary>
    /// True if this a place where the player or moveable tile can walk into
    /// </summary>
    [SerializeField]
    protected bool m_isWalkable = true;
    public bool IsWalkable { get { return m_isWalkable; } }

    /// <summary>
    /// True is this is a tile that contains an object
    /// </summary>
    [SerializeField]
    protected bool m_isObject = true;
    public bool IsObject { get { return IsObject; } }

    /// <summary>
    /// A reference to the map generator that spawned this tile
    /// </summary>
    protected MapGenerator m_generator;

    /// <summary>
    /// Array index position this tile exist within the spanwed tiles
    /// </summary>
    protected Vector3 m_index = Vector3.zero;
    /// <summary>
    /// Position within the map's array where this is located at
    /// </summary>
    public Vector3 Index { get { return m_index; } }

    /// <summary>
    /// How much to raise the tile to place it above a raised floor
    /// </summary>
    [SerializeField, Tooltip("How much to raise this object when on an elevated floor")]
    protected float m_raisedDistance = 2.49f;

    /// <summary>
    /// Stores a ference to the map generator as well as this tile's index within the spawned tiles array
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public void Init (MapGenerator generator, int x, int z)
    {
        m_index = new Vector3(x, 0f, z);
        m_generator = generator;
    }

    /// <summary>
    /// Instatiates the components that make up this tile and runs their setup logic
    /// </summary>
    /// <param name="generator"></param>
    public virtual void Setup()
    {
        SpawnComponent();
    }

    /// <summary>
    /// Instatiates all components
    /// Orients components as needed
    /// Runs any additional logic to configure this tile
    /// </summary>
    protected abstract void SpawnComponent();

    /// <summary>
    /// Raises this object to be on top of the floor tile that is underneath
    /// </summary>
    protected void MatchFloorHeight()
    {
        if (m_generator == null) {
            Debug.LogErrorFormat("CoinTile SpawnComponent Error! m_generator is missing for tile {0}", name);
            return;
        }

        GameObject floorTile = m_generator.GetTileAt(m_index);

        if (floorTile != null && floorTile.CompareTag("RaisedFloor")) {
            transform.position += Vector3.up * m_raisedDistance;
        }
    }
}
