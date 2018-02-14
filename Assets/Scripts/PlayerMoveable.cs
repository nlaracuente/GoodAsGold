using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Objects that are moveable by the player
/// These objects can be pushed or pulled
/// </summary>
public class PlayerMoveable : MonoBehaviour, IClickable
{
    /// <summary>
    /// A list of all directions this object can be moved
    /// </summary>
    List<Vector3> directions = new List<Vector3>() {
        Vector3.forward,
        Vector3.left,
        Vector3.back,
        Vector3.right
    };

    public void OnClick()
    {
        Debug.LogFormat("{0} was interacted with", gameObject.name);
    }
}
