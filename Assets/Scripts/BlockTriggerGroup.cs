using UnityEngine;

public class BlockTriggerGroup : ModeTriggerGroup
{
    private BlockAbility myBlockbility;

    //Overriding base class Ability reference
    public override Ability myAbility
    {
        get => myBlockbility;
        set => myBlockbility = value as BlockAbility;
    }

    //Wrapper for usingTrigger
    public bool blocking
    {
        get => usingTrigger;
        set => usingTrigger = value;
    }

    public bool blocked;


    protected override void DisableThisTriggerImplementation()
    {

    }

    protected override void DisableThisTriggerLocallyImplementation()
    {
        blocked = false;
    }

    protected override void EnableTriggerImplementation()
    {

    }

    protected override void InitialDelayOver_ReEnableTriggerImplementation()
    {
        
    }

    protected override void InitializeSelfImplementation(CombatFunctionality combatFunctionality)
    {
        
    }
}
