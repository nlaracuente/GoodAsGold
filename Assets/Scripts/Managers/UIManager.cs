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
    /// A reference to the game object that contains the level menu items
    /// </summary>
    [SerializeField]
    GameObject m_menuGO;

    [SerializeField]
    GameObject m_moveArrowOne;

    [SerializeField]
    GameObject m_moveArrowTwo;

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
    /// Allows binding to the move arrow ui clicks
    /// </summary>
    public delegate void MoveArrowClicked();

    /// <summary>
    /// Arrow One is for Up/Left arrow
    /// Arrow Two is for Down/Right arrow
    /// </summary>
    public event MoveArrowClicked OnMoveArrowOneClicked;
    public event MoveArrowClicked OnMoveArrowTwoClicked;

    /// <summary>
    /// Sets references
    /// </summary>
    void Awake()
    {
        m_inputManager = FindObjectOfType<InputManager>();
        m_playerGO = GameObject.FindGameObjectWithTag("Player");

        if(m_menuGO == null) {
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

        HideArrows();
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
    public void ShowMoveArrow()
    {
        m_moveArrowOne.SetActive(true);
        m_moveArrowTwo.SetActive(true);
    }

    /// <summary>
    /// Disables the movve arrows
    /// </summary>
    public void HideArrows()
    {
        m_moveArrowOne.SetActive(false);
        m_moveArrowTwo.SetActive(false);
    }

    /// <summary>
    /// Sets the move arrows to display as looking "left" and "right"
    /// </summary>
    /// <param name="leftArrowPos"></param>
    /// <param name="rightArrowPos"></param>
    public void ShowHorizontalMoveArrows(Vector3 leftArrowPos, Vector3 rightArrowPos)
    {
        RotateMoveArrowsInDegrees(90f, -90f);
        SetMoveArrowsPosition(leftArrowPos, rightArrowPos);
        ShowMoveArrow();
    }

    public void ShowVerticalMoveArrows(Vector3 upArrowPos, Vector3 downArrowPos)
    {
        RotateMoveArrowsInDegrees(0, 180);
        SetMoveArrowsPosition(upArrowPos, downArrowPos);
        ShowMoveArrow();
    }

    /// <summary>
    /// Rotates the move arrows in the degrees specified
    /// </summary>
    /// <param name="arrowOneDegrees"></param>
    /// <param name="arrowTwoDegrees"></param>
    void RotateMoveArrowsInDegrees(float arrowOneDegrees, float arrowTwoDegrees)
    {
        //GameObject arrowOne = m_arrows[0];
        //GameObject arrowTwo = m_arrows[1];

        m_moveArrowOne.transform.rotation = Quaternion.Euler(Vector3.forward * arrowOneDegrees);
        m_moveArrowTwo.transform.rotation = Quaternion.Euler(Vector3.forward * arrowTwoDegrees);
    }

    /// <summary>
    /// Updates the move arrow position to the ones given
    /// Coordinates are transformed from world space to screen point
    /// </summary>
    /// <param name="arrowOneWorldPos"></param>
    /// <param name="arrowTwoWorldPos"></param>
    void SetMoveArrowsPosition(Vector3 arrowOneWorldPos, Vector3 arrowTwoWorldPos)
    {
        //GameObject arrowOne = m_arrows[0];
        //GameObject arrowTwo = m_arrows[1];

        RectTransform rectTransformOne = m_moveArrowOne.GetComponent<RectTransform>();
        RectTransform rectTransformTwo = m_moveArrowTwo.GetComponent<RectTransform>();

        rectTransformOne.position = m_mainCamera.WorldToScreenPoint(arrowOneWorldPos);
        rectTransformTwo.position = m_mainCamera.WorldToScreenPoint(arrowTwoWorldPos);
    }

    /// <summary>
    /// Called by the Move Arrow One ui button when clicked
    /// </summary>
    public void OnArrowOneClick()
    {
        if(OnMoveArrowOneClicked != null) {
            OnMoveArrowOneClicked();
        }
    }

    // <summary>
    /// Called by the Move Arrow One ui button when clicked
    /// </summary>
    public void OnArrowTwoClick()
    {
        if (OnMoveArrowTwoClicked != null) {
            OnMoveArrowTwoClicked();
        }
    }
}
