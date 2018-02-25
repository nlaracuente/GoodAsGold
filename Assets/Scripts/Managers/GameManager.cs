using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages the state of the game, moving between scenes, and saving/loading player progress
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// A reference to the only instance of the GameManager
    /// </summary>
    public static GameManager instance = null;

    /// <summary>
    /// A reference to the player manager object
    /// </summary>
    PlayerManager m_player;

    /// <summary>
    /// A reference to the canvas image used for fading in/out
    /// </summary>
    [SerializeField]
    Image m_faderImage;

    /// <summary>
    /// How fast to fade the screen
    /// </summary>
    [SerializeField]
    float m_fadeSpeed = .3f;

    /// <summary>
    /// How close to the target the alpha needs to be when fading
    /// </summary>
    [SerializeField, Tooltip("Difference between current alpha to target to consider fading done")]
    float m_fadeTargetDiff = .001f;

    /// <summary>
    /// Alpha values for fading in/out
    /// </summary>
    float m_fadeInAlpha = 1f;
    float m_fadeOutAlpha = 0f;

    /// <summary>
    /// Has the level finishing setting up and is ready for the player to play?
    /// </summary>
    bool m_hasLevelLoaded = false;
    public bool HasLevelLoaded { get { return m_hasLevelLoaded; } set { m_hasLevelLoaded = value; } }

    /// <summary>
    /// Has the player died or reached the goal?
    /// </summary>
    bool m_isGameOver = false;
    public bool IsGameOver { get { return m_isGameOver; } set { m_isGameOver = value; } }

    /// <summary>
    /// Has the level finished cleaning up after game over?
    /// </summary>
    bool m_hasLevelUnloaded = false;
    public bool HasLevelUnloaded { get { return m_hasLevelUnloaded; } set { m_hasLevelUnloaded = value; } }

    /// <summary>
    /// Creates the GameManager instance
    /// </summary>
    void Awake()
    {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Starts the game loop
    /// </summary>
    void Start()
    {
        m_player = FindObjectOfType<PlayerManager>();

        if (m_player == null) {
            Debug.LogErrorFormat("GameManager Error: Missing Component! Player Manager = {0}", m_player);
        }

        RunGameloop();
    }

    /// <summary>
    /// Triggers the game loop routine
    /// </summary>
    void RunGameloop()
    {
        StartCoroutine(GameLoopRoutine());
    }
    
    /// <summary>
    /// Triggers a change in screen fader's alpha to match the target given
    /// </summary>
    /// <param name="targetAlpha"></param>
    IEnumerator FadeScreenRoutine(float targetAlpha)
    {
        Color faderColor = m_faderImage.color;

        while(Mathf.Abs(faderColor.a - targetAlpha) > m_fadeTargetDiff) {
            faderColor.a = Mathf.Lerp(
                faderColor.a,
                targetAlpha,
                m_fadeSpeed * Time.deltaTime
            );

            m_faderImage.color = faderColor;
            yield return null;
        }

        faderColor.a = targetAlpha;
        m_faderImage.color = faderColor;
    }

    /// <summary>
    /// Main Game loop routine
    /// </summary>
    /// <returns></returns>
    IEnumerator GameLoopRoutine()
    {
        yield return StartCoroutine(LoadLevelRoutine());
        yield return StartCoroutine(GameplayRoutine());
        yield return StartCoroutine(UnloadLevelRoutine());
    }

    /// <summary>
    /// Loads the player's progress
    /// Updates level to show player progress
    /// Fades the screen in
    /// Gives control to the player
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadLevelRoutine()
    {
        while (!m_hasLevelLoaded) {
            yield return StartCoroutine(FadeScreenRoutine(m_fadeOutAlpha));
            m_hasLevelLoaded = true;
        }
    }

    /// <summary>
    /// Waits for the player to lose or finish the level before
    /// </summary>
    /// <returns></returns>
    IEnumerator GameplayRoutine()
    {
        while (!m_player.IsDead) {
            yield return null;
        }
    }

    /// <summary>
    /// Triggers either a level won or level loss sequence
    /// Once those processes are done fades the screen to block
    /// Reloads scene
    /// </summary>
    /// <returns></returns>
    IEnumerator UnloadLevelRoutine()
    {
        while (!m_hasLevelUnloaded) {
            yield return StartCoroutine(FadeScreenRoutine(m_fadeInAlpha));
            m_hasLevelUnloaded = true;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
