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
/// Manages the In-Game Menu 
/// </summary>
public class LevelMenuManager : MonoBehaviour
{
    /// <summary>
    /// A reference to the game object that contains the level menu items
    /// </summary>
    [SerializeField]
    GameObject m_menuGO;

    /// <summary>
    /// A reference to the input manager object
    /// </summary>
    InputManager m_inputManager;

    /// <summary>
    /// The current state the menu is at
    /// </summary>
    [SerializeField]
    MenuState m_state = MenuState.Closed;
    public MenuState State { get { return m_state; } }

    /// <summary>
    /// Sets references
    /// </summary>
    void Awake()
    {
        m_inputManager = FindObjectOfType<InputManager>();

        if(m_menuGO == null) {
            m_menuGO = GameObject.FindGameObjectWithTag("Menu");
        }

        if(m_inputManager == null || m_menuGO == null) {
            Debug.LogErrorFormat("LevelMenuManager Error: Missing Component: " +
                "InputManager = {0}, Menu GameObject = {1}",
                m_inputManager,
                m_menuGO
            );
        }
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
}
