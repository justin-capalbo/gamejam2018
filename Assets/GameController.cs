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
    }

    public void LoadLevel(string name) {
        SceneManager.LoadScene(name);
    }

    public void LoadNextLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        moveText.color = Color.green;
        jumpText.color = Color.green;
    }

    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        moveText.color = Color.green;
        jumpText.color = Color.green;
    }
}
