using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AttackTriggerCollider : MonoBehaviour
{
    public CombatEntity myCombatEntity;
    public LayerMask collideWith;
    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        col.includeLayers = collideWith;
        col.excludeLayers = ~collideWith;
    }

    //Scritpable Object Current Attk
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("In Range");
        myCombatEntity.inRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OutOfRange Range");
        myCombatEntity.inRange = false;
    }

}
