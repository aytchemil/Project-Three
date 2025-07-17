using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMenu : MonoBehaviour
{
    PlayerInputActions ia;
    PlayerController controller;

    InputAction ia_menu;
    Action MenuToggle;
    public GameObject menu;
    public bool on = false;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        ia = controller.controls;

        ia_menu = ia.Player.OpenMenu;

    }

    private void OnEnable()
    {
        ia_menu.Enable();
        ia_menu.performed += ctx =>
        {
            print("pressed esc");
            MenuToggle?.Invoke();
        };
        MenuToggle += ToggleMenu;

    }

    private void OnDisable()
    {
        ia_menu.Disable();
        ia_menu.performed -= ctx => MenuToggle?.Invoke();
        MenuToggle -= ToggleMenu;

    }

    private void LateUpdate()
    {
        if (!on)
            ToggleOff();
        else
            ToggleOn();
    }

    void ToggleMenu()
    {
        print("toggle");
        on = !on;
    }

    void ToggleOn()
    {
        print("paused");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
        menu.SetActive(on);
    }

    void ToggleOff()
    {
        print("unpaused");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
        menu.SetActive(on);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
