using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    private AudioSource music;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        music = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && SceneManager.GetActiveScene().buildIndex == 0)
        {
            LoadNextLevel();
            music.Play();
        }
    }

    public void LoadLevel(string name) {
        SceneManager.LoadScene(name);
    }

    public void LoadNextLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
