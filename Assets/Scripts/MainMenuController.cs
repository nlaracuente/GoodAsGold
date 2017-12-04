using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Simple controller for handling the Main Menu options
/// </summary>
public class MainMenuController : MonoBehaviour
{
    /// <summary>
    /// The name of the level to load
    /// </summary>
    [SerializeField]
    string levelName = "Level";

    /// <summary>
    /// Goes to the level scene
    /// </summary>
	public void StartGame()
    {
        SceneManager.LoadScene(this.levelName);
    }

    /// <summary>
    /// Triggers application quit
    /// </summary>
    public void QuitGame()
    {
        #if UNITY_EDITOR
        Debug.Log("Application Quit");
        #endif
        Application.Quit();
    }
}
