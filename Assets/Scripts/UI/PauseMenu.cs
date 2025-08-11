using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public bool isGamePaused = false;
    public Action PauseAction;
    public GameObject pauseMenuUI;
    public PlayerController playerController;

    private void OnEnable()
    {
        PauseAction += PauseWasPressed;
        pauseMenuUI.SetActive(false);
    }

    private void OnDisable()
    {
        PauseAction -= PauseWasPressed;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            PauseAction?.Invoke();

        if (isGamePaused)
            Pause();
    }

    public void PauseWasPressed()
    {
        Debug.LogWarning("Pressing Pause");
        if (!isGamePaused)
            isGamePaused = true;

    }


    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        isGamePaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerController.controls.Player.Enable();

        if (Cursor.visible == true)
            Resume();
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        isGamePaused = true;
        playerController.controls.Player.Disable();
    }

    public void Quit()
    {
        Debug.LogWarning("Quitting");
        Application.Quit();
    }
}
