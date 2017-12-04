using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple controller for handling the Main Menu options
/// </summary>
public class MainMenuController : MonoBehaviour
{
    /// <summary>
    /// A reference to the game won container to enable disable based on the game state
    /// </summary>
    [SerializeField]
    GameObject gameWonBG;
        
    /// <summary>
    /// Play title music
    /// </summary>
    void Update()
    {
        this.gameWonBG.SetActive(GameManager.instance.IsGameWon);
        AudioManager.instance.PlayMusic("TitleMusic");
    }

    /// <summary>
    /// Goes to the level scene
    /// </summary>
    public void StartGame()
    {
        GameManager.instance.StartGame();
    }

    /// <summary>
    /// Triggers application quit
    /// </summary>
    public void QuitGame()
    {
        GameManager.instance.Exit();
    }
}
