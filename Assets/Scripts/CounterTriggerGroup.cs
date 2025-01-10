using System.Collections;
using UnityEngine;

public class CounterTriggerGroup : AttackTriggerColliderSingle
{
    public virtual CounterAbility myCounterAbility { get; set; }

    //Overriding base class Ability reference
    public override AttackingAbility myAttackingAbility
    {
        get => myCounterAbility;
        set => myCounterAbility = value as CounterAbility;
    }

    //Wrapper for usingTrigger
    public bool countering;
    public bool counterUp;
    public bool counterMissed;

    public LayerMask counterolisionWith;

    public override void Awake()
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


        //Sets the collision's layers
        col.includeLayers = counterolisionWith;
        col.excludeLayers = ~counterolisionWith;

        base.EnableTriggerImplementation();
    }

    protected override void InitialDelayOver_ReEnableTriggerImplementation()
    {
        counterUp = true;

        print("counter is up");

        base.InitialDelayOver_ReEnableTriggerImplementation();

        gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
        StartCoroutine(CounterDown());

    }

    protected override void DisableThisTriggerImplementation()
    {
        combatFunctionality.FinishCountering();

        print("Disabling this Counter Attack Trigger, Not locally");
        StopAllCoroutines();

        base.DisableThisTriggerImplementation();
    }

    protected override void DisableThisTriggerLocallyImplementation()
    {
        counterUp = false;
        countering = false;
        counterMissed = false;

        animator.SetBool("counter", false);
        col.includeLayers = counterolisionWith;
        col.excludeLayers = ~counterolisionWith;

        base.DisableThisTriggerImplementation();
    }

    protected override void InitializeSelfImplementation(CombatFunctionality combatFunctionality)
    {
        base.InitializeSelfImplementation(combatFunctionality);
        //print(combatFunctionality.gameObject.name + " | ability initializing...");
    }

    public override void OnTriggerStay(Collider other)
    {
        if (counterUp && !countering)
        {
            if (other.GetComponent<AttackTriggerGroup>().attacking)
            {
                print("COUNTER STARTED");
                CounterAttack();

                print("Countered against: " + other.gameObject.GetComponent<AttackTriggerGroup>().name);
                other.gameObject.GetComponent<AttackTriggerGroup>().GetCountered(other.ClosestPoint(transform.position));
            }
        }

        if (countering)
        {
            print("Countering attack hit");
            base.OnTriggerStay(other);

        }
    }


    public void MissCounter()
    {
        print("Counter missed");
        counterMissed = true;
    }

    public override void MissAttackCuttoff()
    {
        if (!hitAttack)
        {
            MissCounter();
        }

        base.MissAttackCuttoff();
    }



    void CounterAttack()
    {
        countering = true;
        gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        animator.SetBool("counter", true);

        col.includeLayers = attackColisionWith;
        col.excludeLayers = ~attackColisionWith;
        print("Layer change complete");
    }

    IEnumerator CounterDown()
    {
        yield return new WaitForSeconds(myCounterAbility.counterUpTime);
        print("counter is down");
        DisableThisTrigger();
    }

}
