using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A timer button subscriber fires off a timer routine to deactivate the button pressed
/// with the option of turning off the timer when certain conditions are met such as "all buttons pressed"
/// </summary>
public class TimerButtonSubscriber : ButtonSubscriber
{
    /// <summary>
    /// How long, in seconds, to keep the button active
    /// </summary>    
    [SerializeField, Tooltip("How many seconds to keep the door opened")]
    float m_seconds = 1f;

    /// <summary>
    /// Triggers the button to be active
    /// Triggers the timer routine to deactivate the button after time has passed
    /// Invokes the Activate subscriber event
    /// </summary>
    /// <param name="button"></param>
    /// <param name="other"></param>
    protected override void OnButtonPressed(Button button, GameObject other)
    {
        button.Activate();
        Activate();
        StartCoroutine(TimerRoutine(button));        
    }

    /// <summary>
    /// Deactivation is controlled by a timer therefore we do nothing here
    /// </summary>
    /// <param name="button"></param>
    /// <param name="other"></param>
    protected override void OnButtonReleased(Button button, GameObject other)
    {
        return;
    }

    /// <summary>
    /// Waits for <see cref="m_seconds"/> to pass before telling the button to deactivate
    /// as well as trigger a deactivate
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    IEnumerator TimerRoutine(Button button)
    {
        yield return new WaitForSeconds(m_seconds);
        button.Deactivate();
        Deactivate();
    }
}
