using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public static GameController S;

    private AudioSource music;

    public AdvancedMovementController playerRef;

    public GameObject titleGui;
    public GameObject buttonDisplay;

    public Text moveText;
    public Text jumpText;
    public Text controlsText;
    public Text helpText;

    public Canvas canvas;

    private void Awake()
    {
        if (S != null)
        {
            Destroy(gameObject);
        }
        else
        {
            S = this;
        }

        DontDestroyOnLoad(gameObject);
        music = GetComponent<AudioSource>();
    }

    private void Start()
    { 
        // initialize gui
        buttonDisplay.SetActive(false);
        helpText.enabled = false;
        moveText.color = Color.green;
        jumpText.color = Color.green;
        Cursor.visible = false;

    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && SceneManager.GetActiveScene().buildIndex == 0)
        {
            titleGui.SetActive(false);
            controlsText.enabled = false;
            helpText.enabled = true;
            LoadNextLevel();
            music.Play();
        }
        
        if (Input.GetButtonDown("Cancel"))
            QuitGame();
    }

    private void QuitGame()
    {
        // save any game data here
        #if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void LoadLevel(string name) {
        SceneManager.LoadScene(name);
    }

    public void LoadNextLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        moveText.color = Color.green;
        jumpText.color = Color.green;

        if (SceneManager.GetActiveScene().buildIndex + 1 == 8)
        {
            controlsText.enabled = false;
            helpText.enabled = false;
        }
    }

    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        moveText.color = Color.green;
        jumpText.color = Color.green;
    }
}
