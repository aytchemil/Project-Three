using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AttackTriggerColliderSingle : AttackTriggerGroup
{
    [Header("Collider: Real-Time Variables")]
    public LayerMask collideWith;
    Collider col;
    public Animator attackTriggerAnimator;

    public override void Awake()
    {
        base.Awake();

        //Cache
        col = GetComponent<Collider>();
        if (attackTriggerAnimator == null)
            attackTriggerAnimator = GetComponent<Animator>();

        //Sets the collision's layers
        col.includeLayers = collideWith;
        col.excludeLayers = ~collideWith;
        
    }

    /// <summary>
    /// Sets the attacking flag
    /// </summary>
    public override void InitializeTrigger()
    {
        base.InitializeTrigger();

        DisableIndiviualCollider(Color.grey);

        attackTriggerAnimator.SetBool("windupDone", false);
        attackTriggerAnimator.SetBool("missed", false);
    }

    /// <summary>
    /// Tells this script what its attack parameters are
    /// </summary>
    /// <param name="currentAbility"></param>
    public override void StartAttackFromAttackTrigger(AttackAbility currentAbility, float delay)
    {
        base.StartAttackFromAttackTrigger(currentAbility, delay);
    }

    public override void InitialAttackDelayOverReEnableTrigger()
    {
        base.InitialAttackDelayOverReEnableTrigger();

        gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        col.enabled = true;

        attackTriggerAnimator.SetBool("windupDone", true);
    }

    /// <summary>
    /// Where the actual attack takes place
    /// </summary>
    /// <param name="other"></param>
    public virtual void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<CombatEntityController>() == combatFunctionality.Controls) return;

        float newEnemyHealth;
        if (attacking && combatFunctionality.initialAttackDelayOver)
        {
            HitAttack();
            #region Death
            ///If the Entity being attacked's health reaches less than 0, tell OUR Controller to call the target death delegate action
            newEnemyHealth = other.GetComponent<AttackbleEntity>().Attacked(myAttackAbility);


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




    public override void HitAttack()
    {
        base.HitAttack();

        DisableIndiviualCollider(Color.green);

    }

    public void DisableIndiviualCollider(Color color)
    {
        gameObject.GetComponent<MeshRenderer>().material.color = color;
        col.enabled = false;
    }

    public override void MissAttackCuttoffLocal()
    {
        print(gameObject.name + " | MissAttackCuttoffLocal()");
        base.MissAttackCuttoffLocal();

        DisableIndiviualCollider(Color.grey);

        attackTriggerAnimator.SetBool("missed", true);
    }

}
