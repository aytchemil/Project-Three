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

    void ToggleMenu()
    {
        print("toggle");
        on = !on;
        menu.SetActive(on);
        if (!on)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;


    }

    public void Quit()
    {
        Application.Quit();
    }

}
