using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/// <summary>
/// Controls when the menu is opened/closed
/// The text that is displayed
/// As well as reloading the level on button press
/// </summary>
public class LevelMenu : MonoBehaviour
{
    /// <summary>
    /// The game object that holds the container
    /// </summary>
    [SerializeField]
    GameObject menuContainer;

    /// <summary>
    /// A reference to the UI Text componenent where the description
    /// of the current play state is on
    /// </summary>
    [SerializeField]
    Text description;

    /// <summary>
    /// A reference to the UI Text componenent where curse percentage is recorded
    /// </summary>
    [SerializeField]
    Text cursedPercent;

    /// <summary>
    /// A reference to the menu button to change its text
    /// Or make it disappear
    /// </summary>
    [SerializeField]
    Button menuButton;

    /// <summary>
    /// Text to display when menu is opened in game
    /// </summary>
    [SerializeField]
    string levelText = "Do not lose hope great adventurer for freedom is near";

    /// <summary>
    /// Text to display when menu is opened in game
    /// </summary>
    [SerializeField]
    string gameOverText = "You have been consumed by the cursed and bound to live eternity as a statue of gold. Perhaps you will be of used to a future explorer...";

    /// <summary>
    /// Text to display when menu is opened in game
    /// </summary>
    [SerializeField]
    string gameWonText = "You have managed the unthinkable and restraint yourself from the richest of this world. Now, go brave adventurer, your freedom awaits or dare you try again?";

    /// <summary>
    /// True while the menu is opened
    /// </summary>
    public bool isMenuOpened = false;

    /// <summary>
    /// True when the level is over either because the player won or lost
    /// This is to disable the menu altogether
    /// </summary>
    public bool disableMenu = false;

    /// <summary>
    /// A reference to the player script
    /// </summary>
    Player player;

    /// <summary>
    /// Initialize
    /// </summary>
    void Start()
    {
        this.player = FindObjectOfType<Player>();
    }

    /// <summary>
    /// Toggle Menu On/Off on player input
    /// </summary>
    void Update ()
    {
        if (this.disableMenu || this.player.IsDead || this.player.IsDisabled) {
            return;
        }

        bool isButtonDown = Input.GetKeyDown(KeyCode.Escape);

        if(isButtonDown && !this.isMenuOpened) {
            this.OpenLevelMenu();

        } else if (isButtonDown && this.isMenuOpened) {
            this.CloseMenu();
        }
    }

    /// <summary>
    /// Updates the text that display the curse level to show the current level
    /// </summary>
    void UpdateCursedText()
    {
        this.cursedPercent.text = "Cursed is at: " + this.player.CursePercent + "%";
    }
    
    /// <summary>
    /// Updates the text on the menu button
    /// </summary>
    /// <param name="buttonText"></param>
    void SetMenuButtonText(string buttonText)
    {
        this.menuButton.GetComponentInChildren<Text>().text = buttonText;
    }

    /// <summary>
    /// Opens the level menu displaying current status
    /// This is neither the win or lose state
    /// </summary>
    void OpenLevelMenu()
    {
        this.isMenuOpened = true;
        this.description.text = this.levelText;
        this.SetMenuButtonText("Restart");
        this.UpdateCursedText();
        this.menuContainer.SetActive(true);
    }

    /// <summary>
    /// Opens the game over menu 
    /// </summary>
    public void GameOverMenu()
    {
        this.isMenuOpened = true;
        this.player.IsDisabled = true;
        this.description.text = this.gameOverText;
        this.SetMenuButtonText("Retry");
        this.UpdateCursedText();
        this.menuContainer.SetActive(true);
    }

    /// <summary>
    /// Opens the game won menu 
    /// </summary>
    public void GameWonMenu()
    {
        this.player.IsDisabled = true;
        this.isMenuOpened = true;
        this.description.text = this.gameWonText;
        this.SetMenuButtonText("Again!");
        this.UpdateCursedText();
        this.menuContainer.SetActive(true);
    }

    /// <summary>
    /// Closes the menu
    /// </summary>
    void CloseMenu()
    {
        this.isMenuOpened = false;
        this.menuContainer.SetActive(false);
    }

    /// <summary>
    /// Reloads the current scene
    /// @TODO: update to load scene based on menu state
    ///     - current
    ///     - main menu
    ///     - next
    /// </summary>
    public void OnReloadButtonPressed()
    {
        Debug.Log("Reload");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
