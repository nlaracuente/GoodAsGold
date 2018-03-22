using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The different states the menu could be in
/// </summary>
public enum MenuState
{
    Closed,
    Opened,
}

/// <summary>
/// Manages all level ui such as menus and hud
/// </summary>
public class UIManager : MonoBehaviour
{

    /// <summary>
    /// A reference to self
    /// </summary>
    public static UIManager instance;

    /// <summary>
    /// A reference to the game object that contains the level menu items
    /// </summary>
    [SerializeField]
    GameObject m_menuGO;

    /// <summary>
    /// A reference to the d-pad's up arrow
    /// </summary>
    [SerializeField]
    GameObject m_dPadUpArrow;

    /// <summary>
    /// A reference to the d-pad's down arrow
    /// </summary>
    [SerializeField]
    GameObject m_dPadDownArrow;

    /// <summary>
    /// A reference to the d-pad's left arrow
    /// </summary>
    [SerializeField]
    GameObject m_dPadLeftArrow;

    /// <summary>
    /// A reference to the d-pad's right arrow
    /// </summary>
    [SerializeField]
    GameObject m_dPadRightArrow;

    /// <summary>
    /// The two current active arrows
    /// </summary>
    GameObject m_activeDPadArrowOne;
    GameObject m_activeDPadArrowTwo;

    /// <summary>
    /// A reference to the input manager object
    /// </summary>
    InputManager m_inputManager;

    /// <summary>
    /// A reference to the player game object
    /// </summary>
    GameObject m_playerGO;

    /// <summary>
    /// A reference to the main camera
    /// </summary>
    Camera m_mainCamera;

    /// <summary>
    /// The current state the menu is at
    /// </summary>
    [SerializeField]
    MenuState m_state = MenuState.Closed;
    public MenuState State { get { return m_state; } }

    /// <summary>
    /// A reference to the action ui button which triggers request such as "grab or release"
    /// </summary>
    ActionButton m_actionButton;

    /// <summary>
    /// Allows binding to the move arrow ui clicks
    /// </summary>
    public delegate void DPadArrowClicked();

    /// <summary>
    /// Arrow One is for Up/Left arrow
    /// Arrow Two is for Down/Right arrow
    /// </summary>
    public event DPadArrowClicked OnDPadArrowOneClicked;
    public event DPadArrowClicked OnDPadArrowTwoClicked;

    /// <summary>
    /// Sets references
    /// </summary>
    void Awake()
    {
        instance = this;
        m_inputManager = FindObjectOfType<InputManager>();
        m_playerGO = GameObject.FindGameObjectWithTag("Player");
        m_actionButton = FindObjectOfType<ActionButton>();

        if (m_menuGO == null) {
            m_menuGO = GameObject.FindGameObjectWithTag("Menu");
        }

        if(m_mainCamera == null) {
            m_mainCamera = Camera.main;
        }

        if(m_inputManager == null || m_menuGO == null || m_playerGO == null || m_mainCamera == null) {
            Debug.LogErrorFormat("LevelMenuManager Error: Missing Component: " +
                "InputManager = {0}, Menu GameObject = {1}, Player GO = {2}, Main Camera = {3}",
                m_inputManager,
                m_menuGO,
                m_playerGO,
                m_mainCamera
            );
        }
        
        HideGrabButton();
    }

    /// <summary>
    /// 
    /// </summary>
    public void ShowGrabButton(Moveable moveable)
    {
        m_actionButton.ShowGrabAction(moveable);
    }

    /// <summary>
    /// 
    /// </summary>
    public void HideGrabButton()
    {
        m_actionButton.HideButton();
    }

    /// <summary>
    /// Toggles between opened and closed menu
    /// </summary>
    public void ToggleMenu()
    {
        switch (m_state) {
            case MenuState.Closed:
                OpenMenu();
                break;
            case MenuState.Opened:
                CloseMenu();
                break;
        }
    }

    /// <summary>
    /// Opens the menu, disables player input for controlling the avatar, and pauses the game
    /// </summary>
	public void OpenMenu()
    {
        // Disable input immedeatly to cancel movement
        m_inputManager.DisableInput = true;
        m_menuGO.SetActive(true);        
        Time.timeScale = 0;
        m_state = MenuState.Opened;
    }

    /// <summary>
    /// Closes the menu, re-enables player input for controlling the avatar, and unpauses the game
    /// </summary>
    public void CloseMenu()
    {
        m_menuGO.SetActive(false);
        Time.timeScale = 1;

        // Wait until the menu is hidden to prevent unwanted movement because of a button press
        m_inputManager.DisableInput = false;
        m_state = MenuState.Closed;
    }

    /// <summary>
    /// Enables the move arrows
    /// </summary>
    public void ShowActiveDPadArrows()
    {
        m_activeDPadArrowOne.SetActive(true);
        m_activeDPadArrowTwo.SetActive(true);
    }

    /// <summary>
    /// Disables the movve arrows
    /// </summary>
    public void HideArrows()
    {
        m_activeDPadArrowOne.SetActive(false);
        m_activeDPadArrowTwo.SetActive(false);
    }

    /// <summary>
    /// /// Sets the current active dpad arrows to be the left/right arrows
    /// </summary>
    /// <param name="leftArrowPos"></param>
    /// <param name="rightArrowPos"></param>
    public void ShowHorizontalMoveArrows()
    {
        m_activeDPadArrowOne = m_dPadLeftArrow;
        m_activeDPadArrowTwo = m_dPadRightArrow;
        ShowActiveDPadArrows();
    }

    /// <summary>
    /// Sets the current active dpad arrows to be the up/down arrows
    /// </summary>
    /// <param name="upArrowPos"></param>
    /// <param name="downArrowPos"></param>
    public void ShowVerticalMoveArrows()
    {
        m_activeDPadArrowOne = m_dPadUpArrow;
        m_activeDPadArrowTwo = m_dPadDownArrow;
        ShowActiveDPadArrows();
    }

    /// <summary>
    /// Called by the Move Arrow One ui button when clicked
    /// </summary>
    public void OnArrowOneClick()
    {
        if(OnDPadArrowOneClicked != null) {
            OnDPadArrowOneClicked();
        }
    }

    // <summary>
    /// Called by the Move Arrow One ui button when clicked
    /// </summary>
    public void OnArrowTwoClick()
    {
        if (OnDPadArrowTwoClicked != null) {
            OnDPadArrowTwoClicked();
        }
    }
}
