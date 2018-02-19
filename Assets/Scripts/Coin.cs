using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A coin has two states (invisible/visible)
/// Based on the current state, collecting a coin will either make it appear or dissapear
/// Collected coins increase the player's curse causing them to move/rotate/etc. slower 
/// </summary>
public class Coin : MonoBehaviour
{
    enum State
    {
        Visible,
        Invisible,
    }

    /// <summary>
    /// Current coin state
    /// </summary>
    [SerializeField]
    State m_state;

    /// <summary>
    /// A reference to the coin model to make it rotate
    /// </summary>
    [SerializeField]
    GameObject m_coinModel;

    /// <summary>
    /// How fast to rotate the coin
    /// </summary>
    [SerializeField]
    float m_rotationSpeed = 5f;
    
    /// <summary>
    /// Rotate the coin
    /// </summary>
    void Update()
    {
        transform.Rotate(Vector3.up * m_rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// When the player collides with the coin it:
    ///     - Reveals hidden coins
    ///     - Triggers player to collect coin and destroys this object
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        // Ignore non-player collisions
        if (!other.CompareTag("Player")) {
            return;
        }

        PlayerManager playerManager = other.GetComponent<PlayerManager>();

        // Log error and destroy object
        if (playerManager == null) {
            Debug.LogErrorFormat("Coin OnTriggerEnter: Failed to get PlayerManager component from {0}", other.name);
            Destroy(gameObject);
        }

        switch (m_state) {
            case State.Visible:
                playerManager.CoinsCollected++;
                break;
        }

        Destroy(gameObject);
    }
}
