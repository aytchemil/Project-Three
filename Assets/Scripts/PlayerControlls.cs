using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CombatEntityController
{
    //Rule of thumb : Keep these such that they are always set on the outside, never during gameplay
    public PlayerInputActions controls;


    //Input Actions
    public InputAction ia_look;
    public InputAction ia_move;
    public InputAction ia_sprint;
    public InputAction ia_lockOn;
    public InputAction ia_dash;
    public InputAction ia_attack;
    public InputAction ia_block;

    private void Awake()
    {
        controls = new PlayerInputActions();

        //Input Cache
        ia_look = controls.Player.Look;
        ia_move = controls.Player.Move;
        ia_sprint = controls.Player.Sprint;
        ia_lockOn = controls.Player.LockOn;
        ia_dash = controls.Player.Dash;
        ia_attack = controls.Player.Attack;
        ia_block = controls.Player.Block;
    }


    #region enable/disable

    protected override void OnEnable()
    {
        ia_look.Enable();
        look = () => ia_look.ReadValue<Vector2>();

        ia_move.Enable();
        move = () => ia_move.ReadValue<Vector2>();

        ia_sprint.Enable();
        ia_sprint.started += ctx => sprintStart?.Invoke();
        ia_sprint.canceled += ctx => sprintStop?.Invoke();

        ia_lockOn.Enable();
        ia_lockOn.performed += ctx => lockOn?.Invoke();

        ia_dash.Enable();
        ia_dash.performed += ctx => dash?.Invoke();

        ia_attack.Enable();
        ia_attack.performed += ctx => attack?.Invoke();

        ia_block.Enable();
        ia_block.started += ctx => blockStart?.Invoke();
        ia_block.canceled += ctx => blockStop?.Invoke();

        base.OnEnable();
    }


    protected override void OnDisable()
    {
        ia_look.Disable();

        ia_move.Disable();

        ia_sprint.Disable();
        ia_sprint.started -= ctx => sprintStart?.Invoke();
        ia_sprint.canceled -= ctx => sprintStop?.Invoke();

        ia_lockOn.Disable();
        ia_lockOn.performed -= ctx => lockOn?.Invoke();

        ia_dash.Disable();
        ia_dash.performed -= ctx => dash?.Invoke();

        ia_attack.Disable();
        ia_attack.performed -= ctx => attack?.Invoke();

        ia_block.Disable();
        ia_block.performed -= ctx => blockStart?.Invoke();
        ia_block.canceled -= ctx => blockStop?.Invoke();

        base.OnDisable();
    }
    #endregion 



}
