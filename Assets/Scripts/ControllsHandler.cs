using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllsHandler : MonoBehaviour
{
    public PlayerInputActions controls;

    //Action Delegates
    public Action EnterCombat;
    public Action ExitCombat;
    public Action<CombatEntity> CombatFollowTarget;

    public Action<string> UtilizeAbility;
    public Ability a_current;
    public Ability a_right;
    public Ability a_left;
    public Ability a_up;
    public Ability a_down;


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
