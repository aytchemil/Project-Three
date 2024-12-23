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
        Controls = GetComponent<CombatEntityController>();
    }

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

    protected virtual void Start()
    {
        Respawn();
    }

    protected virtual void Respawn()
    {
        InstantiateColliderDetector();
    }

    public virtual ColliderDetector InstantiateColliderDetector()
    {
        myColliderDetector = Instantiate(colliderDetecterAsset, transform, false).GetComponent<ColliderDetector>();
        myColliderDetector.combatLock = this;

        return myColliderDetector;
    }

    void ExitCombat()
    {
        //combatEntityInLockedZone = false;
        isLockedOnto = false;
    }

    public virtual void DeLock()
    {
        Debug.Log("Combat lock : Delocking");
        isLockedOnto = false;
        Controls.ExitCombat?.Invoke();
        myColliderDetector.UnLockFromCombatLock();
    }

    protected virtual void Lock()
    {
        Debug.Log("Combat Lock: Locking on");
        isLockedOnto = true;
        Controls.EnterCombat?.Invoke();
        Controls.CombatFollowTarget?.Invoke(lockedTarget);
        Controls.SelectCertainAbility?.Invoke("up");
    }


    protected virtual void AttemptLock()
    {
        Debug.Log("Attempting a lock");
        if (!isLockedOnto)
        {
           Debug.Log("Is not locked onto something already");

            if (combatEntityInLockedZone)
            {
                Debug.Log("Found something to lock onto");
                Debug.Log("Locking On");

                Lock();
            }
        }
        else
        {
            Debug.Log("Is already locked onto, will delock");
            DeLock();
        }
    }

    public virtual void ColliderLockOntoTarget()
    {
        Transform transform = myColliderDetector.gameObject.transform;
        transform.LookAt(lockedTarget.transform.position);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }

    void TargetDeath(CombatEntityController target)
    {
        print("Combat lock: target death: " + target.name);
        myColliderDetector.OnTriggerExit(target.gameObject.GetComponent<Collider>());
    }


}
