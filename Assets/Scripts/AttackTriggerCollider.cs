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


    //Scritpable Object Current Attk
    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("In Range");
    }

    private void OnDisable()
    {
        DisableTrigger();
    }

    private void OnTriggerStay(Collider other)
    {
        if (attacking)
        {
            other.GetComponent<AttackbleEntity>().Attacked(myAbility);
            Debug.Log("Collider attacking");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Debug.Log("OutOfRange Range");
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
