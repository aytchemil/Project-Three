using System.Collections;
using UnityEngine;

public class CounterTriggerGroupCollider : ModeTriggerGroup
{
    public bool counterUp;

    public virtual CounterAbility myCounterAbility { get; set; }
    public override Ability myAbility
    {
        get => myCounterAbility;
        set => myCounterAbility = value as CounterAbility;

    }

    public virtual bool countering { get; set; }
    public override bool usingTrigger
    {
        get => countering;
        set => countering = value;
    }

    public virtual bool counterMissed { get; set; }

    public LayerMask counterolisionWith;
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
        counterUp = false;
        countering = false;
        counterMissed = false;
        animator.SetBool("counter", false);
        animator.SetBool("windupDone", false);


        print("enabled counter trigger");
    }

    protected override void InitialDelayOver_ReEnableTriggerImplementation()
    {
        counterUp = true;
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
        counterUp = false;
        countering = false;
        counterMissed = false;

        animator.SetBool("counter", false);
    }

    protected override void InitializeSelfImplementation(CombatFunctionality combatFunctionality, Ability abilty)
    {
        myAbility = abilty as CounterAbility;
    }



    public virtual void OnTriggerStay(Collider other)
    {
        if (counterUp && !countering)
        {
            if(other.GetComponent<AttackTriggerGroup>() == null)
            {
                //print("no attack trigger found. returning");
                return;
            }

            if (other.GetComponent<AttackTriggerGroup>().attacking)
            {
                print("COUNTER STARTED");

                //print("Countered against: " + other.gameObject.GetComponent<AttackTriggerGroup>().name);
                other.gameObject.GetComponent<AttackTriggerGroup>().GetCountered(other.ClosestPoint(transform.position));

                CounterAttack();

            }
        }
    }

    void CounterAttack()
    {
        countering = true;
        gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        animator.SetBool("counter", true);
    }

    IEnumerator CounterDown()
    {
        yield return new WaitForSeconds(myCounterAbility.counterUpTime);
        print("counter collider is down");
        unused = true;
        DisableThisTriggerOnlyLocally();
    }

}
