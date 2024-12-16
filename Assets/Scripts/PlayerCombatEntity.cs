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

        //Input Action Callback Additions
        controls.lockOn.performed += ctx => AttemptLock();
    }

    protected override void Respawn()
    {
        base.Respawn();
    }

    protected override void Lock()
    {
        base.Lock();
        EnterCombat();
       // Debug.Log("Player Locking on");
       
    }

    protected override void DeLock()
    {
        base.DeLock();
        ExitCombat();
       // Debug.Log("Player Unlocking");
    }

    void EnterCombat()
    {
        controls.EnterCombat?.Invoke();
    }

    private void ExitCombat()
    {
        controls.ExitCombat?.Invoke();
    }




}
