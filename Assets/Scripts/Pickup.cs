using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// These are the items the player is trying to avoid
/// When the player comes in contact with them they are "collected"
/// and removed from the level
/// </summary>
public class Pickup : MonoBehaviour
{
    /// <summary>
    /// Rotation speed
    /// </summary>
    [SerializeField]
    float rotationSpeed = -120f;

    /// <summary>
    /// How fast to rotate when collected
    /// </summary>
    [SerializeField]
    float pickedupSpeed = 500f;

    /// <summary>
    /// True when this is picked up
    /// This is to help prevent the coroutine from starting more than once
    /// </summary>
    bool isItemCollected = false;

    /// <summary>
    /// Since these are non-physic based collectables
    /// </summary>
    void Update()
    {
        // Rotate
        Vector3 targetRotation = new Vector3(
            0f,
            this.rotationSpeed * Time.deltaTime,
            0f
        );

        this.transform.Rotate(targetRotation);
    }

    /// <summary>
    /// Triggers the "pickup" logic when the player enters into its triggers
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (this.isItemCollected) {
            return;
        }

        if(other.tag == "Player") {
            StartCoroutine(this.ItemCollected(other.gameObject.GetComponent<Player>()));
        }
    }

    /// <summary>
    /// Increases the player's total pickup
    /// Destroys itself
    /// 
    /// @TODO: play pickup animation
    /// </summary>
    IEnumerator ItemCollected(Player player)
    {
        yield return null;
        AudioManager.instance.PlaySound("CoinPickup");
        this.isItemCollected = true;
        player.CursedItemCollected();
        Destroy(this.gameObject);
    }
}
