using System;
using UnityEngine;

/// <summary>
/// Handles the creation of buttons with the appropriate floor underneath
/// </summary>
public class ButtonTile : BaseObject
{
    /// <summary>
    /// A reference to the button prefab
    /// </summary>
    [SerializeField]
    GameObject m_buttonPrefab;

    /// <summary>
    /// Spawns the button placing ontop of the floor it is currently on
    /// </summary>
    protected override void OnSetup()
    {
        Instantiate(m_buttonPrefab, transform);
        RaiseObjectOntopOfCurrentTile();
    }
}
