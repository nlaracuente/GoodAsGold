using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Functions as a proxy to notify the parent Player script of 
/// events triggered by the animator controller
/// </summary>
public class PlayerAvatarModel : MonoBehaviour
{
    /// <summary>
    /// A reference to the parent Player script
    /// </summary>
    Player player;

	/// <summary>
    /// Initialize
    /// </summary>
	void Start ()
    {
        this.player = GetComponentInParent<Player>();
	}

    /// <summary>
    /// Invoked by the pushing/pulling animation when it is done
    /// </summary>
    public void ActionAnimationCompleted()
    {
        this.player.IsActionCompleted = true;
    }

    /// <summary>
    /// Invoked by the pushing/pulling animation to notify that
    /// the player/staue can now move
    /// </summary>
    public void DoTheAction()
    {
        this.player.DoTheAction = true;
    }
}
