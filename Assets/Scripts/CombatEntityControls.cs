using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEditor.ShaderGraph;

/// <summary>
/// Centralized Controller and controls for every combat entity.
/// </summary>
public class CombatEntityController : MonoBehaviour
{
    //Rule of thumb : Keep these such that they are always set on the outside, never during gameplay

    [Header("Controls")]
    public Func<Vector2> look;
    public Func<Vector2> move;
    public Action sprintStart;
    public Action sprintStop;
    public Action lockOn;
    public Action dash;
    public Action<string> useAbility;
    public Action blockStart;
    public Action blockStop;
    public Action switchAttackMode;
    public string mode = "attack";

    [Header("Observer Events")]
    public Func<CombatEntityController> GetTarget;
    public Action EnterCombat;
    public Action ExitCombat;
    public Action<CombatEntityController> CombatFollowTarget;
    public Action<string> SelectCertainAbility;
    public Action<CombatEntityController> TargetDeath;
    public Action ResetAttack;
    public Action MissedAttack;
    public Action CompletedAttack;
    public Action<float> Flinch;
    public Func<string> getMoveDirection;


    [Header("Pre-Selected Abilities")]
    [Header("Attack Abilities")]
    public AttackAbility a_right;
    public AttackAbility a_left;
    public AttackAbility a_up;
    public AttackAbility a_down;

    [Header("Counter Abilities")]
    public CounterAbility c_right;
    public CounterAbility c_left;
    public CounterAbility c_up;
    public CounterAbility c_down;

    [Header("Central Flags")]
    public Func<bool> cantUseAbility;
    public bool dashing;
    public bool dashOnCooldown;
    public bool isLockedOn;
    public bool alreadyAttacking;
    public bool isBlocking;
    public bool currentlyRetargetting;
    public bool isAlive = true;
    public bool isFlinching = false;

    [Header("Combat Flags")]
    public bool targetIsDodging;

    protected virtual void OnEnable()
    {
        cantUseAbility = () => (!isLockedOn || alreadyAttacking || isBlocking || isFlinching);
    }
    protected virtual void OnDisable()
    {
        look = null;
        move = null;
        sprintStart = null;
        sprintStop = null;
        lockOn = null;
        dash = null;
        useAbility = null;
        blockStart = null;
        blockStop = null;
        GetTarget = null;
        cantUseAbility = null;
    }


}
