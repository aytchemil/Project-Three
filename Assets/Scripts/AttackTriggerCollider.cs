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
    public bool hitAttack;

    //Cache
    Collider col;

    private void Awake()
    {
        //Cache
        col = GetComponent<Collider>();
        if (attackTriggerAnimator == null)
            attackTriggerAnimator = GetComponent<Animator>();

        //Sets the collision's layers
        col.includeLayers = collideWith;
        col.excludeLayers = ~collideWith;
        
    }

    private void OnEnable()
    {
        hitAttack = false;
        gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        col.enabled = true;
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
            HitAttack();
            #region Death
            ///If the Entity being attacked's health reaches less than 0, tell OUR Controller to call the target death delegate action
            newEnemyHealth = other.GetComponent<AttackbleEntity>().Attacked(myAbility);
            if (newEnemyHealth < 0)
            {
                //Debug.Log("Enemy health 0, killed enemy, now calling TargetDeath to signal an enemy death");
                //Debug.Log("Enemy that died was: " + other.gameObject.name + " by " + combatFunctionality.gameObject.name);
                combatFunctionality.TargetDeathCaller(other.GetComponent<CombatEntityController>());
                if(gameObject.GetComponent<CombatLock>() != null)
                {
                    gameObject.GetComponent<CombatLock>().ExitCombatCaller();
                }
                

                //Enemy exits combat when dieing
                if(other.gameObject.GetComponent<CombatLock>() != null)
                {
                    other.gameObject.GetComponent<CombatLock>().ExitCombatCaller();
                   // print("enemy dead, delocking caller called");
                }
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


        if(combatFunctionality.Controls.GetTarget?.Invoke() != null)
        {
            ResetAttackCaller();
           // print("Disabling trigger from : " + gameObject.name + " Target: " + combatFunctionality.Controls.GetTarget?.Invoke());
        }
        else
        {
            //print("Target is already null when trying to reset attack caller");
        }

        gameObject.SetActive(false);
    }

    void ResetAttackCaller()
    {
       // print("Attack Trigger: ResetAttackCaller()");

        combatFunctionality.Controls.ResetAttack?.Invoke();
    }

    void HitAttack()
    {
        hitAttack = true;
        DisableAttack(Color.green);
    }

    void DisableAttack(Color color)
    {
        gameObject.GetComponent<MeshRenderer>().material.color = color;
        col.enabled = false;
    }

    void MissAttackCuttoff()
    {
        if (hitAttack)
        {
            Invoke(nameof(ComboOffOfHit), myAbility.nextAttacksComboDelay);
            ComboOffOfHit();
            return;
        }

       // print("missed attack");
        hitAttack = false;

        DisableAttack(Color.grey);
        MissedAttackCaller();
    }

    void MissedAttackCaller()
    {
        combatFunctionality.Controls.MissedAttack?.Invoke();
    }

    void ComboOffOfHit()
    {
       // print("Can now combo attack");
        DisableTrigger();
    }
}
