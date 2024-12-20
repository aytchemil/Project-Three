using System;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerController))]
public class PlayerCombatLock : CombatLock
{
    //Cache
    PlayerController controls;
    public float damage;

    protected virtual void Awake()
    {
        controls = gameObject.GetComponent<PlayerController>();
    }


    private void Start()
    {
        Debug.Log("start : PLayer");
        //Input Action Callback Additions
        controls.lockOn.performed += ctx => AttemptLock();
        Respawn();
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable : PLayer");
    }



    protected override void Respawn()
    {
        InstantiateColliderDetector();
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
        //Debug.Log("PlayerCombatEntity: Delocking");

        base.DeLock();
        ExitCombat();
        // Debug.Log("Player Unlocking");
    }

    void EnterCombat()
    {
       // Debug.Log("PlayerCombatEntity: Entering Combat");

        controls.EnterCombat?.Invoke();
        controls.CombatFollowTarget?.Invoke(lockedTarget);
    }

    private void ExitCombat()
    {
        //Debug.Log("PlayerCombatEntity: Exiting Combat");
        
        controls.ExitCombat?.Invoke();
    }
    public override ColliderDetector InstantiateColliderDetector()
    {
        //Debug.Log("Player creating Collider Detector");
        ColliderDetector colliderDetector = base.InstantiateColliderDetector();

        //Debug.Log(colliderDetector);
        
        return colliderDetector;

    }

   // GameObject InstantiateAttkCollider()
   // {
   //    // return Instantiate(AttkTriggerColliderPrefab, GetComponentInChildren<ColliderDetector>().transform, false);
   // }

    public override void ColliderLockOntoTarget()
    {
        base.ColliderLockOntoTarget();


    }




}
