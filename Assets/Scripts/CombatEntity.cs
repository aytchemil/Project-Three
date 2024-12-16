using UnityEngine;


public class CombatEntity : MonoBehaviour
{

    //Component References
    public CombatEntity lockedTarget;
    public LayerMask combatEntityMask;
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
        Respawn();
    }

    protected virtual void Respawn()
    {
        InstantiateColliderDetector();
    }

    void InstantiateColliderDetector()
    {
        myColliderDetector = Instantiate(colliderDetecterAsset, transform, false).GetComponent<ColliderDetector>();
        myColliderDetector.myCombatEntity = this;
    }

    public virtual void DeLock()
    {
        isLockedOnto = false;
    }

    protected virtual void Lock()
    {
        isLockedOnto = true;
    }


    protected virtual void AttemptLock()
    {
        if (!isLockedOnto)
        {
           // Debug.Log("Attempting a lock");

            if (combatEntityInLockedZone)
            {
              //  Debug.Log("Found something to lock onto");
              //  Debug.Log("Locking On");

                Lock();
            }
        }
        else
        {
            DeLock();
        }
    }

    public virtual void ColliderLockOntoTarget()
    {
        myColliderDetector.gameObject.transform.LookAt(lockedTarget.transform.position);
    }




}
