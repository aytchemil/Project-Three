using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CounterTriggerGroup : AttackTriggerFollowUp
{
    public virtual CounterAbility myCounterAbility { get; set; }

    //Overriding base class Ability reference
    public override AttackMultiAbility myAttackMultiAbility 
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

    protected override void InitializeSelfImplementation(CombatFunctionality combatFunctionality, Ability abilty)
    {
        base.InitializeSelfImplementation(combatFunctionality, abilty);

        myAbility = abilty as CounterAbility;

        //print(combatFunctionality.gameObject.name + " | ability initializing...");
    }

    IEnumerator CounterDown()
    {
        yield return new WaitForSeconds(myCounterAbility.counterUpTime);

        if (countering) yield break;

        print("counter is down");
        DisableThisTrigger();
    }

    public override float CheckForTriggerUpdates_ReturnDelay(int i)
    {
        float delay = base.CheckForTriggerUpdates_ReturnDelay(i);

        if (triggerBeingUsed is BlockTriggerGroupCollider block)
            if (block.blocking)
            {
                print("following up... Countering... counter");
                countering = true;
                triggerProgress[i] = true;
            }

        return delay;
    }

}
