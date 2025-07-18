using System.Collections;
using UnityEngine;

public class CounterTriggerGroup : MAT_FollowupGroup
{
    public virtual AbilityCounter myCounterAbility { get; set; }

    //Overriding base class Ability reference
    public override AbilityMulti myMultiAbility
    {
        get => myCounterAbility;
        set => myCounterAbility = value as AbilityCounter;
    }

    //Wrapper for trigger
    public virtual bool countering { get; set; }
    public override bool attacking
    {
        get => countering;
        set => countering = value;
    }
    //public override bool hitAttack
    //{
    //    get => blocked;
    //    set => blocked = value;
    //}

    public bool counterUp;

    protected override void EnableTriggerImplementation()
    {
        counterUp = false;
        //blocked = false;
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
        combatFunctionality.Controls.Mode("Counter").functionality.Finish();

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

        ability = (AbilityCounter)abilty;

        //print(combatFunctionality.gameObject.name + " | ability initializing...");
    }

    IEnumerator CounterDown()
    {
        yield return new WaitForSeconds(myCounterAbility.counterUpTime);

        if (countering) yield break;

        print("counter is down");
        DisableThisTrigger();
    }

    public override void CheckForTriggerUpdates_ReturnDelay(int i)
    {
        base.CheckForTriggerUpdates_ReturnDelay(i);

        if (chosenChildTrigger is BlockTriggerCollider block)
            if (block.blocking)
            {
                print("following up... Countering... counter");
                countering = true;
                triggerProgress[i] = true;
            }
    }

}
