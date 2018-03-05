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
    /// A reference to the map generator that spawned this tile
    /// </summary>
    protected MapGenerator m_generator;

    /// <summary>
    /// Array index position this tile exist within the spanwed tiles
    /// </summary>
    protected Vector3 m_index = Vector3.zero;

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
}
