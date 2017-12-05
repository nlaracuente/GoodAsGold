using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Receives notifications from the player animation controller
/// to play certains sounds based on the animation playing
/// </summary>
[RequireComponent(typeof(Player))]
public class PlayerSoundEffects : MonoBehaviour
{
    /// <summary>
    /// A reference to the parent Player script
    /// this is to know the current cursed percent to change certain sounds
    /// </summary>
    [SerializeField]
    Player player;
    
    /// <summary>
    /// Plays the sound clip for the given "leg" based on the 
    /// player's current cursed precentage
    /// </summary>
    /// <param name="legName"></param>
    public void FootStep(string legName)
    {
        string clipName = legName;

        if(this.player.CursePercent < 25f) {
            clipName += "NormalStep";
        } else if (this.player.CursePercent < 50f) {
            clipName += "HeavyStep";
        } else {
            clipName += "GoldStep";
        }

        AudioManager.instance.PlaySound(clipName);
    }

    /// <summary>
    /// Statue being pushed or pulled
    /// </summary>
    public void DragStatue()
    {
        AudioManager.instance.PlaySound("DragStatue");
    }

    public void GameOver()
    {
        AudioManager.instance.PlaySound("GameOver");
    }

    public void Victory()
    {
        AudioManager.instance.PlaySound("Victory");
    }
}
