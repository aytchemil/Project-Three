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

    protected override void InitializeSelfImplementation(CombatFunctionality cf, Ability abilty)
    {
        base.InitializeSelfImplementation(cf, abilty);

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
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ModeTriggerGroup>()) return;
        if (other.GetComponent<EntityController>() == cf.Controls) return;

        if (attacking && initialUseDelayOver)
        {
            print($"AT [{ability.name}] hit {other.gameObject.name}");
            HitAttack();
            AbilityExecutor.OnHit(ability, myAttackAbility.Dir.ToString(), cf.gameObject, other.gameObject);
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
        StartCoroutine(FreezeAttack(0.15f, myLookDir, effectPos));
    }
    

    IEnumerator FreezeAttack(float time, string l, Vector3 effectpos)
    {
        //print("Freezing attack");
        float prevSpeed = animator.speed;
        float prevanimContSpeed = cf.Controls.animController.animator.speed;

        animator.speed = 0;
        cf.Controls.animController.animator.speed = 0;

        yield return new WaitForSeconds(time);

        animator.speed = prevSpeed;
        cf.Controls.animController.animator.speed = prevanimContSpeed;
        BlockedCompleteSequence(l, effectpos);
    }

    void BlockedCompleteSequence(string l, Vector3 effectpos)
    {
        print("didreattack: block sequence complete");
        base.AttackTriggerBlocked(l, effectpos);
        DisableThisTrigger();

        //cf.Controls.Mode("Attack").isUsing = false;
    }

    #endregion

}
