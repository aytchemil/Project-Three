using UnityEngine;

public class ColliderDetector : MonoBehaviour
{
    public CombatEntity myCombatEntity;


    private void OnTriggerEnter(Collider other)
    {
        myCombatEntity.combatEntityInLockedZone = true;
        myCombatEntity.lockedTarget = other.GetComponent<CombatEntity>();
    }

    private void OnTriggerStay(Collider other)
    {
        if(myCombatEntity.isLockedOnto)
            myCombatEntity.ColliderLockOntoTarget();
    }


    private void OnTriggerExit(Collider other)
    {
        myCombatEntity.combatEntityInLockedZone = false;
        myCombatEntity.lockedTarget = null;
        if (myCombatEntity.isLockedOnto)
            myCombatEntity.DeLock();
    }
}
