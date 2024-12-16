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
        controls.lockOn.started += ctx => AttemptLock();
    }

    protected override void Respawn()
    {
        base.Respawn();
    }

    protected override void Lock()
    {
        base.Lock();

        //if (isLockedOntoBySomething)
        //{
        //    isLockedOntoBySomething = false;
        //    //state = PlayerStates.CurrentState.notSprinting;
        //}
        //else
        //{
        //    //state = PlayerStates.CurrentState.combat;
        //    isLockedOntoBySomething = true;
        //}

    }


    void AttemptLock()
    {
        if (isLockedOntoSomething) return;

        Debug.Log("Attempting a lock");

        if (combatEntityInLockedZone)
        {
            Debug.Log("Found something to lock onto");
            Debug.Log("Locking On");

            Lock();

        }


    }



}
