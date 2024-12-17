using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColliderDetector : MonoBehaviour
{
    public CombatEntity myCombatEntity;
    public LayerMask collideWith;

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        col.includeLayers = collideWith;
        col.excludeLayers = ~collideWith;
    }


    private void OnTriggerEnter(Collider other)
    {
        myCombatEntity.combatEntityInLockedZone = true;
        myCombatEntity.lockedTarget = other.GetComponent<CombatEntity>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (myCombatEntity.isLockedOnto)
        {
            myCombatEntity.ColliderLockOntoTarget();

        }
    }


    private void OnTriggerExit(Collider other)
    {
        myCombatEntity.combatEntityInLockedZone = false;
        myCombatEntity.lockedTarget = null;
        if (myCombatEntity.isLockedOnto)
            myCombatEntity.DeLock();

        transform.localEulerAngles = Vector3.zero;
    }
}
