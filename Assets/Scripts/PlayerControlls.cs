using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CombatEntityController
{
    public PlayerInputActions controls;


    //Input Actions
    public InputAction look;
    public InputAction move;
    public InputAction sprint;
    public InputAction lockOn;
    public InputAction dash;
    public InputAction attack;

    private void Awake()
    {
        controls = new PlayerInputActions();

        //Input Cache
        look = controls.Player.Look;
        move = controls.Player.Move;
        sprint = controls.Player.Sprint;
        lockOn = controls.Player.LockOn;
        dash = controls.Player.Dash;
        attack = controls.Player.Attack;
    }


    #region enable/disable movement

    private void OnEnable()
    {
        look.Enable();
        move.Enable();
        sprint.Enable();
        lockOn.Enable();
        dash.Enable();
        attack.Enable();
    }

    private void OnDisable()
    {
        look.Disable();
        move.Disable();
        sprint.Disable();
        lockOn.Disable();
        dash.Disable();
        attack.Disable();
    }
    #endregion 




}
