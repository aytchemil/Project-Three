using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CombatEntityController
{
    //Rule of thumb : Keep these such that they are always set on the outside, never during gameplay
    public PlayerInputActions controls;

    [Header("Input Actions")]
    //Input Actions
    public InputAction ia_look;
    public InputAction ia_move;
    public InputAction ia_sprint;
    public InputAction ia_lockOn;
    public InputAction ia_dash;
    public InputAction ia_useAbility;
    public InputAction ia_useCombo;
    public InputAction ia_block;
    public InputAction ia_switchAttackMode;
    [Space]
    public InputAction[] ia_abilities;

    protected override void Awake()
    {
        ia_abilities = new InputAction[AMOUNT_OF_ABIL_SLOTS];
        controls = new PlayerInputActions();

        //Input Cache
        ia_look = controls.Player.Look;
        ia_move = controls.Player.Move;
        ia_sprint = controls.Player.Sprint;
        ia_lockOn = controls.Player.LockOn;
        ia_dash = controls.Player.Dash;
        ia_useAbility = controls.Player.UseAbility;
        ia_useCombo = controls.Player.UseCombo;
        ia_block = controls.Player.Block;
        ia_switchAttackMode = controls.Player.SwitchMode;

        ia_abilities[0] = controls.Player.Ability1;
        ia_abilities[1] = controls.Player.Ability2;
        ia_abilities[2] = controls.Player.Ability3;
        ia_abilities[3] = controls.Player.Ability4;
    }


    #region enable/disable

    protected override void OnEnable()
    {
        base.OnEnable();

        ia_look.Enable();
        look = () => ia_look.ReadValue<Vector2>();
        //print("e");
        ia_move.Enable();
        move = () => ia_move.ReadValue<Vector2>();

        ia_sprint.Enable();
        ia_sprint.started += ctx => sprintStart?.Invoke();
        ia_sprint.canceled += ctx => sprintStop?.Invoke();

        ia_lockOn.Enable();
        ia_lockOn.performed += ctx => lockOn?.Invoke();

        ia_dash.Enable();
        ia_dash.performed += ctx => dash?.Invoke();

        ia_useAbility.Enable();
        ia_useAbility.performed += ctx =>
        {
            useAbility?.Invoke(mode);
        };

        ia_useCombo.Enable();
        ia_useCombo.performed += ctx =>
        {
            usedCombo = true;
            useAbility?.Invoke("Combo");
        };

        ia_block.Enable();
        ia_block.started += ctx => blockStart?.Invoke();
        ia_block.canceled += ctx => blockStop?.Invoke();

        ia_switchAttackMode.Enable();
        ia_switchAttackMode.performed += ctx =>
        {
            switchAttackMode?.Invoke();
        };


        for(int i = 0; i < AMOUNT_OF_ABIL_SLOTS; i++)
        {
            print($"[PlayerControls] Enabled Ability Slot [{i}]");
            ia_abilities[i].Enable();
        }

        ia_abilities[0].performed += ctx =>
        {
            print($"[PlayerControlls] pressed ability [{0}]");
            abilitySlots[0]?.Invoke(0);
        };
        ia_abilities[1].performed += ctx =>
        {
            print($"[PlayerControlls] pressed ability [{1}]");
            abilitySlots[1]?.Invoke(1);
        };
        ia_abilities[2].performed += ctx =>
        {
            print($"[PlayerControlls] pressed ability [{2}]");
            abilitySlots[2]?.Invoke(2);
        };
        ia_abilities[3].performed += ctx =>
        {
            print($"[PlayerControlls] pressed ability [{3}]");
            abilitySlots[3]?.Invoke(3);
        };

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

        ia_useAbility.Disable();
        ia_useAbility.performed -= ctx => useAbility?.Invoke(mode);

        ia_useCombo.Disable();
        ia_useCombo.performed -= ctx => useAbility?.Invoke("Combo");

        ia_block.Disable();
        ia_block.performed -= ctx => blockStart?.Invoke();
        ia_block.canceled -= ctx => blockStop?.Invoke();

        ia_switchAttackMode.Disable();
        ia_switchAttackMode.performed -= ctx => switchAttackMode?.Invoke();

        for (int i = 0; i < AMOUNT_OF_ABIL_SLOTS; i++)
        {
            print($"[PlayerControls] Disabled Ability Slot [{i}]");
            ia_abilities[i].Disable();
        }

        ia_abilities[0].performed -= ctx =>
        {
            print($"[PlayerControlls] pressed ability [{0}]");
            abilitySlots[0]?.Invoke(0);
        };
        ia_abilities[1].performed -= ctx =>
        {
            print($"[PlayerControlls] pressed ability [{1}]");
            abilitySlots[1]?.Invoke(1);
        };
        ia_abilities[2].performed -= ctx =>
        {
            print($"[PlayerControlls] pressed ability [{2}]");
            abilitySlots[2]?.Invoke(2);
        };
        ia_abilities[3].performed -= ctx =>
        {
            print($"[PlayerControlls] pressed ability [{3}]");
            abilitySlots[3]?.Invoke(3);
        };

        base.OnDisable();
    }
    #endregion 


}
