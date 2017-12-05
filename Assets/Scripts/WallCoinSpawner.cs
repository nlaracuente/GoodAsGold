using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the frequency, rate, quanitity and destinations of all coins 
/// Spawn
/// </summary>
public class WallCoinSpawner : MonoBehaviour
{
    /// <summary>
    /// A reference to the spawn point to spawn the coins
    /// </summary>
    Transform spawnPoint;

    /// <summary>
    /// A reference to the target point to destroy the coins
    /// </summary>
    Transform targetPoint;

    /// <summary>
    /// How many seconds to wait before spawning cycle begins
    /// </summary>
    [SerializeField]
    float frequency = 3f;

    /// <summary>
    /// How long to wait before spawning the next coin
    /// </summary>
    [SerializeField]
    float rate = 1f;

    /// <summary>
    /// How many coins to spawn per frequency
    /// </summary>
    [SerializeField]
    float total = 10;

    /// <summary>
    /// The coin prefab to spawn
    /// </summary>
    [SerializeField]
    GameObject coinPrafab;

    /// <summary>
    /// How fast the coin moves towards the target
    /// </summary>
    [SerializeField]
    float coinSpeed = 4f;

    /// <summary>
    /// How close to the target before considering it as "arrived"
    /// </summary>
    [SerializeField]
    float distancePad = 0.01f;

    /// <summary>
    /// A reference to the player script
    /// </summary>
    Player player;

    /// <summary>
    /// A reference to the meny script
    /// </summary>
    LevelMenu menu;

    /// <summary>
    /// True while either the player is disabled or the menu is opened
    /// </summary>
    bool DisableSpawner
    {
        get { return this.player.IsDisabled; }
    }

    /// <summary>
    /// True when routines are stopped
    /// </summary>
    bool routinesStopped = true;

    /// <summary>
    /// Gets point references
    /// </summary>
    void Start()
    {
        this.player = FindObjectOfType<Player>();
        this.menu = FindObjectOfType<LevelMenu>();
        this.spawnPoint = this.transform.Find("SpawnPoint");
        this.targetPoint = this.transform.Find("TargetPoint");

        this.StartRoutine();
    }

    public void StartRoutine()
    {
        StartCoroutine(this.Spawn());
    }

    /// <summary>
    /// Only when it view trigger the coroutine
    /// </summary>
    IEnumerator Spawn()
    {
        // Spawn first then wait
        while (!this.DisableSpawner) {

            // Wait until the player is enabled
            if (this.DisableSpawner) {
                continue;
            }

            for (int i = 0; i < this.total; i++) {

                // Wait until the player is enabled
                if (this.DisableSpawner) {
                    continue;
                }

                // Spawn the coins with no rotation
                GameObject coin = Instantiate(this.coinPrafab, this.spawnPoint.position, Quaternion.identity, this.transform);
                coin.GetComponent<Pickup>().StopRotation = true;

                StartCoroutine(this.MoveCoinToTarget(coin));
                yield return new WaitForSeconds(this.rate);
            }

            yield return new WaitForSeconds(this.frequency);
        }        
    }

    /// <summary>
    /// Continues to move the coins spawn until the reach the target
    /// Destroys the coin on arrival
    /// </summary>
    /// <param name="coin"></param>
    /// <returns></returns>
    IEnumerator MoveCoinToTarget(GameObject coin)
    {
        while (!this.DisableSpawner && coin != null && Vector3.Distance(coin.transform.position, this.targetPoint.position) > this.distancePad) {

            // Wait until the player is enabled
            if (this.DisableSpawner) {
                continue;
            }

            coin.transform.position = Vector3.MoveTowards(
                coin.transform.position,
                this.targetPoint.position,
                this.coinSpeed * Time.deltaTime
            );

            yield return new WaitForEndOfFrame();
        }

        Destroy(coin);
    }
}

