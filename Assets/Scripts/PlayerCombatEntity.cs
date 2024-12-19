using System;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(ControllsHandler))]
public class PlayerCombatEntity : CombatEntity
{
    //Cache
    ControllsHandler controls;
    public float damage;

    protected virtual void Awake()
    {
        controls = gameObject.GetComponent<ControllsHandler>();
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

        //Instantiate the attack triggers based on the current abilites
        ControllsHandler c = controls;

        //Debug.Log(controls);

        Ability[] abilities = { c.a_right, c.a_left, c.a_up, c.a_down };

        //Debug.Log(abilities);

        colliderDetector.InstantiateAttackTriggers(abilities);


        //GameObject attkTrigger = InstantiateAttkCollider();
        //attkTrigger.GetComponent<AttackTriggerCollider>().myCombatEntity = this;


        colliderDetector.Init();
        

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
