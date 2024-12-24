using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CombatEntityController
{
    //Rule of thumb : Keep these such that they are always set on the outside, never during gameplay
    public PlayerInputActions controls;


    //Input Actions
    public InputAction look;
    public InputAction move;
    public InputAction sprint;
    public InputAction lockOn;
    public InputAction dash;
    public InputAction attack;
    public InputAction block;

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
        block = controls.Player.Block;
    }


    #region enable/disable

    private void OnEnable()
    {
        look.Enable();
        move.Enable();
        sprint.Enable();
        lockOn.Enable();
        dash.Enable();
        attack.Enable();
        block.Enable();
    }

    private void OnDisable()
    {
        look.Disable();
        move.Disable();
        sprint.Disable();
        lockOn.Disable();
        dash.Disable();
        attack.Disable();
        block.Disable();
    }
    #endregion 




}
