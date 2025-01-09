using UnityEngine;

/// <summary>
/// Template Method Pattern
/// </summary>
public abstract class ModeTriggerGroup : MonoBehaviour
{
    public CombatFunctionality combatFunctionality;
    public virtual Ability myAbility { get; set; }
    [SerializeField] protected bool usingTrigger;

    public void StartUsingAbilityTrigger(Ability currentAbility, float delay)
    {
        if (usingTrigger)
            return;

        usingTrigger = true;

        myAbility = currentAbility;
        EnableTrigger();

        Invoke(nameof(InitialDelayOver_ReEnableTrigger), delay);
    }


    /// <summary>
    /// Initializes the Trigger
    /// </summary>
    public void EnableTrigger()
    {
        EnableTriggerImplementation();

        combatFunctionality.initialAbilityUseDelayOver = false;
    }

    protected abstract void EnableTriggerImplementation();

    
    /// <summary>
    /// This method is invoked on a delay, when the delay completes it will tell combatFunctionality that the initial ability delay is over (by flag value)
    /// </summary>
    public void InitialDelayOver_ReEnableTrigger()
    {
        InitialDelayOver_ReEnableTriggerImplementation();

        // print("Initial Attack Delay over, reenbling trigger");
        combatFunctionality.initialAbilityUseDelayOver = true;
    }

    protected abstract void InitialDelayOver_ReEnableTriggerImplementation();



    /// <summary>
    /// Disables the trigger, allows calls to other scripts with the Implementation.
    /// </summary>
    public void DisableThisTrigger()
    {
        DisableThisTriggerImplementation();

        combatFunctionality.initialAbilityUseDelayOver = true;

        DisableThisTriggerOnlyLocally();
    }

    protected abstract void DisableThisTriggerImplementation();




    /// <summary>
    /// Use the base.DisableThisTriggerOnlyLocally() at the end of the override, because it using gameObejct.SetActive(false); at the end
    /// </summary>
    public void DisableThisTriggerOnlyLocally()
    {
        DisableThisTriggerLocallyImplementation();

        usingTrigger = false;

        gameObject.SetActive(false);
    }

    protected abstract void DisableThisTriggerLocallyImplementation();



    public void InitializeSelf(CombatFunctionality combatFunctionality)
    {
        this.combatFunctionality = combatFunctionality;

        InitializeSelfImplementation(combatFunctionality);
    }

    protected abstract void InitializeSelfImplementation(CombatFunctionality combatFunctionality);


}
