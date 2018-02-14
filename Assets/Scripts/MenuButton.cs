using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Menu button that opens/closes the level menu
/// </summary>
public class MenuButton : MonoBehaviour
{
    /// <summary>
    /// A reference to the level menu manager object
    /// </summary>
    LevelMenuManager m_menuManager;

    /// <summary>
    /// A reference to the image component for this button
    /// </summary>
    [SerializeField]
    Image m_buttonImage;

    /// <summary>
    /// Sprite to show when button will open the menu
    /// </summary>
    [SerializeField, Tooltip("Open Menu Icon")]
    Sprite m_openMenuSprite;

    /// <summary>
    /// Sprite to show when button will close the menu
    /// </summary>
    [SerializeField, Tooltip("Close Menu Icon")]
    Sprite m_closeMenuSprite;

    /// <summary>
    /// Sets references
    /// </summary>
    void Awake()
    {
        m_menuManager = FindObjectOfType<LevelMenuManager>();

        if (m_menuManager == null || m_buttonImage == null || m_openMenuSprite == null || m_closeMenuSprite == null) {
            Debug.LogErrorFormat("MenuButton Error: Missing Component: " +
               "MenuManager = {0}, Button Image = {1}, Open Menu Icon = {2}, Close Menu Icon = {3}",
               m_menuManager, m_buttonImage, m_openMenuSprite, m_closeMenuSprite
           );
        }
    }

    /// <summary>
    /// Always start with the menu closed
    /// </summary>
    void Start()
    {
        m_menuManager.CloseMenu();
        m_buttonImage.sprite = m_openMenuSprite;
    }

    /// <summary>
    /// Toggles menu on button clicked
    /// Updates the sprite to indicate the action the button will perform next time it is clicked
    /// </summary>
    public void OnButtonClicked()
    {
        m_menuManager.ToggleMenu();

        switch (m_menuManager.State) {
            case MenuState.Closed:
                m_buttonImage.sprite = m_openMenuSprite;
                break;
            case MenuState.Opened:
                m_buttonImage.sprite = m_closeMenuSprite;
                break;
        }
    }
}
