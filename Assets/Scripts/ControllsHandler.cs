using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllsHandler : MonoBehaviour
{
    public PlayerInputActions controls;

    //Action Delegates
    public Action<CombatEntity> EnterCombat;
    public Action<CombatEntity> ExitCombat;

    //Input Actions
    public InputAction look;
    public InputAction move;
    public InputAction sprint;
    public InputAction lockOn;
    public InputAction dash;

    private void Awake()
    {
        controls = new PlayerInputActions();

        //Input Cache
        look = controls.Player.Look;
        move = controls.Player.Move;
        sprint = controls.Player.Sprint;
        lockOn = controls.Player.LockOn;
        dash = controls.Player.Dash;


        lockOn.performed += ctx => TestLockOn();
    }


    #region enable/disable movement

    private void OnEnable()
    {
        look.Enable();
        move.Enable();
        sprint.Enable();
        lockOn.Enable();
        dash.Enable();
    }

    private void OnDisable()
    {
        look.Disable();
        move.Disable();
        sprint.Disable();
        lockOn.Disable();
        dash.Disable();
    }
    #endregion 

    void TestLockOn()
    {
        //Debug.Log("Lock on Pressed");
    }



}
