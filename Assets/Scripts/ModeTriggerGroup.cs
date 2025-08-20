using System.Collections;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using static AM;
using static EntityController;

/// <summary>
/// Template Method Pattern
/// </summary>
public abstract class ModeTriggerGroup : MonoBehaviour
{
    public CombatFunctionality cf;
    public abstract Ability ability { get; set; }
    public Ability Ability() { return ability; }
    public virtual bool trigger { get; set; }
    public virtual bool used { get; set; }
    public virtual bool unused { get; set; }

    public bool initialUseDelayOver;

    public bool isLocal;
    [ShowIf("isLocal")]
    public ModeTriggerGroup parentTrigger;

    /// <summary>
    /// VIRTUAL PARENT FUNCTION 
    /// To use the trigger on a potential delay using an ability
    /// </summary>
    /// <param name="currentAbility"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    public virtual void Use(float delay)
    {
        //print("Using Ability: " + Ability());
        //Already Using it Check
        if (trigger) return;

        //ability = ability;

        //Mutations
        // + SETS using trigger TRUE
        // + SETS unused FALSE
        // + SETS did reattack FALSE
        // + METHOD -> Enables the Trigger
        // + INVOKE (SPEAKER) -> The initial delay is over -> Renable the trigger
        trigger = true;
        unused = false;
        cf.Controls.didReattack = false;
        EnableTrigger();
        Invoke(nameof(InitialDelayOver_ReEnableTrigger), delay);

        //Individual Triggers (or Individual Abilities) with AF
        if (ability.hasAdditionalFunctionality)
            AbilityExecutor.ExecuteAbility(ability, cf.gameObject);

    }


    /// <summary>
    /// PATHWAY FUNCTION from Use() that moves into a VIRTUAL
    /// + PATHWAY FUNCTION
    /// + SETS initial ability use delay over FALSE
    /// </summary>
    public void EnableTrigger()
    {
        EnableTriggerImplementation();

        initialUseDelayOver = false;
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
        initialUseDelayOver = true;
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

            initialUseDelayOver = true;

            DisableThisTriggerOnlyLocally();
        }
    }

    protected abstract void DisableThisTriggerImplementation();




    /// <summary>
    /// Use the base.DisableThisTriggerOnlyLocally() at the end of the override, because it using gameObejct.SetActive(false); at the end
    /// </summary>
    public void DisableThisTriggerOnlyLocally()
    {
        print("BlockSys: Disbaling locally");
        DisableThisTriggerLocallyImplementation();

        //print("disabling trigger locally");

        trigger = false;
        unused = false;
        initialUseDelayOver = false;

        gameObject.SetActive(false);
    }

    protected abstract void DisableThisTriggerLocallyImplementation();



    public void InitializeSelf(CombatFunctionality cf, Ability ability)
    {
        //print($"[{gameObject.name}] [TRIGGER] [INIT:SELF] Ability: {ability}");
        this.cf = cf;
        this.ability = ability;
        InitializeSelfImplementation(cf, ability);
    }

    protected abstract void InitializeSelfImplementation(CombatFunctionality cf, Ability abilty);


    protected IEnumerator DisableThisTriggerOnDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DisableThisTriggerOnDelayImplementation();
        DisableThisTrigger();
    }

    protected abstract void DisableThisTriggerOnDelayImplementation();

}
