using System.Collections;
using UnityEngine;

public class BlockTriggerCollider : ModeTriggerGroup
{
    public bool blockUp;

    public virtual AbilityBlock myBlockAbility { get; set; }
    public override Ability myAbility
    {
        get => myBlockAbility;
        set => myBlockAbility = value as AbilityBlock;

    }

    public virtual bool blocking { get; set; }
    public override bool usingTrigger
    {
        get => blocking;
        set => blocking = value;
    }

    public virtual bool blockMissed { get; set; }

    public LayerMask blockCollisionWith;
    public Collider col;
    public Animator animator;

    public virtual void Awake()
    {
        //Cache
        col = GetComponent<Collider>();
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    protected override void EnableTriggerImplementation()
    {
        blockUp = false;
        blocking = false;
        blockMissed = false;
        animator.SetBool("counter", false);
        animator.SetBool("windupDone", false);


        print("enabled counter trigger");
    }

    protected override void InitialDelayOver_ReEnableTriggerImplementation()
    {
        blockUp = true;
        animator.SetBool("windupDone", true);

        gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
        StartCoroutine(CounterDown());

        print("intial delay done. counter is up...");

    }

    protected override void DisableThisTriggerImplementation()
    {
        combatFunctionality.FinishCountering();

        print("Disabling this Counter Attack Trigger, Not locally");
        StopAllCoroutines();
    }

    protected override void DisableThisTriggerLocallyImplementation()
    {
        blockUp = false;
        blocking = false;
        blockMissed = false;

        animator.SetBool("counter", false);
    }

    protected override void InitializeSelfImplementation(CombatFunctionality combatFunctionality, Ability abilty)
    {
        myAbility = (AbilityBlock)abilty;
    }



    public virtual void OnTriggerStay(Collider other)
    {
        if (blockUp && !blocking)
        {
            if(other.GetComponent<GeneralAttackTriggerGroup>() == null)
            {
                //print("no attack trigger found. returning");
                return;
            }

            if (other.GetComponent<GeneralAttackTriggerGroup>().attacking)
            {
                print("COUNTER STARTED");

                //print("Countered against: " + other.gameObject.GetComponent<AttackTriggerGroup>().name);
                other.gameObject.GetComponent<GeneralAttackTriggerGroup>().GetCountered(other.ClosestPoint(transform.position));

                CounterAttack();

            }
        }
    }

    void CounterAttack()
    {
        blocking = true;
        gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        animator.SetBool("counter", true);
    }

    IEnumerator CounterDown()
    {
        yield return new WaitForSeconds(myBlockAbility.blockUpTime);
        print("counter collider is down");
        unused = true;
        DisableThisTriggerOnlyLocally();
    }

}
