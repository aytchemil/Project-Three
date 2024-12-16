using UnityEngine;

public class ColliderDetector : MonoBehaviour
{
    public CombatEntity myCombatEntity;

    private void OnTriggerEnter(Collider other)
    {
        myCombatEntity.combatEntityInLockedZone = true;
        myCombatEntity.lockedTarget = other.GetComponent<CombatEntity>();
    }


    private void OnTriggerExit(Collider other)
    {
        myCombatEntity.combatEntityInLockedZone = false;
        myCombatEntity.lockedTarget = null;
        myCombatEntity.DeLock();
    }
}
