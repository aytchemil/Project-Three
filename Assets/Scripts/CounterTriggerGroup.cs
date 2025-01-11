using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CounterTriggerGroup : AttackTriggerFollowUp
{
    public virtual CounterAbility myCounterAbility { get; set; }

    //Overriding base class Ability reference
    public override AttackingAbility myAttackingAbility
    {
        get => myCounterAbility;
        set => myCounterAbility = value as CounterAbility;
    }

    //Wrapper for usingTrigger
    public virtual bool countering { get; set; }
    public override bool attacking
    {
        get => countering;
        set => countering = value;
    }

    public virtual bool countered { get; set; }
    public override bool hitAttack
    {
        get => countered;
        set => countered = value;
    }

    public bool counterUp;


    protected override void EnableTriggerImplementation()
    {
        counterUp = false;
        countered = false;
        countering = false;

        base.EnableTriggerImplementation();
    }

    protected override void InitialDelayOver_ReEnableTriggerImplementation()
    {
        counterUp = true;

        print("counter is up");

        base.InitialDelayOver_ReEnableTriggerImplementation();

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

        base.DisableThisTriggerImplementation();
    }

    protected override void InitializeSelfImplementation(CombatFunctionality combatFunctionality)
    {
        base.InitializeSelfImplementation(combatFunctionality);
        //print(combatFunctionality.gameObject.name + " | ability initializing...");
    }

    void CounterAttack()
    {
        countering = true;
        gameObject.GetComponent<MeshRenderer>().material.color = Color.green;

        triggerBeingUsed.gameObject.SetActive(true);
        triggerBeingUsed.StartUsingAbilityTrigger(myCounterAbility.counterAttack_attackAbility, myCounterAbility.counterAttack_attackAbility.initialAttackDelay[0]);
    }

    IEnumerator CounterDown()
    {
        yield return new WaitForSeconds(myCounterAbility.counterUpTime);
        print("counter is down");
        DisableThisTrigger();
    }

}
