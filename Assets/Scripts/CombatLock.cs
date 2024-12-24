using UnityEngine;


public class CombatLock : MonoBehaviour
{
    public virtual CombatEntityController Controls { get; set; }

    //Component References
    public CombatEntityController lockedTarget;
    public bool combatEntityInLockedZone;
    public bool lockUnlockDelayInEffect = false;
    public float lockUnlockDelay = 0.5f;

    //Cache
    ColliderDetector myColliderDetector;

    //Asset References
    public GameObject colliderDetecterAsset;


    //Flags
    public bool isLockedBy; //Later make this a list so multiple things can lock onto a Combat Entity
    public bool isLockedOnto;

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
    private void OnEnable()
    {
        Controls.ExitCombat += ExitCombat;
        Controls.TargetDeath += TargetDeath;
        Controls.CombatFollowTarget += ColliderLockOntoTarget;
    }

    private void OnDisable()
    {
        Controls.ExitCombat -= ExitCombat;
        Controls.TargetDeath -= TargetDeath;
        Controls.CombatFollowTarget -= ColliderLockOntoTarget;
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

    /// <summary>
    /// Method given to the ExitCombat action delegate 
    /// - Sets the flag for locked onto something to false
    /// </summary>
    void ExitCombat()
    {
        //combatEntityInLockedZone = false;
        isLockedOnto = false;
    }

    #region Combat Lock

    /// <summary>
    /// Caller to de lock onto something
    /// - Sets the locked on flag
    /// - Caller to exit combat via controls
    /// - Tells the collider detector to unlock
    /// </summary>
    public virtual void DeLockCaller()
    {
       // Debug.Log("Combat lock : Delocking Caller Executed");
        Controls.ExitCombat?.Invoke();
        myColliderDetector.UnLockFromCombatLock();
    }

    /// <summary>
    /// Caller to lock onto a target via the Controls
    /// - Sets the locked onto flag
    /// - Caller to enter combat
    /// - Caller to Follow the target
    /// - Cllaer to Select the default ability
    /// </summary>
    protected virtual void LockOnCaller()
    {
        //Debug.Log("Combat Lock: Locking on");
        isLockedOnto = true;
        Controls.EnterCombat?.Invoke();
        Controls.SelectCertainAbility?.Invoke("up");
    }

    /// <summary>
    /// Attempts a lock on to a target inside the CollisionDetector
    /// - If already not not locked onto something
    /// - If there is an entity in the zone
    /// 
    /// If there is already something locked onto, then delock on that something
    /// </summary>
    protected virtual void AttemptLock()
    {
        if (lockUnlockDelayInEffect) return;

        UnlockDelockDelay();

       // Debug.Log("Attempting a lock");
        if (!isLockedOnto)
        {
         //  Debug.Log("Is not locked onto something already");

            if (combatEntityInLockedZone)
            {
           //     Debug.Log("Found something to lock onto");
                //Debug.Log("Locking On");

                LockOnCaller();
            }
        }
        else
        {
       //     Debug.Log("Is already locked onto, will delock");
            CantUnlockWhileAttackingOtherwiseUnlock();
        }
    }

    /// <summary>
    /// Locks on and looks at the target's location, restricted to Y and Z
    /// </summary>
    public virtual void ColliderLockOntoTarget(CombatEntityController target)
    {
        Transform transform = myColliderDetector.gameObject.transform;
        transform.LookAt(lockedTarget.transform.position);
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
       // print("Combat lock: target death: " + target.name);
        myColliderDetector.OnTriggerExit(target.gameObject.GetComponent<Collider>());
    }

    #endregion

    /// <summary>
    /// If the player is already attacking, you cant unlock
    ///- Integration with CombatFunctionality to check if the player is attacking
    /// </summary>
    void CantUnlockWhileAttackingOtherwiseUnlock()
    {
        if (GetComponent<CombatFunctionality>() != null)
            if (!GetComponent<CombatFunctionality>().Controls.alreadyAttacking)
                DeLockCaller();
    }

    #region LockUnlockDelay
    void UnlockDelockDelay()
    {
        lockUnlockDelayInEffect = true;
        Invoke("DisableUnlockDelockDelay", lockUnlockDelay);
        
    }

    void DisableUnlockDelockDelay()
    {
        lockUnlockDelayInEffect = false;
    }

    #endregion

}
