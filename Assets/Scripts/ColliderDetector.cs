using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColliderDetector : MonoBehaviour
{
/// <summary>
/// Colliider detector class controls : Itself, Attack collide triggers
/// Turns on and off the attack triggers
/// </summary>
    public CombatLock combatLock;
    public LayerMask collideWith;
        
    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        col.includeLayers = collideWith;
        col.excludeLayers = ~collideWith;
    }

    private void OnTriggerEnter(Collider other)
    {
        combatLock.combatEntityInLockedZone = true;
        combatLock.lockedTarget = other.GetComponent<CombatEntityController>();
        if (other.GetComponent<CombatEntityController>() == null)
            Debug.LogError("Error: Combat entity controller script not given to a combat entity : " + other.name);
    }

    private void OnTriggerStay(Collider other)
    {
        if (combatLock.isLockedOnto)
        {
            combatLock.ColliderLockOntoTarget();
        }
    }


    private void OnTriggerExit(Collider other)
    {
        combatLock.combatEntityInLockedZone = false;
        combatLock.lockedTarget = null;
        if (combatLock.isLockedOnto)
        {
            combatLock.DeLock();
        }


        transform.localEulerAngles = Vector3.zero;
    }
}
