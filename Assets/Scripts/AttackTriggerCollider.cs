using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AttackTriggerCollider : MonoBehaviour
{
    public CombatLock combatLock;
    public LayerMask collideWith;

    public bool attacking;

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        col.includeLayers = collideWith;
        col.excludeLayers = ~collideWith;
    }

    //Scritpable Object Current Attk
    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("In Range");
        combatLock.inRange = true;
        if (attacking)
        {
            Debug.Log("Collider attacking");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Debug.Log("OutOfRange Range");
        combatLock.inRange = false;
    }

}
