using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : EntityController
{
    //Rule of thumb : Keep these such that they are always set on the outside, never during gameplay
    public PlayerInputActions controls;

    [BoxGroup("Player Inputs")] [Header("Input Actions")]
    //Input Actions
    [BoxGroup("Player Inputs")] public InputAction ia_look;
    [BoxGroup("Player Inputs")] public InputAction ia_move;
    [BoxGroup("Player Inputs")] public InputAction ia_sprint;
    [BoxGroup("Player Inputs")] public InputAction ia_lockOn;
    [BoxGroup("Player Inputs")] public InputAction ia_dash;
    [BoxGroup("Player Inputs")] public InputAction ia_useCombo;
    [BoxGroup("Player Inputs")] public InputAction ia_block;
    [BoxGroup("Player Inputs")] public InputAction ia_switchAttackMode;
    [Space]
    [BoxGroup("Player Inputs")] public InputAction[] ia_abilityUse;

    public override void Init(List<ModeData> _modes)
    {
        base.Init(_modes);

        ia_abilityUse = new InputAction[AMOUNT_OF_ABIL_SLOTS];
        controls = new PlayerInputActions();

        //Input Cache
        ia_look = controls.Player.Look;
        ia_move = controls.Player.Move;
        ia_sprint = controls.Player.Sprint;
        ia_lockOn = controls.Player.LockOn;
        ia_dash = controls.Player.Dash;
        ia_useCombo = controls.Player.UseCombo;
        ia_block = controls.Player.Block;
        ia_switchAttackMode = controls.Player.SwitchMode;

        ia_abilityUse[0] = controls.Player.Ability1;
        ia_abilityUse[1] = controls.Player.Ability2;
        ia_abilityUse[2] = controls.Player.Ability3;
        ia_abilityUse[3] = controls.Player.Ability4;

        print("Player Init, IA Abilityuses set up");
    }



    #region enable/disable

    protected override void OnEnable()
    {
        WaitExtension.WaitAFrame(this, Enable);

        void Enable()
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

            ia_useCombo.Enable();
            ia_useCombo.performed += ctx =>
            {
                usedCombo = true;
                useAbility?.Invoke("Combo");
            };

            ia_block.Enable();
            ia_block.started += ctx =>
            {
                blockStart?.Invoke();
                useAbility?.Invoke("Block");
            };
            ia_block.canceled += ctx =>
            {
                blockStop?.Invoke();
            };

            ia_switchAttackMode.Enable();
            ia_switchAttackMode.performed += ctx =>
            {
                switchAbilityMode?.Invoke();
            };


            for (int i = 0; i < AMOUNT_OF_ABIL_SLOTS; i++)
            {
                print($"[PlayerControls] Enabled Ability Slot [{i}]");
                ia_abilityUse[i].Enable();
            }

            ia_abilityUse[0].performed += ctx =>
            {
                print($"[PlayerControlls] pressed ability [{0}]");
                abilitySlots[0]?.Invoke(0);
                useAbility?.Invoke(CurMode().name);
            };
            ia_abilityUse[1].performed += ctx =>
            {
                print($"[PlayerControlls] pressed ability [{1}]");
                abilitySlots[1]?.Invoke(1);
                useAbility?.Invoke(CurMode().name);
            };
            ia_abilityUse[2].performed += ctx =>
            {
                print($"[PlayerControlls] pressed ability [{2}]");
                abilitySlots[2]?.Invoke(2);
                useAbility?.Invoke(CurMode().name);
            };
            ia_abilityUse[3].performed += ctx =>
            {
                print($"[PlayerControlls] pressed ability [{3}]");
                abilitySlots[3]?.Invoke(3);
                useAbility?.Invoke(CurMode().name);
            };
        }


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

        ia_useCombo.Disable();
        ia_useCombo.performed -= ctx => useAbility?.Invoke("Combo");

        ia_block.started -= ctx =>
        {
            blockStart?.Invoke();
            useAbility?.Invoke("Block");
        };
        ia_block.canceled -= ctx =>
        {
            blockStop?.Invoke();
        };

        ia_switchAttackMode.Disable();
        ia_switchAttackMode.performed -= ctx => switchAbilityMode?.Invoke();

        for (int i = 0; i < AMOUNT_OF_ABIL_SLOTS; i++)
        {
            //print($"[PlayerControls] Disabled Ability Slot [{i}]");
            ia_abilityUse[i].Disable();
        }

        ia_abilityUse[0].performed -= ctx =>
        {
            //print($"[PlayerControlls] pressed ability [{0}]");
            abilitySlots[0]?.Invoke(0);
        };
        ia_abilityUse[1].performed -= ctx =>
        {
            //print($"[PlayerControlls] pressed ability [{1}]");
            abilitySlots[1]?.Invoke(1);
        };
        ia_abilityUse[2].performed -= ctx =>
        {
            //print($"[PlayerControlls] pressed ability [{2}]");
            abilitySlots[2]?.Invoke(2);
        };
        ia_abilityUse[3].performed -= ctx =>
        {
            //print($"[PlayerControlls] pressed ability [{3}]");
            abilitySlots[3]?.Invoke(3);
        };

        base.OnDisable();
    }
    #endregion 


}
