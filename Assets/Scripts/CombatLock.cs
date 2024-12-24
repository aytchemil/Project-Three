using System;
using UnityEngine;


public class CombatLock : MonoBehaviour
{
    public virtual CombatEntityController Controls { get; set; }

    //Component References
    public bool combatEntityInLockedZone;
    public bool lockUnlockDelayInEffect = false;
    public float lockUnlockDelay = 0.5f;

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
    }

    protected virtual void OnDisable()
    {
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
        Debug.Log(gameObject.name + " | Combat lock : ExitCombatCaller Caller Executed");


        //if (Controls.ExitCombat != null)
        //    foreach (var subscriber in Controls.ExitCombat.GetInvocationList())
        //        Debug.Log($"Subscriber: {subscriber.Target}, Method: {subscriber.Method}");
        //else
        //    Debug.LogError("ExitCombat subscribers null");



        Controls.isLockedOn = false;
        StartCoroutine(myColliderDetector.ReturnToPreLockedUnlockedState());
        myColliderDetector.UnLockFromCombatLock();

        //CALLER : EXIT COMBAT
        if (Controls.ExitCombat == null)
            Debug.LogError("ExitCombat subscribers are null, please check subscribers to ensure they are subscribed for : " + gameObject.name);
        Controls.ExitCombat?.Invoke();
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

        //CALLER : SELECT CERTAIN ABILITY CALLER (DEFAULT INPUT)
        if (Controls.SelectCertainAbility == null)
            Debug.LogError("Select Certain Ability subscribers are null, please check subscribers to ensure they are subscribed for : " + gameObject.name);
        Controls.SelectCertainAbility?.Invoke("up");


        //CALLER : ENTER COMBAT 
        if (Controls.EnterCombat == null)
            Debug.LogError("EnterCombat subscribers are null, please check subscribers to ensure they are subscribed for : " + gameObject.name);
        Controls.EnterCombat?.Invoke();
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
        //Debug.Log("attemping lock");

        if (lockUnlockDelayInEffect && Controls.isAlive) return;

        UnlockDelockDelay();

       // Debug.Log("Attempting a lock");
        if (!Controls.isLockedOn)
        {
         //  Debug.Log("Is not locked onto something already");

            if (combatEntityInLockedZone)
            {
           //     Debug.Log("Found something to lock onto");
                //Debug.Log("Locking On");

                EnterCombatCaller();
            }
        }
        else
        {
            //Debug.Log("Is already locked onto, will delock");
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
        print("Combat lock: target death: " + target.name);
        myColliderDetector.OnTriggerExit(target.gameObject.GetComponent<Collider>());
    }

    #endregion

    /// <summary>
    /// If the player is already attacking, you cant unlock
    ///- Integration with CombatFunctionality to check if the player is attacking
    /// </summary>
    void UnlockWhileNotAttacking()
    {
        if (!GetComponent<CombatFunctionality>().Controls.alreadyAttacking)
        {
            //print("unlocking while not attacking succeeded");
            ExitCombatCaller();
        }
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
