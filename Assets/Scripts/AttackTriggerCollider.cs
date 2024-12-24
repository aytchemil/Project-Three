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
        //Cache
        Collider col = GetComponent<Collider>();
        if (attackTriggerAnimator == null)
            attackTriggerAnimator = GetComponent<Animator>();

        //Sets the collision's layers
        col.includeLayers = collideWith;
        col.excludeLayers = ~collideWith;
        
    }

    private void OnDisable()
    {
        DisableTrigger();
    }

    /// <summary>
    /// Where the actual attack takes place
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        float newEnemyHealth;
        if (attacking)
        {

            #region Death
            ///If the Entity being attacked's health reaches less than 0, tell OUR Controller to call the target death delegate action
            newEnemyHealth = other.GetComponent<AttackbleEntity>().Attacked(myAbility);
            if (newEnemyHealth < 0)
            {
                //Debug.Log("Enemy health 0, killed enemy, now calling TargetDeath to signal an enemy death");
                Debug.Log("Enemy that died was: " + other.gameObject.name);
                combatFunctionality.TargetDeathCaller(other.GetComponent<CombatEntityController>());
            }

            #endregion


            //Debug.Log("Collider attacking");
        }
    }

    /// <summary>
    /// Tells this script what its attack parameters are
    /// </summary>
    /// <param name="currentAbility"></param>
    public void AttackTriggerAttack(Ability currentAbility)
    {
        //Debug.Log("Attack Trigger attacking!");
        myAbility = currentAbility;
        EnableTrigger();
    }


    /// <summary>
    /// Sets the attacking flag
    /// </summary>
    void EnableTrigger()
    {
        attacking = true;
    }


    /// <summary>
    /// Disables this trigger's functionality
    /// - sets the attacking flag to false
    /// - Tell's the CombatFunctionality to finish attacking
    /// - Disables this GameObject
    /// </summary>
    void DisableTrigger()
    {
        attacking = false;
        combatFunctionality.FinishAttacking();
        gameObject.SetActive(false);
    }
}
