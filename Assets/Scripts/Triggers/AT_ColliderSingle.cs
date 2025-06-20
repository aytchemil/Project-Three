using System.Collections;
using TMPro;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AT_ColliderSingle : GeneralAttackTriggerGroup
{
    public virtual AbilityAttack myAttackAbility { get; set; }


    //Overriding base class Ability reference
    public override Ability myAbility
    {
        get => myAttackAbility;
        set => myAttackAbility = value as AbilityAttack;
    }


    [Header("Collider Single: Real-Time Variables")]
    public LayerMask attackColisionWith;
    public Collider col;
    public Animator animator;

    #region  Template Pattern Overrides
    //Template Pattern Overrides
    ///=======================================================================================================================================

    protected override void EnableTriggerImplementation()
    {
        base.EnableTriggerImplementation();

        if (!DebugManager.instance.AttackCollisionDebugsOn)
            DisableIndiviualCollider(Color.grey);
        else
            EnableColliderVisual(false);

        animator.SetBool("windupDone", false);
        animator.SetBool("missed", false);
    }

    protected override void InitialDelayOver_ReEnableTriggerImplementation()
    {
        missedAttack = false;
        hitAttack = false;
        col.enabled = true;

        if (!DebugManager.instance.AttackCollisionDebugsOn)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else
            EnableColliderVisual(false);

        animator.SetBool("windupDone", true);
    }

    protected override void InitializeSelfImplementation(CombatFunctionality combatFunctionality, Ability abilty)
    {
        base.InitializeSelfImplementation(combatFunctionality, abilty);

        col = GetComponent<Collider>();
        if (animator == null)
            animator = GetComponent<Animator>();

        //Sets the collision's layers
        col.includeLayers = attackColisionWith;
        col.excludeLayers = ~attackColisionWith;
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

            //print("attacking with ability: " + myAttackAbility);

            ///If the Entity being attacked's health reaches less than 0, tell OUR Controller to call the target death delegate action
            newEnemyHealth = other.GetComponent<AttackbleEntity>().Attacked(myAttackAbility, combatFunctionality.Controls.lookDir);


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

        if (!DebugManager.instance.AttackCollisionDebugsOn)
            DisableIndiviualCollider(Color.green);
        else
            EnableColliderVisual(false);
    }

    public override void MissAttackCuttoffLocal()
    {
        //print(gameObject.name + " | MissAttackCuttoffLocal() " + " " + myAttackAbility);

        base.MissAttackCuttoffLocal();

        if (!DebugManager.instance.AttackCollisionDebugsOn)
            DisableIndiviualCollider(Color.grey);
        else
            EnableColliderVisual(false);

        col.enabled = false;
        animator.SetBool("missed", true);
    }

    #endregion



    #region  Methods
    // Methods
    ///=============================================================================================================================================================

    public void DisableIndiviualCollider(Color color)
    {
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        gameObject.GetComponent<MeshRenderer>().material.color = color;
    }

    public void EnableColliderVisual(bool enable)
    {
        gameObject.GetComponent<MeshRenderer>().enabled = enable;
    }



    public override void AttackTriggerBlocked(Vector3 effectPos, string lookdir)
    {
        col.enabled = false;

        print("AT Collider being frozen");
        StartCoroutine(FreezeAttack(0.2f, effectPos, lookdir));
    }
    

    IEnumerator FreezeAttack(float time, Vector3 e, string l)
    {
        float prevSpeed = animator.speed;
        animator.speed = 0;
        yield return new WaitForSeconds(time);
        animator.speed = prevSpeed;
        BlockedCompleteSequence(e, l);
    }

    void BlockedCompleteSequence(Vector3 e, string l)
    {
        print("didreattack: block sequence complete");
        base.AttackTriggerBlocked(e, l);
        DisableThisTrigger();

        //combatFunctionality.Controls.Mode("Attack").isUsing = false;
    }

    #endregion

}
