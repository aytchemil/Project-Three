using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AT_ColliderSingle : GeneralAttackTriggerGroup
{
    public virtual AbilityAttack myAttackAbility { get; set; }


    //Overriding base class Ability reference
    public override Ability ability
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

        if (DebugManager.instance.AttackCollisionDebugsOn)
        {
            ColliderVisualActive(true);
            ColliderVisualColor(Color.grey);
        }
        else
            ColliderVisualActive(false);

        animator.SetBool("windupDone", false);
        animator.SetBool("missed", false);
    }

    protected override void InitialDelayOver_ReEnableTriggerImplementation()
    {
        missedAttack = false;
        hitAttack = false;
        col.enabled = true;

        if (DebugManager.instance.AttackCollisionDebugsOn)
        {
            ColliderVisualActive(true);
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else
            ColliderVisualActive(false);

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
        if (attacking && initialUseDelayOver)
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

        if (DebugManager.instance.AttackCollisionDebugsOn)
            ColliderVisualColor(Color.green);
        else
            ColliderVisualActive(false);
    }

    public override void MissAttackCuttoffLocal()
    {
        //print(gameObject.name + " | MissAttackCuttoffLocal() " + " " + myAttackAbility);

        base.MissAttackCuttoffLocal();

        if (DebugManager.instance.AttackCollisionDebugsOn)
            ColliderVisualColor(Color.grey);
        else
            ColliderVisualActive(false);

        col.enabled = false;
        animator.SetBool("missed", true);
    }

    #endregion



    #region  Methods
    // Methods
    ///=============================================================================================================================================================

    public void ColliderVisualColor(Color color)
    {
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        gameObject.GetComponent<MeshRenderer>().material.color = color;
    }

    public void ColliderVisualActive(bool enable)
    {
        gameObject.GetComponent<MeshRenderer>().enabled = enable;
    }



    public override void AttackTriggerBlocked(string myLookDir, Vector3 effectPos)
    {
        col.enabled = false;

        //print(effectPos + " " + myLookDir);

        //print("AT Collider being frozen");
        StartCoroutine(FreezeAttack(0, myLookDir, effectPos));
    }
    

    IEnumerator FreezeAttack(float time, string l, Vector3 e)
    {
        //print("Freezing attack");
        float prevSpeed = animator.speed;
        float prevanimContSpeed = combatFunctionality.Controls.animController.animator.speed;

        animator.speed = 0;
        combatFunctionality.Controls.animController.animator.speed = 0;

        yield return new WaitForSeconds(time);

        animator.speed = prevSpeed;
        combatFunctionality.Controls.animController.animator.speed = prevanimContSpeed;
        BlockedCompleteSequence(l, e);
    }

    void BlockedCompleteSequence(string l, Vector3 e)
    {
        print("didreattack: block sequence complete");
        base.AttackTriggerBlocked(l, e);
        DisableThisTrigger();

        //combatFunctionality.Controls.Mode("Attack").isUsing = false;
    }

    #endregion

}
