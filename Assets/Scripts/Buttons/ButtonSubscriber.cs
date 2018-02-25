using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Base class for all button subscriber 
/// A button subscriber contains a list of buttons that it subscribes to know when they are pressed or released
/// The actions of which determines when to trigger an OnPressedAction or OnReleasedAction so that things like
/// door or traps open/close or acticate/deactive themselves
/// </summary>
public abstract class ButtonSubscriber : MonoBehaviour
{
    /// <summary>
    /// Array of all the buttons this subscriber is registered to
    /// </summary>
    [SerializeField]
    [Tooltip("Buttons required to be ON/OFF to activate/deactive this object. \nThe order matters for sequence events")]
    protected List<Button> m_buttons;

    /// <summary>
    /// Delegation for triggering button actions such as activation/deactivate
    /// </summary>
    public delegate void ButtonSubscriberEvent();

    /// <summary>
    /// Triggered when all the buttons meet the conditions for this subscriber to be considered "active"
    /// </summary>
    public event ButtonSubscriberEvent OnActivate;

    /// <summary>
    /// Triggered when all the buttons no longer meet the conditions for this subscriber to be considered "active"
    /// </summary>
    public event ButtonSubscriberEvent OnDeactivate;

    /// <summary>
    /// Unity events that expose the OnActive trigger to easily assign more subscriber via the editor
    /// </summary>
    public UnityEvent OnActivateUnityEvent;

    /// <summary>
    /// Unity events that expose the OnActive trigger to easily assign more subscriber via the editor
    /// </summary>
    public UnityEvent OnDeactivateUnityEvent;

    /// <summary>
    /// Subscribe to all the buttons
    /// </summary>
    void Start()
    {
        OnStart();
    }

    /// <summary>
    /// Provides a way for objects to override the MonoBehaviour::Start()
    /// </summary>
    protected virtual void OnStart()
    {
        Subscribe();
    }

    /// <summary>
    /// Subscribes to all the buttons on pressed and on release events
    /// </summary>
    protected void Subscribe()
    {
        foreach (Button button in m_buttons) {
            if(button == null) {
                continue;
            }

            button.OnButtonPressed += OnButtonPressed;
            button.OnButtonReleased += OnButtonReleased;
        }
    }

    /// <summary>
    /// Unsubscribes to all the buttons on pressed and on release events
    /// </summary>
    protected void Unsubscribe()
    {
        foreach (Button button in m_buttons) {
            if (button == null) {
                continue;
            }

            button.OnButtonPressed -= OnButtonPressed;
            button.OnButtonReleased -= OnButtonReleased;
        }
    }

    /// <summary>
    /// Called when the button is pressed
    /// </summary>
    /// <param name="button"></param>
    /// <param name="other"></param>
    protected abstract void OnButtonPressed(Button button, GameObject other);

    /// <summary>
    /// Called when the button is released
    /// </summary>
    /// <param name="button"></param>
    /// <param name="other"></param>
    protected abstract void OnButtonReleased(Button button, GameObject other);

    /// <summary>
    /// Returns true if all the buttons in <see cref="m_buttons"/> are in an active status
    /// </summary>
    /// <returns></returns>
    protected virtual bool AllButtonsAreActive()
    {
        bool allButtonsAreActive = true;

        foreach (Button button in m_buttons) {
            if (!button.IsActive) {
                allButtonsAreActive = false;
                break;
            }
        }

        return allButtonsAreActive;
    }

    /// <summary>
    /// Triggers the OnActivate subscriber and unity event 
    /// when all the buttons in the list are active
    /// </summary>
    protected virtual void Activate()
    {
        if (AllButtonsAreActive()) {
            if(OnActivate != null) {
                OnActivate();
            }

            if(OnActivateUnityEvent != null) {
                OnActivateUnityEvent.Invoke();
            }
        }
    }

    /// <summary>
    /// Triggers the OnDeactivate subscriber and unity event event
    /// </summary>
    protected virtual void Deactivate()
    {
        if (OnDeactivate != null) {
            OnDeactivate();
        }

        if (OnDeactivateUnityEvent != null) {
            OnDeactivateUnityEvent.Invoke();
        }        
    }
}
