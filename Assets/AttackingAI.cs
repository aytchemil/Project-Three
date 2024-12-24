using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackingAI : MonoBehaviour
{
    CombatEntityController controls;
    CombatLock combatLock;
    CombatFunctionality combatFunctionality;
    EntityLook entityLook;

    public Action look;
    public Action move;
    public Action sprint;
    public Action lockOn;
    public Action dash;
    public Action attack;
    public Action block;

    private void Awake()
    {
        controls = GetComponent<CombatEntityController>();
        combatLock = GetComponent<CombatLock>();
        combatFunctionality = GetComponent<CombatFunctionality>();
        entityLook = GetComponent<EntityLook>();
    }

    protected void OnEnable()
    {
        controls.CombatFollowTarget += CombatFollowTarget;

        attack += combatFunctionality.UseAttackAbility;
        lockOn += combatLock.AttemptLock;

        //print("f");
        block += BlockCaller;
        block += StopBlockingCaller;

    }

    protected void OnDisable()
    {
        controls.CombatFollowTarget -= CombatFollowTarget;

        attack -= combatFunctionality.UseAttackAbility;
        lockOn -= combatLock.AttemptLock;

        block -= BlockCaller;
        block -= StopBlockingCaller;
    }

    private void FixedUpdate()
    {
        if (!controls.isLockedOn)
        {
            //print("f");
            AttemptLockOn();
        }

        //If not in combat,"Look regularly". else "combat look"
        if (controls.isLockedOn)
        {
            controls.CombatFollowTarget?.Invoke(combatLock.myColliderDetector.closestCombatEntity.GetComponent<CombatEntityController>());
        }
    }

    void AttemptLockOn()
    {
        //print(gameObject.name + " attempting to lock on");
        lockOn?.Invoke();
    }


    void CombatFollowTarget(CombatEntityController target)
    {
        entityLook.CameraLookAtLockTarget(target.transform.position);
        entityLook.TransformLookAtTarget(target.transform.position);
    }


    void BlockCaller()
    {
        // print("Player Combat : Block Caller called");
        controls.Block?.Invoke();
    }

    void StopBlockingCaller()
    {
        //print("Player Combat : Stop Blocking Caller called");
        controls.StopBlocking?.Invoke();
    }


}
