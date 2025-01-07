using UnityEngine;

public class HealthTrigger : MonoBehaviour
{
    public float healAmount;

    private void OnTriggerStay(Collider other)
    {
        AttackbleEntity attackbleEntity = other.GetComponent<AttackbleEntity>();

        attackbleEntity.Heal(healAmount);
    }
}
