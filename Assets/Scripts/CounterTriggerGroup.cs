using UnityEngine;

public class CounterTriggerGroup : AttackTriggerColliderSingle
{
    private CounterAbility myCounterAbility;

    //Overriding base class Ability reference
    public override Ability myAbility
    {
        get => myCounterAbility;
        set => myCounterAbility = value as CounterAbility;
    }

    //Wrapper for usingTrigger
    public bool countering;
    public bool counterUp;

    public LayerMask counterolisionWith;

    public virtual void Awake()
    {
        //Cache
        col = GetComponent<Collider>();
        if (animator == null)
            animator = GetComponent<Animator>();

        //Sets the collision's layers
        col.includeLayers = counterolisionWith;
        col.excludeLayers = ~counterolisionWith;

    }

    protected override void EnableTriggerImplementation()
    {
        counterUp = false;
        countering = false;
        animator.SetBool("counter", false);

        base.EnableTriggerImplementation();
    }

    protected override void InitialDelayOver_ReEnableTriggerImplementation()
    {
        counterUp = true;

        base.InitialDelayOver_ReEnableTriggerImplementation();

        gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    protected override void DisableThisTriggerImplementation()
    {
        print("Disabling this Counter Attack Trigger, Not locally");
    }

    protected override void DisableThisTriggerLocallyImplementation()
    {
        counterUp = false;
        countering = false;
        animator.SetBool("counter", false);
        col.includeLayers = counterolisionWith;
        col.excludeLayers = ~counterolisionWith;
    }

    protected override void InitializeSelfImplementation(CombatFunctionality combatFunctionality)
    {
        //print(combatFunctionality.gameObject.name + " | ability initializing...");
    }

    public override void OnTriggerStay(Collider other)
    {
        if (counterUp)
        {
            if (other.GetComponent<AttackTriggerGroup>().attacking)
                CounterAttack();

        }

        if (countering)
        {

            base.OnTriggerStay(other);

        }
    }



    void CounterAttack()
    {
        countering = true;
        gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        animator.SetBool("counter", true);

        col.includeLayers = attackColisionWith;
        col.excludeLayers = ~attackColisionWith;
    }

}
