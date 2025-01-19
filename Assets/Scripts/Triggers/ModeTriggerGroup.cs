using UnityEngine;

/// <summary>
/// Template Method Pattern
/// </summary>
public abstract class ModeTriggerGroup : MonoBehaviour
{
    public CombatFunctionality combatFunctionality;
    public virtual Ability myAbility { get; set; }
    public virtual bool usingTrigger { get; set; }
    public virtual bool used { get; set; }
    public virtual bool unused { get; set; }

    public bool isLocal;


    public virtual void StartUsingAbilityTrigger(Ability currentAbility, float delay)
    {
        if (usingTrigger)
            return;

        usingTrigger = true;
        unused = false;

        //print("starting to use ability from " + gameObject.name + " setting my ability : " + myAbility);
        EnableTrigger();

        Invoke(nameof(InitialDelayOver_ReEnableTrigger), delay);
        //print("successfully started using ability: " + currentAbility);
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
        if (isLocal)
        {
            DisableThisTriggerOnlyLocally();
        }
        else
        {
            DisableThisTriggerImplementation();

            combatFunctionality.initialAbilityUseDelayOver = true;

            DisableThisTriggerOnlyLocally();
        }
    }

    protected abstract void DisableThisTriggerImplementation();




    /// <summary>
    /// Use the base.DisableThisTriggerOnlyLocally() at the end of the override, because it using gameObejct.SetActive(false); at the end
    /// </summary>
    public void DisableThisTriggerOnlyLocally()
    {
        DisableThisTriggerLocallyImplementation();

        usingTrigger = false;
        unused = false;

        gameObject.SetActive(false);
    }

    protected abstract void DisableThisTriggerLocallyImplementation();



    public void InitializeSelf(CombatFunctionality combatFunctionality, Ability ability)
    {
        //print(gameObject.name + " init self");
        this.combatFunctionality = combatFunctionality;

        InitializeSelfImplementation(combatFunctionality, ability);

        //print($"initializing self: " + gameObject.name + $" myAbility value is now {myAbility.name}");
    }

    protected abstract void InitializeSelfImplementation(CombatFunctionality combatFunctionality, Ability abilty);


}
