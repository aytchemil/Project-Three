using UnityEngine;
using System;
using UnityEngine.InputSystem;

/// <summary>
/// Centralized Controller and controls for every combat entity.
/// </summary>
public class CombatEntityController : MonoBehaviour
{
    //Rule of thumb : Keep these such that they are always set on the outside, never during gameplay
    public Action EnterCombat;
    public Action ExitCombat;
    public Action<CombatEntityController> CombatFollowTarget;
    public Action<string> SelectCertainAbility;
    [Space]
    public Ability a_right;
    public Ability a_left;
    public Ability a_up;
    public Ability a_down;
}
