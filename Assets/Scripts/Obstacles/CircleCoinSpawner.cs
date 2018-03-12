using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns a ring of coins around the center of this object and spins them
/// </summary>
public class CircleCoinSpawner : MonoBehaviour
{
    /// <summary>
    /// How many coins to spawn
    /// </summary>
    [SerializeField, Tooltip("Total coins minus one to allow an opening"), Range(2, 101)]
    int m_total = 10;

    /// <summary>
    /// How far from the center of this object to spawn the coins at
    /// </summary>
    [SerializeField, Tooltip("Distance from the center"), Range(1f, 10f)]
    float m_radius = 5f;

    /// <summary>
    /// The coin prefab
    /// </summary>
    [SerializeField]
    GameObject m_coinPrefab;

    /// <summary>
    /// Used to set the rotating state of the the individual coins
    /// </summary>
    [SerializeField, Tooltip("Allow individual coin rotation")]
    bool m_coinsRotate = false;

    /// <summary>
    /// Triggers the creation of the coins
    /// </summary>
    void Start()
    {
        SpawnCoins();
    }

    /// <summary>
    /// Spawns the coins around this object
    /// </summary>
    public void SpawnCoins()
    {
        // For some reason not all the coins get cleaned up (possibly due to how the editor script is calling
        // this spawn coins too many times in one update hence calling the RemoveCoins this many times
        RemoveCoins();
        RemoveCoins();
        RemoveCoins();

        Vector3 center = transform.position;

        for (int i = 1; i < m_total; i++) {
            string instanceName = string.Format("Coin_{0}", i);

            // Already exist
            if(transform.Find(instanceName) != null) {
                continue;
            }

            float angle = i * Mathf.PI * 2 / (float)m_total;

            Vector3 position = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * m_radius;
            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, center - position);

            GameObject instant = Instantiate(m_coinPrefab, position, rotation);
            instant.name = instanceName;
            instant.transform.SetParent(transform, false);

            // Set the coin's rotation state
            Coin coin = instant.GetComponent<Coin>();
            if(coin != null) {
                coin.HasRotation = m_coinsRotate;
            }
        }
    }

    /// <summary>
    /// Destroys all the coins in th spawner to make room for new ones
    /// </summary>
    public void RemoveCoins()
    {
        foreach (Transform child in transform) {
            if (child.name == "CoinModel") {
                DestroyImmediate(child.gameObject, true);
            }
        }
    }
}
