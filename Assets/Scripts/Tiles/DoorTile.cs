using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the basic behaviors of doors from being opened/closed
/// To being triggered to such by the button subscriber component
/// associated with this tile
/// </summary>
public class DoorTile : BaseTile
{
    /// <summary>
    /// A reference to the actual door model prefab
    /// </summary>
    [SerializeField]
    GameObject m_doorPrefab;

    /// <summary>
    /// True when the door is opened
    /// </summary>
    [SerializeField]
    bool m_isOpened = false;

    /// <summary>
    /// The name of the animator controller bool for openning/closing the door
    /// </summary>
    [SerializeField]
    string m_isOpenedTag = "IsOpened";

    /// <summary>
    /// A reference to the animator component
    /// </summary>
    Animator m_animator;

    /// <summary>
    /// A reference to any of the button subscriber attached to this door
    /// </summary>
    ButtonSubscriber m_buttonSubscriber;

    /// <summary>
    /// Set references
    /// Registeres to the button subscriber on activate/deactivate
    /// </summary>
    void Awake()
    {
        m_buttonSubscriber = GetComponent<ButtonSubscriber>();
        m_animator = GetComponentInChildren<Animator>();

        if (m_buttonSubscriber == null || m_animator == null) {
            Debug.LogErrorFormat("Door Error: Missing Component! " +
                "Button Subscriber = {0}",
                m_buttonSubscriber,
                m_animator
            );
        } else {
            m_buttonSubscriber.OnActivate += Open;
            m_buttonSubscriber.OnDeactivate += Close;
        }
    }

    /// <summary>
    /// Updates the animator controller to trigger door opened/close
    /// </summary>
    void Update()
    {
        m_animator.SetBool(m_isOpenedTag, m_isOpened);
    }

    /// <summary>
    /// Changes the status of the door to opened
    /// </summary>
    void Open()
    {
        m_isOpened = true;
        m_isWalkable = true;
    }

    /// <summary>
    /// Changes the status of the door to closed
    /// </summary>
    void Close()
    {
        m_isOpened = false;
        m_isWalkable = false;
    }

    /// <summary>
    /// Instantiates the door at a high that matches the floor type it is standing on
    /// Looks for the first tile of similar type than the one it is standing on to face it
    /// so long as there isn't another object on that tile
    /// </summary>
    protected override void SpawnComponent()
    {
        // Spawn the door
        GameObject door = Instantiate(m_doorPrefab, transform);

        // Tile the door sits on
        GameObject onTile = m_generator.GetTileAt(m_index);
        
        // Look for the first neighbor tile of the same type the door is on
        // This is used to rotate the door to face that tile as it must be a connecting tile
        foreach (Vector3 point in GameManager.cardinalPoints) {
            GameObject tile = m_generator.GetTileAt(m_index + point);
            GameObject objectOnTile = m_generator.GetObjectAt(m_index + point);

            if (tile != null && tile.CompareTag(onTile.tag) && objectOnTile == null) {
                transform.LookAt(tile.transform);
                break;
            }
        }

        MatchFloorHeight();
    }
}
