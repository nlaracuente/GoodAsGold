using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When the player enters the tile, it stores the position
/// where to respawn the player and cleanses the player
/// </summary>
public class CleansingTile : MonoBehaviour
{
    /// <summary>
    /// A reference to the particle emitter
    /// </summary>
    [SerializeField]
    ParticleSystem particles;

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player") {
            Player player = other.GetComponent<Player>();
            player.LiftCurse();
            GameManager.instance.RespawnPoint = new Vector3(
                this.transform.position.x,
                player.transform.position.y,
                this.transform.position.z
            );

            AudioManager.instance.PlaySound("Healed");
            this.particles.Play();
        }
    }
}
