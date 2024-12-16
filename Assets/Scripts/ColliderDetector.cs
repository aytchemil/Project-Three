using UnityEngine;

public class ColliderDetector : MonoBehaviour
{
    public CombatEntity myCombatEntity;

    private void OnTriggerEnter(Collider other)
    {
        myCombatEntity.combatEntityInLockedZone = true;
    }

    private void OnTriggerStay(Collider other)
    {
        myCombatEntity.combatEntityInLockedZone = true;
    }

    private void OnTriggerExit(Collider other)
    {
        myCombatEntity.combatEntityInLockedZone = false;
    }
}
