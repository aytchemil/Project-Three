using UnityEngine;
using System;
using UnityEngine.InputSystem;

/// <summary>
/// Centralized Controller and controls for every combat entity.
/// </summary>
public class CombatEntityController : MonoBehaviour
{
    //Rule of thumb : Keep these such that they are always set on the outside, never during gameplay
    public Func<Vector2> look;
    //public Action<Vector2> move;
    //public Action sprintStarted;
    //public Action sprintCancelled;
    //public Action lockOn;
    //public Action dash;
    //public Action attack;

    public Func<Vector2> move;

    public Action sprintStart;
    public Action sprintStop;

    public Action lockOn;

    public Action dash;

    public Action attack;

    public Action blockStart;
    public Action blockStop;



    public Action EnterCombat;
    public Action ExitCombat;
    public Action<CombatEntityController> CombatFollowTarget;
    public Action<string> SelectCertainAbility;
    public Action<CombatEntityController> TargetDeath;
    [Space]
    public Ability a_right;
    public Ability a_left;
    public Ability a_up;
    public Ability a_down;
    [Space]
    public Action Block;
    public Action StopBlocking;

    [Header("Central Flags")]
    public bool dashOnCooldown;
    public bool isLockedOn;
    public bool alreadyAttacking;
    public bool isBlocking;
    public bool currentlyRetargetting;
    public bool isAlive = true;

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
    }


}
