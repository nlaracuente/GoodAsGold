using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
/// Manages the state of the game
/// Handles transitioning between scenes
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Name of the level scene
    /// </summary>
    [SerializeField]
    string levelName = "Level";

    /// <summary>
    /// Singleton
    /// </summary>
    public static GameManager instance = null;

    /// <summary>
    /// True when the game is completed
    /// </summary>
    public bool IsGameWon { get; set; }

    /// <summary>
    /// Stores the point where the player will respawn
    /// </summary>
    public Vector3 RespawnPoint { get; set; }

    public bool IsMenuOpened
    {
        get { return FindObjectOfType<LevelMenu>().isMenuOpened; }
    }
   
    /// <summary>
    /// Prevents more than once instance of the GameManager
    /// </summary>
    void Awake()
    {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// Closes the app
    /// </summary>
    public void Exit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    /// <summary>
    /// Tranistions to the first level
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene(this.levelName);
    }

    /// <summary>
    /// Takes the player back to the main menu
    /// </summary>
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}