using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for all objects that are spawned by the MapController
/// This allows the object to be initialized and setup
/// </summary>
public interface ISpawnable
{
    /// <summary>
    /// The location within the tilemap's array this is located at
    /// </summary>
    Vector3 Index { get; set; }

    /// <summary>
    /// True when the object is in a state where the player can walk on it
    /// </summary>
    bool IsWalkable { get; set; }

    /// <summary>
    /// Contains all the logic to "setup" the object such as rotation and y position
    /// </summary>
    void Setup();
}
