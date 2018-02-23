using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A sequence button subscriber requires that all the buttons in the button list
/// be pressed in the order as they appear on the list. Breaking the sequence before 
/// completeing results in all the buttons being deactivated.
/// </summary>
public class SequenceButtonSubscriber : ButtonSubscriber
{
    /// <summary>
    /// Contains all the buttons that must be activated in the order they must be activated
    /// to trigger the Activate subscriber event
    /// </summary>
    Queue<Button> m_buttonQueue;

    /// <summary>
    /// Creates the queue
    /// </summary>
    protected override void OnStart()
    {
        base.OnStart();
        CreateQueue();
    }

    /// <summary>
    /// Converts the <see cref="m_buttons"/> list in the parent class into a queue
    /// </summary>
    void CreateQueue()
    {
        m_buttonQueue = new Queue<Button>(m_buttons);
    }

    /// <summary>
    /// Compares button pressed with the next button in queue
    /// if the two match then it activates the button
    /// else it deactivates all buttons and recreates the queue
    /// </summary>
    /// <param name="button"></param>
    /// <param name="other"></param>
    protected override void OnButtonPressed(Button button, GameObject other)
    {
        button.Activate();
        Button nextButton = m_buttonQueue.Dequeue();

        if(button == nextButton) {            
            Activate();
        } else {
            foreach (Button b in m_buttons) { b.Deactivate(); }
            CreateQueue();
        }
    }

    /// <summary>
    /// Sequence buttons are a one time activation and ignore deactivation triggers
    /// </summary>
    /// <param name="button"></param>
    /// <param name="other"></param>
    protected override void OnButtonReleased(Button button, GameObject other)
    {
        return;
    }
}
