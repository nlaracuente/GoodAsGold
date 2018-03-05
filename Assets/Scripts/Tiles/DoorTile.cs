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
    /// A reference to the floor prefab
    /// </summary>
    [SerializeField]
    GameObject m_floorPrefab;

    /// <summary>
    /// A reference to the raised floor prefab
    /// </summary>
    [SerializeField]
    GameObject m_raisedFloorPrefab;

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
    /// How far to raise the door by when on a raised floor
    /// </summary>
    [SerializeField, Tooltip("How far to raise the door when on a raised floor")]
    float m_raisedDistance = 2.25f;

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
    }

    /// <summary>
    /// Changes the status of the door to closed
    /// </summary>
    void Close()
    {
        m_isOpened = false;
    }

    /// <summary>
    /// Instantiates the floor that goes under the door
    /// Orients itself as to have a wall tile on each side
    /// </summary>
    protected override void SpawnComponent()
    {
        GameObject door = Instantiate(m_doorPrefab, transform);

        // Search for the first instance of a surrounding floor
        // This dictates which direction to look at and which type of
        // floor to create underneath of the door
        foreach(Vector3 point in GameManager.cardinalPoints) {
            GameObject tile = m_generator.GetTileAt(m_index + point);

            if (tile.CompareTag("Floor") || tile.CompareTag("RaisedFloor")) {
                transform.LookAt(tile.transform);

                // Raise the door and spawn the proper floor tile
                if (tile.CompareTag("RaisedFloor")) {
                    door.transform.position += Vector3.up * m_raisedDistance;
                    GameObject floor = Instantiate(m_raisedFloorPrefab, transform);
                    BaseTile floorTile = floor.GetComponent<BaseTile>();

                    // Have to run the update so that the tile creates itself
                    floorTile.Init(m_generator, (int)m_index.x, (int)m_index.z);
                    floorTile.Setup();

                // Regular floor
                } else {
                    Instantiate(m_floorPrefab, transform);
                }

                break;
            }
        }

        //GameObject tileAbove  = m_generator.GetTileAt(m_index + GameManager.directions["up"]);
        //GameObject tileBellow = m_generator.GetTileAt(m_index + GameManager.directions["down"]);
        //GameObject tileLeft = m_generator.GetTileAt(m_index + GameManager.directions["left"]);
        //GameObject tileRight = m_generator.GetTileAt(m_index + GameManager.directions["left"]);

        //if (tileAbove.CompareTag("Wall") || tileBellow.CompareTag("Wall")) {
        //    door.transform.rotation = Quaternion.LookRotation(GameManager.directions["left"]);
        //} else {
        //    door.transform.rotation = Quaternion.LookRotation(GameManager.directions["up"]);
        //}

        //// Raise this object if we are on a raised floor
        //if(tileAbove.CompareTag("RaisedFloor") || 
        //    tileLeft.CompareTag("RaisedFloor") ||
        //    tileBellow.CompareTag("RaisedFloor") ||
        //    tileRight.CompareTag("RaisedFloor")) {

        //    door.transform.position += Vector3.up * m_raisedDistance;

        //    GameObject floor = Instantiate(m_raisedFloorPrefab, transform);
        //    floor.GetComponent<BaseTile>().Setup();

        //} else {
        //    Instantiate(m_floorPrefab, transform);
        //}
    }
}
