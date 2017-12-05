using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Triggers a level completed when the player touches it
/// </summary>
public class Exit : MonoBehaviour
{
    /// <summary>
    /// A reference to the level menu
    /// </summary>
    LevelMenu menu;

    /// <summary>
    /// True when triggered 
    /// </summary>
    bool winStateTriggered = false;

    /// <summary>
    /// Initialize
    /// </summary>
    private void Start()
    {
        this.menu = FindObjectOfType<LevelMenu>();
    }

    /// <summary>
    /// Triggers a level won
    /// Snaps the player on the center of the tile
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Player" || this.menu.isMenuOpened || this.winStateTriggered) {
            return;
        }

        this.winStateTriggered = true;
        Player player = other.GetComponent<Player>();
        player.PlayerVictory();
        player.transform.position = new Vector3(
            this.transform.position.x,
            player.transform.position.y,
            this.transform.position.z
        );
    }
}
