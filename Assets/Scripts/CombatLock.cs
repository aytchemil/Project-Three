using UnityEngine;


public class CombatLock : MonoBehaviour
{
    protected virtual CombatEntityController Controls { get; set; }

    //Component References
    public CombatEntityController lockedTarget;
    public bool combatEntityInLockedZone;

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
    }

    private void OnDisable()
    {
        Controls.ExitCombat -= ExitCombat;
        Controls.TargetDeath -= TargetDeath;
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
        //Debug.Log("Combat lock : Delocking");
        isLockedOnto = false;
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
        Controls.CombatFollowTarget?.Invoke(lockedTarget);
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
        //Debug.Log("Attempting a lock");
        if (!isLockedOnto)
        {
           //Debug.Log("Is not locked onto something already");

            if (combatEntityInLockedZone)
            {
                //Debug.Log("Found something to lock onto");
                //Debug.Log("Locking On");

                LockOnCaller();
            }
        }
        else
        {
            //Debug.Log("Is already locked onto, will delock");
            DeLockCaller();
        }
    }

    /// <summary>
    /// Locks on and looks at the target's location, restricted to Y and Z
    /// </summary>
    public virtual void ColliderLockOntoTarget()
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
        print("Combat lock: target death: " + target.name);
        myColliderDetector.OnTriggerExit(target.gameObject.GetComponent<Collider>());
    }

    #endregion


}
