using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AttackTriggerColliderSingle : AttackTriggerGroup
{
    public virtual AttackAbility myAttackAbility { get; protected set; }

    //Overriding base class Ability reference
    public override AttackingAbility myAttackingAbility
    {
        get => myAttackAbility;
        set => myAttackAbility = value as AttackAbility;
    }

    [Header("Collider Single: Real-Time Variables")]
    public LayerMask attackColisionWith;
    public Collider col;
    public Animator animator;

    public virtual void Awake()
    {
        //Cache
        col = GetComponent<Collider>();
        if (animator == null)
            animator = GetComponent<Animator>();

        //Sets the collision's layers
        col.includeLayers = attackColisionWith;
        col.excludeLayers = ~attackColisionWith;

    }

    #region  Template Pattern Overrides
    //Template Pattern Overrides
    ///=======================================================================================================================================

    protected override void EnableTriggerImplementation()
    {
        base.EnableTriggerImplementation();

        DisableIndiviualCollider(Color.grey);

        animator.SetBool("windupDone", false);
        animator.SetBool("missed", false);
    }

    protected override void InitialDelayOver_ReEnableTriggerImplementation()
    {
        base.InitialDelayOver_ReEnableTriggerImplementation();

        gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        col.enabled = true;

        animator.SetBool("windupDone", true);
    }



    #endregion


    /// <summary>
    /// Where the actual attack takes place
    /// </summary>
    /// <param name="other"></param>
    public virtual void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<ModeTriggerGroup>()) return;
        if (other.GetComponent<CombatEntityController>() == combatFunctionality.Controls) return;

        float newEnemyHealth;
        if (attacking && combatFunctionality.initialAbilityUseDelayOver)
        {
            HitAttack();
            #region Death
            ///If the Entity being attacked's health reaches less than 0, tell OUR Controller to call the target death delegate action
            newEnemyHealth = other.GetComponent<AttackbleEntity>().Attacked(myAttackingAbility);


            if (newEnemyHealth < 0)
            {
                //Debug.Log("Enemy health 0, killed enemy, now calling TargetDeath to signal an enemy death");
                //Debug.Log("Enemy that died was: " + other.gameObject.name + " by " + combatFunctionality.gameObject.name);
                combatFunctionality.TargetDeathCaller(other.GetComponent<CombatEntityController>());
                if (gameObject.GetComponent<CombatLock>() != null)
                {
                    gameObject.GetComponent<CombatLock>().ExitCombatCaller();
                }


                //Enemy exits combat when dieing
                if (other.gameObject.GetComponent<CombatLock>() != null)
                {
                    other.gameObject.GetComponent<CombatLock>().ExitCombatCaller();
                    // print("enemy dead, delocking caller called");
                }
            }

            #endregion


            //Debug.Log("Collider attacking");
        }
    }


    #region Overrides
    //Overides
    ///==============================================================================================================================================================================================

    public override void HitAttack()
    {
        base.HitAttack();

        DisableIndiviualCollider(Color.green);
    }

    public override void MissAttackCuttoffLocal()
    {
        //print(gameObject.name + " | MissAttackCuttoffLocal() " + " " + myAttackAbility);

        base.MissAttackCuttoffLocal();

        DisableIndiviualCollider(Color.grey);

        animator.SetBool("missed", true);
    }

    #endregion



    #region  Methods
    // Methods
    ///=============================================================================================================================================================

    public void DisableIndiviualCollider(Color color)
    {
        gameObject.GetComponent<MeshRenderer>().material.color = color;
        col.enabled = false;
    }


    #endregion

}
