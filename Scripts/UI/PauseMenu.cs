using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Panels")] [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject hudPanel;

    [SerializeField] private string mainMenuSceneString;

    private bool isPaused;
    private bool canPause;

    private void Awake()
    {
        canPause = true;
        isPaused = false;
    }

    private void Update()
    {
        // TEMP SOLUTION -> NEED TO UPDATE WITH NEW INPUT SYSTEM LATER
        if (Input.GetKeyDown(KeyCode.Escape))
            HandlePauseInput();
    }

    private void HandlePauseInput()
    {
        if (!isPaused && canPause)
            PauseGame();
        else if (isPaused && canPause)
            ResumeGame();
    }

    public void PauseGame()
    {
        // set pausing bool, ADD CALL TO OSKAR'S PAUSE STATE LATER
        isPaused = true;

        // set time scale to 0 for pausing
        //Time.timeScale = 0f;

        // set the panels correctly
        hudPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        // set pausing bool, ADD CALL TO OSKAR'S PAUSE STATE LATER
        isPaused = false;

        // set time scale to 1 for unpausing
        //Time.timeScale = 1f;

        // set the panels correctly
        hudPanel.SetActive(true);
        pausePanel.SetActive(false);
    }

    // public void RestartGame()
    // {
    //     SceneManager.LoadScene(StringManager.GAMEPLAY_SCENE_TAG);
    // }

    // public void GameOver()
    // {
    //     canPause = false;
    //     Time.timeScale = 0f;
    //     gameOverPanel.SetActive(true);
    //     gameplayPanel.SetActive(false);
    // }

    public void QuitToMainMenu()
    {
        _=SceneTransition.TransitionScene(0);
    }
}
