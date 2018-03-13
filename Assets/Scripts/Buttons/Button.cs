using UnityEngine;

/// <summary>
/// A button is only aware of itself dispatching events to subscribers of its OnPressed/OnReleased events
/// A button also handles updating it physical state and triggering sound
/// Additionally, buttons can be manipulated to be active or inactive by its listenwea
/// </summary>
public class Button : MonoBehaviour
{
    /// <summary>
    /// A reference to the gameobject that represent button's active state
    /// </summary>
    [SerializeField]
    GameObject m_activeButton;

    /// <summary>
    /// A reference to the gameobject that represent button's inactive state
    /// </summary>
    [SerializeField]
    GameObject m_inactiveButton;

    /// <summary>
    /// True while the button is active
    /// </summary>
    bool m_isActive = false;
    public bool IsActive
    {
        get { return m_isActive; }
        set {
            m_isActive = value;
            // TODO: Play active/deactivate sound
        }
    }

    /// <summary>
    /// Button event delegattion
    /// Both the button and the object that interacted with are passed in as arguments
    /// </summary>
    public delegate void ButtonEvent(Button button, GameObject other);

    /// <summary>
    /// Triggered when something is pressing this button
    /// </summary>
    public event ButtonEvent OnButtonPressed;

    /// <summary>
    /// Triggered when that something is no longer pressing the button
    /// </summary>
    public event ButtonEvent OnButtonReleased;

    /// <summary>
    /// Defaults the button to inactive
    /// </summary>
    void Awake()
    {
        if (m_activeButton == null || m_inactiveButton == null) {
            Debug.LogErrorFormat("Button Error: Missing Component! " +
                "Active Button = {0}, Inactive Button = {1}",
                m_activeButton,
                m_inactiveButton
            );
        }
    }

    /// <summary>
    /// Dispatches the OnButtonPressed event if the object in question is something that can interat with this
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IButtonInteractible>() != null && OnButtonPressed != null) {
            // Ignore if already active
            if (!m_isActive) {
                OnButtonPressed(this, other.gameObject);
            }
        }
    }

    /// <summary>
    /// Triggers the button to deactivate
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<IButtonInteractible>() != null && OnButtonReleased != null) {
            OnButtonReleased(this, other.gameObject);
        }
    }

    /// <summary>
    /// Sets the button to active state
    /// </summary>
    public void Activate()
    {
        m_isActive = true;
        m_inactiveButton.SetActive(false);
        m_activeButton.SetActive(true);
    }

    /// <summary>
    /// Sets the button to an inactive state
    /// </summary>
    public void Deactivate()
    {
        m_isActive = false;
        m_inactiveButton.SetActive(true);
        m_activeButton.SetActive(false);
    }
}
