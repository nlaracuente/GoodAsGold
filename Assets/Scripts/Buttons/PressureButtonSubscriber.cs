using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A pressure button requires that an object is continually pressing it to be considred "active"
/// </summary>
public class PressureButtonSubscriber : ButtonSubscriber
{
    /// <summary>
    /// Triggers the button to activate and invokes the activate event
    /// </summary>
    /// <param name="other"></param>
    protected override void OnButtonPressed(Button button, GameObject other)
    {
        button.Activate();
        Activate();
    }

    /// <summary>
    /// Triggers the button to deactivate and invokes the deactivate event
    /// </summary>
    /// <param name="button"></param>
    /// <param name="other"></param>
    protected override void OnButtonReleased(Button button, GameObject other)
    {
        button.Deactivate();
        Deactivate();
    }
}
