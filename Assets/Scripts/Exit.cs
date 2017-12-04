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
    /// Initialize
    /// </summary>
    private void Start()
    {
        this.menu = FindObjectOfType<LevelMenu>();
    }

    /// <summary>
    /// Triggers a level won
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Player" || this.menu.isMenuOpened) {
            return;
        }
        
        other.GetComponent<Player>().PlayerVictory();
    }
}
