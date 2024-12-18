using UnityEngine;


public class CombatEntity : MonoBehaviour
{

    //Component References
    public CombatEntity lockedTarget;
    public bool combatEntityInLockedZone;

    //Cache
    ColliderDetector myColliderDetector;

    //Asset References
    public GameObject colliderDetecterAsset;


    //Flags
    public bool isLockedBy; //Later make this a list so multiple things can lock onto a Combat Entity
    public bool isLockedOnto;
    public bool inRange;


    protected virtual void Awake()
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
        myColliderDetector.myCombatEntity = this;

        return myColliderDetector;
    }

    public virtual void DeLock()
    {
        isLockedOnto = false;
        myColliderDetector.DisableAttackTriggers();
    }

    protected virtual void Lock()
    {
        isLockedOnto = true;
        myColliderDetector.EnableAttackTriggers();

    }


    protected virtual void AttemptLock()
    {
        //Debug.Log("Attempting a lock");
        if (!isLockedOnto)
        {
           //Debug.Log("Is not locked onto something already");

            if (combatEntityInLockedZone)
            {
                //Debug.Log("Found something to lock onto");
               // Debug.Log("Locking On");

                Lock();
            }
        }
        else
        {
           // Debug.Log("Is already locked onto, will delock");
            DeLock();
        }
    }

    public virtual void ColliderLockOntoTarget()
    {
        Transform transform = myColliderDetector.gameObject.transform;
        transform.LookAt(lockedTarget.transform.position);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }




}
