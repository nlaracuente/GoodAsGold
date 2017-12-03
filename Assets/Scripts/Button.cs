using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for button type switches
/// </summary>
public abstract class Button : MonoBehaviour
{
    /// <summary>
    /// Color to display when the switch is active
    /// </summary>
    [SerializeField]
    Material activeMaterial;

    /// <summary>
    /// Color to display when the switch is not active
    /// </summary>
    [SerializeField]
    Material inactiveMaterial;

    /// <summary>
    /// A reference to the mesh renderer component
    /// </summary>
    [SerializeField]
    new MeshRenderer renderer;

    /// <summary>
    /// Delegates for enabling/disabling the switch
    /// </summary>
    /// <param name="value">Value of the object clicked</param>
    public delegate void OnButtonPressDelegate();
    public delegate void OnButtonReleaseDelegate();

    /// <summary>
    /// Events triggered when switch is enabled/disabled
    /// </summary>
    public event OnButtonPressDelegate OnButtonPressedEvent;
    public event OnButtonReleaseDelegate OnButtonReleasedEvent;

    /// <summary>
    /// Holds a reference to the object that trigger this button
    /// </summary>
    protected IMoveable activatedBy;
        
    /// <summary>
    /// While an IMoveable object is pressing this button we will mark it as active
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerStay(Collider other)
    {
        // Already have one 
        if(this.activatedBy != null) {
            return;
        }

        IMoveable moveable = other.GetComponent<IMoveable>();

        if(moveable != null) {
            this.activatedBy = moveable;
            this.OnButtonPressed();
        }
    }

    /// <summary>
    /// Checks if the object that left is the one that activated this button
    /// if so, then triggers a deactivation event
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerExit(Collider other)
    {
        // Don't have one - must not be active 
        if (this.activatedBy == null ) {
            return;
        }

        IMoveable moveable = other.GetComponent<IMoveable>();

        if (moveable != null && this.activatedBy == moveable) {
            this.activatedBy = null;
            this.OnButtonReleased();
        }
    }

    /// <summary>
    /// Registeres all the events for this buttons to the proper mapped event
    /// </summary>
    /// <param name="trigger"></param>
    public void RegisterButtonEvents(ITriggerable trigger)
    {
        this.OnButtonPressedEvent += trigger.OnButtonPressed;
        this.OnButtonReleasedEvent += trigger.OnButtonReleased;
    }

    /// <summary>
    /// Notifies all registered delegates the button is pressed
    /// Toggles material to show the button is pressed
    /// </summary>
    protected virtual void OnButtonPressed()
    {
        if(this.OnButtonPressedEvent != null) {
            this.OnButtonPressedEvent();
        }

        if(this.renderer != null && this.activeMaterial != null) {
            this.renderer.material = this.activeMaterial;
        }
    }

    /// <summary>
    /// Notifies all registered delegates the button has been released
    /// Toggles material to show the button has been released
    /// </summary>
    protected virtual void OnButtonReleased()
    {
        if (this.OnButtonReleasedEvent != null) {
            this.OnButtonReleasedEvent();
        }

        if (this.renderer != null && this.inactiveMaterial != null) {
            this.renderer.material = this.inactiveMaterial;
        }
    }
}
