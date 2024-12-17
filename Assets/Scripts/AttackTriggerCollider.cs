using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AttackTriggerCollider : MonoBehaviour
{
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

    }

}
