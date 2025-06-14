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



        if (!DebugManager.instance.AttackCollisionDebugsOn)
            gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
        else
            gameObject.GetComponent<MeshRenderer>().enabled = false;

        if (myBlockAbility.hasBlockUpTime)
            StartCoroutine(CounterDownDelayed());
        else
            StartCoroutine(WaitForBlockToStop());

        print("intial delay done. counter is up...");

    }

    protected override void DisableThisTriggerImplementation()
    {
        combatFunctionality.Controls.BlockedAttack?.Invoke(combatFunctionality.Controls.lookDir);

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
                print("BLOCKED");
                string lookdir = combatFunctionality.Controls.lookDir;
                Vector3 effectpos = other.ClosestPoint(transform.position);

                //print("Countered against: " + other.gameObject.GetComponent<AttackTriggerGroup>().name);
                other.gameObject.GetComponent<GeneralAttackTriggerGroup>().GetBlocked(effectpos, lookdir);

                //TODO: If in defensive mode counter
                //CounterAttack();

            }
        }
    }

    void BlockAttack()
    {
        combatFunctionality.Controls.Mode("Counter");
    }

    void CounterAttack()
    {
        blocking = true;

        if (!DebugManager.instance.AttackCollisionDebugsOn)
            gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        animator.SetBool("counter", true);
    }

    IEnumerator CounterDownDelayed()
    {
        yield return new WaitForSeconds(myBlockAbility.blockUpTime);
        CounterDown();
    }
    IEnumerator WaitForBlockToStop()
    {
        while(combatFunctionality.Controls.Mode("Block").isUsing)
        {
            yield return new WaitForEndOfFrame();
        }
        CounterDown();
    }

    void CounterDown()
    {
        print("counter collider is down");
        unused = true;
        DisableThisTriggerOnlyLocally();
    }

    protected override void DisableThisTriggerOnDelayImplementation()
    {
        throw new System.NotImplementedException();
    }
}
