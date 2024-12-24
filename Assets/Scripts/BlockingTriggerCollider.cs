using UnityEngine;

public class BlockingTriggerCollider : MonoBehaviour
{
    public CombatFunctionality myCombatFunctionality;
    public LayerMask collideWith;

    private void Awake()
    {
        //Cache
        Collider col = GetComponent<Collider>();

        //Sets the collision's layers
        col.includeLayers = collideWith;
        col.excludeLayers = ~collideWith;
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    
}
