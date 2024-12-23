using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AttackTriggerCollider : MonoBehaviour
{
    [Header("Real-Time Variables")]
    public CombatFunctionality combatFunctionality;
    public Ability myAbility;
    public LayerMask collideWith;
    public Animator attackTriggerAnimator;

    public bool attacking;

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        col.includeLayers = collideWith;
        col.excludeLayers = ~collideWith;
        if(attackTriggerAnimator == null)
            attackTriggerAnimator = GetComponent<Animator>();
    }

    private void OnDisable()
    {
        DisableTrigger();
    }

    private void OnTriggerStay(Collider other)
    {
        float newEnemyHealth;
        if (attacking)
        {
            newEnemyHealth = other.GetComponent<AttackbleEntity>().Attacked(myAbility);
            if (newEnemyHealth < 0)
            {
                combatFunctionality.TargetDeathCaller(other.GetComponent<CombatEntityController>());
            }
            Debug.Log("Collider attacking");
        }
    }


    public void AttackTriggerAttack(Ability currentAbility)
    {
        //Debug.Log("Attack Trigger attacking!");
        myAbility = currentAbility;
        EnableTrigger();
    }



    void EnableTrigger()
    {
        attacking = true;
    }

    void DisableTrigger()
    {
        attacking = false;
        combatFunctionality.FinishAttacking();
        gameObject.SetActive(false);
    }
}
