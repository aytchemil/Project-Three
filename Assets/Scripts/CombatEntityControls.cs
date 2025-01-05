using UnityEngine;
using System;
using UnityEngine.InputSystem;

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
    public Action attack;
    public Action blockStart;
    public Action blockStop;

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

    [Header("Pre-Selected Abilities")]
    public Ability a_right;
    public Ability a_left;
    public Ability a_up;
    public Ability a_down;

    [Header("Central Flags")]
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

    }


    protected virtual void OnDisable()
    {
        look = null;
        move = null;
        sprintStart = null;
        sprintStop = null;
        lockOn = null;
        dash = null;
        attack = null;
        blockStart = null;
        blockStop = null;
        GetTarget = null;
    }


}
