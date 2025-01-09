using UnityEngine;

public class CounterTriggerGroup : ModeTriggerGroup
{
    private CounterAbility myCounterAbility;

    //Overriding base class Ability reference
    public override Ability myAbility
    {
        get => myCounterAbility;
        set => myCounterAbility = value as CounterAbility;
    }

    //Wrapper for usingTrigger
    public bool countering
    {
        get => usingTrigger;
        set => usingTrigger = value;
    }

    public bool countered;
    public bool missedCounter;

    protected override void InitializeTriggerImplementation()
    {
        countered = false;
        missedCounter = false;
    }

    protected override void InitialDelayOver_ReEnableTriggerImplementation()
    {
        print("ability use delay over, countering...");
    }

    protected override void DisableThisTriggerImplementation()
    {
        print("Disabling this Counter Attack Trigger, Not locally");
    }

    protected override void DisableThisTriggerLocallyImplementation()
    {
        countered = false;
        missedCounter = false;
    }


}
