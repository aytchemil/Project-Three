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

    /// <summary>
    /// Where the actual attack takes place
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        float newEnemyHealth;
        if (attacking && combatFunctionality.initialAttackDelayOver)
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
    public void StartAttackFromAttackTrigger(Ability currentAbility)
    {
        if(attacking) { print("already attacking, cannot re attack"); return; }
        //Debug.Log("Starting an Attack: " + currentAbility.name);
        myAbility = currentAbility;
        InitializeTrigger();
        Invoke(nameof(InitialAttackDelayOverReEnableTrigger), currentAbility.initialAttackDelay);
    }


    /// <summary>
    /// Sets the attacking flag
    /// </summary>
    void InitializeTrigger()
    {
        //print("initializing attack trigger: " + gameObject.name);
        attacking = true;
        hitAttack = false;
        combatFunctionality.initialAttackDelayOver = false;
        attackTriggerAnimator.SetBool("windupDone", false);

        DisableAttack(Color.grey);
    }


    /// <summary>
    /// Disables this trigger's functionality
    /// - sets the attacking flag to false
    /// - Tell's the CombatFunctionality to finish attacking
    /// - Disables this GameObject
    /// </summary>
    public void DisableTrigger()
    {
        //print("Disabling trigger");
        CompleteAttackCaller();
        attacking = false;
        combatFunctionality.initialAttackDelayOver = false;
        combatFunctionality.FinishAttacking();


        if(combatFunctionality.Controls.GetTarget?.Invoke() != null)
        {
            ResetAttackCaller();
            //print("Disabling trigger from : " + gameObject.name + " Target: " + combatFunctionality.Controls.GetTarget?.Invoke());
        }
        else
        {
            //print("Target is already null when trying to reset attack caller");
        }

        gameObject.SetActive(false);
    }

    public void EndOfAnimationDebug()
    {
       // print("Hit end of attack animation");
    }

    void CompleteAttackCaller()
    {
        //Debug.Log("Compeleted Attack");
        combatFunctionality.Controls.CompletedAttack?.Invoke();
    }

    void ResetAttackCaller()
    {
        //print("Attack Trigger: ResetAttackCaller()");

        foreach (var subscriber in combatFunctionality.Controls.ResetAttack.GetInvocationList())
        {
            if (subscriber != null)
            {
                //print(subscriber);

            }
            else
                Debug.LogError("No subscribers found in reset attack, this needs movement subscribed to it, check for that first");
        }

        combatFunctionality.Controls.ResetAttack?.Invoke();
    }

    void HitAttack()
    {
        //Debug.Log("Attack hit registered");
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
        //print("Miss Attack Cuttoff Reached");
        if (hitAttack) return;

        //print("missed attack");
        hitAttack = false;

        DisableAttack(Color.grey);
        MissedAttackCaller();
    }

    void MissedAttackCaller()
    {
        combatFunctionality.Controls.MissedAttack?.Invoke();
    }

    public void ComboOffOfHitNowAvaliable()
    {
        if (hitAttack)
        {
            //print("Combo off hit time period reached, can now combo because attack hit");
            print("Disabling this attack to allow for combo");
            DisableTrigger();
        }
    }

    public void InitialAttackDelayOverReEnableTrigger()
    {
        // print("Initial Attack Delay over, reenbling trigger");
        combatFunctionality.initialAttackDelayOver = true;
        attackTriggerAnimator.SetBool("windupDone", true);
        gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        col.enabled = true;
    }
}
