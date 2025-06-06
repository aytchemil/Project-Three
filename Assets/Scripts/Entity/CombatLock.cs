using System;
using System.Collections;
using UnityEngine;


public class CombatLock : MonoBehaviour
{
    public virtual CombatEntityController Controls { get; set; }

    //Component References
    public bool combatEntityInLockedZone;
    public bool lockUnlockDelayInEffect = false;
    public float lockUnlockDelay = 0.5f;
    bool tryingToLock;

    //Cache
    public ColliderDetector myColliderDetector;

    //Asset References
    public GameObject colliderDetecterAsset;

    protected virtual void Awake()
    {
        //Cache
        Controls = GetComponent<CombatEntityController>();
    }
    protected virtual void Start()
    {
        Respawn();
    }

    #region EnableDisable
    protected virtual void OnEnable()
    {
        Controls.TargetDeath += TargetDeath;
        Controls.CombatFollowTarget += ColliderLockOntoTarget;
        Controls.lockOn += AttemptLock;
    }

    protected virtual void OnDisable()
    {
        Controls.TargetDeath -= TargetDeath;
        Controls.CombatFollowTarget -= ColliderLockOntoTarget;
        Controls.lockOn -= AttemptLock;
    }
    #endregion


    /// <summary>
    /// Respawns the collider detector
    /// </summary>
    protected virtual void Respawn()
    {
        InstantiateColliderDetector();
    }


    /// <summary>
    /// Instantiates the collider detector and caches it
    /// </summary>
    /// <returns></returns>
    public virtual ColliderDetector InstantiateColliderDetector()
    {
        myColliderDetector = Instantiate(colliderDetecterAsset, transform, false).GetComponent<ColliderDetector>();
        myColliderDetector.combatLock = this;

        return myColliderDetector;
    }



    #region Combat Lock

    /// <summary>
    /// Caller to de lock onto something
    /// - Sets the locked on flag
    /// - Caller to exit combat via controls
    /// - Tells the collider detector to unlock
    /// </summary>
    public virtual void ExitCombatCaller()
    {
        //EXIT COMBAT:
        CombatLockExitCombatInternalMethod();

        //CALLER : EXIT COMBAT (Called after ExitCombatCaller has done its stuff
        if (Controls.ExitCombat == null)
            Debug.LogError("ExitCombat subscribers are null, please check subscribers to ensure they are subscribed for : " + gameObject.name);
        Controls.ExitCombat?.Invoke();

    }

    void CombatLockExitCombatInternalMethod()
    {
        //Debug.Log(gameObject.name + " | Combat lock : ExitCombatCaller Caller Executed");

        Controls.isLockedOn = false;
        StartCoroutine(myColliderDetector.ReturnToPreLockedUnlockedState());
        myColliderDetector.UnLockFromCombatLock();

        Controls.GetTarget = null;
        //Controls.ResetAttack = null; //This is important, this prevents the combat state from reverting to combat when this entity kills another
    }

    /// <summary>
    /// Caller to lock onto a target via the Controls
    /// - Sets the locked onto flag
    /// - Caller to enter combat
    /// - Caller to Follow the target
    /// - Cllaer to Select the default ability
    /// </summary>
    protected virtual void EnterCombatCaller()
    {
        //Debug.Log("Combat Lock: EnterCombatCaller called");
        //ENTER COMBAT: 
        Controls.isLockedOn = true;
        //print("is now locked on");

        //CALLER : SELECT CERTAIN ABILITY CALLER (DEFAULT INPUT)
        if (Controls.abilitySlots == null)
            Debug.LogError($"[{gameObject.name}] Select Certain Ability subscribers are null, please check subscribers to ensure they are subscribed for : " + gameObject.name);
        Controls.abilitySlots[0]?.Invoke(0);


        //CALLER : ENTER COMBAT 
        if (Controls.EnterCombat == null)
            Debug.LogError("EnterCombat subscribers are null, please check subscribers to ensure they are subscribed for : " + gameObject.name);
        Controls.EnterCombat?.Invoke();

        Controls.GetTarget = () => myColliderDetector.closestCombatEntity.GetComponent<CombatEntityController>();
        //print("Locked onto target: " + Controls.GetTarget?.Invoke());
    }

    /// <summary>
    /// Attempts a lock on to a target inside the CollisionDetector
    /// - If already not not locked onto something
    /// - If there is an entity in the zone
    /// 
    /// If there is already something locked onto, then delock on that something
    /// </summary>
    public virtual void AttemptLock()
    {
        //Debug.Log(gameObject.name + " | attemping lock");

        if (lockUnlockDelayInEffect || !Controls.isAlive || Controls.dashing) return;
        
        //Debug.Log(gameObject.name + " | lock successfull, applying..");

        LockUnlockDelay();

        // Debug.Log(gameObject.name + " | Attempting a lock");
        //print("Are we locked on already? " + Controls.isLockedOn);
        if (!Controls.isLockedOn)
        {
             // Debug.Log(gameObject.name + " | Is not locked onto something already");

            if (combatEntityInLockedZone)
            {
               // Debug.Log(gameObject.name + " | Found something to lock onto");
               // Debug.Log(gameObject.name + " | Locking On");

                EnterCombatCaller();
            }
        }
        else
        {
          //  Debug.Log(gameObject.name + " | Is already locked onto, will delock");
            UnlockWhileNotAttacking();
        }
    }

    /// <summary>
    /// Locks on and looks at the target's location, restricted to Y and Z
    /// </summary>
    public virtual void ColliderLockOntoTarget(CombatEntityController target)
    {
        Transform transform = myColliderDetector.gameObject.transform;

        transform.LookAt(target.transform.position);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }


    #endregion

    #region Death

    /// <summary>
    /// Tells the collider detector that the target who is now dead must leave the TriggerStay bounds
    /// </summary>
    /// <param name="target"></param>
    void TargetDeath(CombatEntityController target)
    {
        print("Combat lock: TargetDeath Reciever method called for newly dead target: " + target.name + " Now calling OnTriggerExit for killer " + gameObject.name);
        myColliderDetector.OnTriggerExit(target.gameObject.GetComponent<Collider>());
    }

    #endregion

    /// <summary>
    /// If the player is already attacking, you cant unlock
    ///- Integration with CombatFunctionality to check if the player is attacking
    /// </summary>
    void UnlockWhileNotAttacking()
    {
        //print(Controls.isLockedOn);
        if (!GetComponent<CombatFunctionality>().Controls.Mode("Attack").isUsing)
        {
          //  print("unlocking while not attacking succeeded");
            ExitCombatCaller();
        }
    }

    #region LockUnlockDelay
    void LockUnlockDelay()
    {
       // Debug.Log("Lock unlock delay");
      //  print(lockUnlockDelayInEffect);

        lockUnlockDelayInEffect = true;
        if (!IsInvoking("DisableUnlockDelockDelay"))
            Invoke("DisableUnlockDelockDelay", lockUnlockDelay);
        
    }

    void DisableUnlockDelockDelay()
    {
        lockUnlockDelayInEffect = false;
    }

    #endregion

}
