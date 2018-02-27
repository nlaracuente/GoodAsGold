using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wall Coin Spawners reside on a wall and spawn coins at a given frequence
/// Each coin then moves in the wall spawner's forward direction until reaching the destion
/// Upon reaching the destination the coins are destroyed 
/// This repeats itself with no end
/// </summary>
public class WallCoinSpawner : MonoBehaviour
{
    /// <summary>
    /// The coin prefab to spawn
    /// </summary>
    [SerializeField]
    GameObject m_coinPrefab;

    /// <summary>
    /// Where to spawn the coins at
    /// </summary>
    [SerializeField]
    Transform m_startTransform;

    /// <summary>
    /// Where to move the coins towards
    /// </summary>
    [SerializeField]
    Transform m_destinationTransform;

    /// <summary>
    /// How to long to wait between waves
    /// </summary>
    [SerializeField, Tooltip("Delays between waves")]
    float m_waveDelay = 5f;

    /// <summary>
    /// How long to wait before spawning the next coins
    /// </summary>
    [SerializeField, Tooltip("Delays between each coins being spawned")]
    float m_spawnDelay = .5f;

    /// <summary>
    /// How many coins per wave to spawn
    /// </summary>
    [SerializeField, Tooltip("Total coins per wave")]
    float m_totalCoins = 10f;

    /// <summary>
    /// How fast the coin moves
    /// </summary>
    [SerializeField]
    float m_coinSpeed = 5f;

    /// <summary>
    /// Triggers the spawining of the coins immediately
    /// </summary>
    [SerializeField]
    bool m_autoStart = true;

    /// <summary>
    /// Triggers the coins to spawn when auto start
    /// </summary>
    void Start()
    {
        if (m_autoStart) {
            SpawnCoins();
        }    
    }

    /// <summary>
    /// Triggers the routine to spawn coins
    /// </summary>
    public void SpawnCoins()
    {
        StartCoroutine("SpawnCoinRoutine");
    }

    /// <summary>
    /// Spawns the waves of coins
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnCoinRoutine()
    {
        while (true) {
            for (int i = 0; i < m_totalCoins; i++) {
                GameObject instance = Instantiate(m_coinPrefab, m_startTransform.position, Quaternion.identity, transform);
                Coin coin = instance.GetComponent<Coin>();
                coin.MoveTowards(m_destinationTransform.position, m_coinSpeed);
                yield return new WaitForSeconds(m_spawnDelay);
            }
            yield return new WaitForSeconds(m_waveDelay);
        }
    }
}
