using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Activates the buttons on the list only once ignoring the "OnButtonReleased" event
/// </summary>
public class TriggerButtonSubscriber : ButtonSubscriber
{
    /// <summary>
    /// Triggers the button to be activated
    /// Unregisters from the buttons' OnButtonPress to prevent beign called again
    /// Invokes the Activate subscriber event
    /// </summary>
    /// <param name="button"></param>
    /// <param name="other"></param>
    protected override void OnButtonPressed(Button button, GameObject other)
    {
        button.Activate();
        button.OnButtonPressed -= OnButtonPressed;
        Activate();
    }

    /// <summary>
    /// Unregisters from the buttons' OnButtonReleased to prevent beign called again
    /// Does not deactivate the button
    /// </summary>
    /// <param name="button"></param>
    /// <param name="other"></param>
    protected override void OnButtonReleased(Button button, GameObject other)
    {
        button.OnButtonReleased -= OnButtonReleased;
    }
}
