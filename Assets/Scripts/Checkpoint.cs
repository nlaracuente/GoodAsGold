using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checkpoints fully remove the player's curse and creates a restore point should they die
/// or desire to restart from last checkpoint
/// </summary>
public class Checkpoint : MonoBehaviour
{
    /// <summary>
    /// A refrence to the particle system
    /// </summary>
    [SerializeField]
    ParticleSystem m_particles;

    /// <summary>
    /// A reference to the player manager object
    /// </summary>
    PlayerManager m_playerManager;

    /// <summary>
    /// Set references
    /// </summary>
    void Awake()
    {
        if(m_particles == null) {
            m_particles = GetComponentInChildren<ParticleSystem>();
        }

        m_playerManager = FindObjectOfType<PlayerManager>();

        if(m_particles == null || m_playerManager == null) {
            Debug.LogErrorFormat("Checkpoint Error: Missing Component. " +
                "ParticleSystem = {0}, PlayerManager = {1}",
                m_particles,
                m_playerManager
            );
        }
    }

    /// <summary>
    /// Resets the player's curse percentage to 0
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            m_playerManager.CursePercent = 0f;
            m_particles.Play();
        }
    }
}
