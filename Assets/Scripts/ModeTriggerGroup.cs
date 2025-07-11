using System.Collections;
using UnityEngine;

/// <summary>
/// Template Method Pattern
/// </summary>
public abstract class ModeTriggerGroup : MonoBehaviour
{
    public CombatFunctionality combatFunctionality;
    public virtual Ability myAbility { get; set; }
    public virtual bool trigger { get; set; }
    public virtual bool used { get; set; }
    public virtual bool unused { get; set; }

    public bool isLocal;

    /// <summary>
    /// VIRTUAL PARENT FUNCTION 
    /// To use the trigger on a potential delay using an ability
    /// </summary>
    /// <param name="currentAbility"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    public virtual void Use(Ability ability, float delay)
    {
        //Already Using it Check
        if (trigger) return;

        //Mutations
        // + SETS using trigger TRUE
        // + SETS unused FALSE
        // + SETS did reattack FALSE
        // + METHOD -> Enables the Trigger
        // + INVOKE (SPEAKER) -> The initial delay is over -> Renable the trigger
        trigger = true;
        unused = false;
        combatFunctionality.Controls.didReattack = false;
        EnableTrigger();
        Invoke(nameof(InitialDelayOver_ReEnableTrigger), delay);
        if(ability.hasAdditionalFunctionality)
            combatFunctionality.Controls.UseCombatAdditionalFunctionality?.Invoke(ability);
    }


    /// <summary>
    /// PATHWAY FUNCTION from Use() that moves into a VIRTUAL
    /// + PATHWAY FUNCTION
    /// + SETS initial ability use delay over FALSE
    /// </summary>
    public void EnableTrigger()
    {
        EnableTriggerImplementation();

        combatFunctionality.initialAbilityUseDelayOver = false;
    }

    protected abstract void EnableTriggerImplementation();

    
    /// <summary>
    /// SETS initialAbilityUseDelayOver to TRUE
    /// + SETS a flag value in CombatFunctionality that says the initial attack delay is over
    /// + PATHWAY FUNCTION to a VIRTUAL 
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

        //print("disabling trigger locally");

        trigger = false;
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


    protected IEnumerator DisableThisTriggerOnDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DisableThisTriggerOnDelayImplementation();
        DisableThisTrigger();
    }

    protected abstract void DisableThisTriggerOnDelayImplementation();

}
