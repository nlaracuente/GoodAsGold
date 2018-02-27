using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pharaoh Statue shoot coins like fire from their mouth when triggered (or on auto start)
/// and stop only when told to
/// </summary>
[RequireComponent(typeof(Collider))]
public class PharaohStatue : MonoBehaviour
{
    /// <summary>
    /// The particle system that simulates coins being fired
    /// </summary>
    [SerializeField]
    ParticleSystem m_particles;

    /// <summary>
    /// How many coins to add per iteration
    /// </summary>
    [SerializeField, Tooltip("How much to increase player's coin count by")]
    int m_coinDamage = 3;

    /// <summary>
    /// How long to wait before inflicting damage
    /// </summary>
    [SerializeField, Tooltip("Time to wait before inflicting damage")]
    float m_damageDelay = .25f;

    /// <summary>
    /// While active the statue fires coins and detects player collision
    /// </summary>
    [SerializeField]
    bool m_isActive = false;
    public bool IsActive {
        get { return m_isActive; }
        set {
            m_isActive = value;
            if (m_isActive) {
                m_particles.Play();
            } else {
                m_particles.Stop();
            }
        }
    }

    /// <summary>
    /// A reference to the player to inflict damage
    /// </summary>
    PlayerManager m_player;

    /// <summary>
    /// Failsafe flag to avoid multiple routines from running
    /// </summary>
    bool m_routineRunning = false;


    /// <summary>
    /// Set references
    /// </summary>
    void Awake()
    {
        m_player = FindObjectOfType<PlayerManager>();

        if(m_particles == null || m_player == null) {
            Debug.LogErrorFormat("Pharaoh Statue Error: Missing Component! " +
                "Particle System = {0}, Player = {1}",
                m_particles,
                m_player
            );
        }
    }

    /// <summary>
    /// Causes the particle system to start/stop
    /// </summary>
    void Start()
    {
        this.IsActive = m_isActive;
    }

    /// <summary>
    /// Triggers the routine to inflict damage
    /// </summary>
    void InflictDamage()
    {
        if (!m_routineRunning) {
            StartCoroutine("InflictDamageRoutine");
        }
    }

    /// <summary>
    /// Stops the routine that inflicts damage
    /// </summary>
    void StopDamage()
    {
        m_routineRunning = false;
        StopCoroutine("InflictDamageRoutine");
    }

    /// <summary>
    /// If the player is within the trigger collider then initiate the damage increase
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        // Not the player
        if (!other.CompareTag("Player")) {
            return;
        }

        InflictDamage();
    }

    /// <summary>
    /// Stops inflicting damage when the player leaves the trigger collider
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerExit(Collider other)
    {
        // Not the player
        if (!other.CompareTag("Player")) {
            return;
        }

        StopDamage();
    }

    /// <summary>
    /// Increases the player's coin collection in intervals while it is active
    /// and the player is not fully cursed
    /// </summary>
    /// <returns></returns>
    IEnumerator InflictDamageRoutine()
    {
        m_routineRunning = true;

        // Runs while is active and the player is not fully cursed
        while (m_isActive && m_player.CursePercent < 100f) {
            m_player.CoinsCollected += m_coinDamage;
            yield return new WaitForSeconds(m_damageDelay);
        }

        // Turn it off if the player is dead so that we can see the dead animation
        if (m_player.CursePercent >= 100f) {
            this.IsActive = false;
        }

        m_routineRunning = false;
    }
}
