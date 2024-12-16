using System;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(ControllsHandler))]
public class PlayerCombatEntity : CombatEntity
{
    //Cache
    ControllsHandler controls;


    protected override void Awake()
    {
        base.Awake();
        controls = gameObject.GetComponent<ControllsHandler>();

    }


    private void Start()
    {
        //Input Action Callback Additions
        controls.lockOn.performed += ctx => AttemptLock();
    }



    protected override void Respawn()
    {
        base.Respawn();
    }

    protected override void Lock()
    {
       // Debug.Log("PlayerCombatEntity: Locking on");
        base.Lock();
        EnterCombat();
       // Debug.Log("Player Locking on");
       
    }

    public override void DeLock()
    {
        Debug.Log("PlayerCombatEntity: Delocking");

        base.DeLock();
        ExitCombat();
        // Debug.Log("Player Unlocking");
    }

    void EnterCombat()
    {
       // Debug.Log("PlayerCombatEntity: Entering Combat");

        controls.EnterCombat?.Invoke(lockedTarget);
    }

    private void ExitCombat()
    {
        Debug.Log("PlayerCombatEntity: Exiting Combat");
        
        controls.ExitCombat?.Invoke(lockedTarget);
    }



}
