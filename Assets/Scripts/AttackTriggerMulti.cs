using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTriggerMulti : AttackTriggerGroup
{
    public virtual AttackMultiAbility myAttackMultiAbility { get; set; }
    public override AttackingAbility myAttackingAbility
    {
        get => myAttackMultiAbility;
        set => myAttackMultiAbility = value as AttackMultiAbility;

    }

    public List<ModeTriggerGroup> triggers;
    public ModeTriggerGroup triggerBeingUsed;
    protected bool hasTriggers = false;
    protected bool initializedChildTriggers = false;








    #region Method Overrides
    //Method Overrides
    //============================================================================================================================================================================

    protected override void DisableThisTriggerImplementation()
    {
        //This is here because we want to Rest this MultiAttackTrigger.
        //Resseting it causes all of its OWN members it uses to be reset
        //AND since it has children AttackTriggerColliderSingles that it handles, it must Disable Their Triggers (locally) to
        Reset();

        base.DisableThisTriggerImplementation();
    }

    protected override void InitializeSelfImplementation(CombatFunctionality combatFunctionality, Ability abilty)
    {
        base.InitializeSelfImplementation(combatFunctionality, abilty);

        CreateChildrenTriggers(myAttackMultiAbility.GetAbilities());

        TakeOnChildrenAttackTriggers();

        InitializeChildTriggers(abilty as AttackMultiAbility);
    }

    #endregion






    #region Virtual Methods
    //Virtual Methods
    //==========================================================================================================================================================================

    public virtual void CreateChildrenTriggers(Ability[] abilities)
    {
        foreach (Ability ability in abilities)
        {
            if ((ability as AttackAbility).triggerCollider == null) Debug.LogError("trigger collider not set on ability : " + ability.abilityName);

            print("multi: Creating child trigger: " + (ability as AttackAbility).triggerCollider);
            GameObject newChildTrigger = Instantiate((ability as AttackAbility).triggerCollider, transform, false);
            newChildTrigger.GetComponent<ModeTriggerGroup>().isLocal = true;
        }
    }


    protected virtual void TakeOnChildrenAttackTriggers()
    {
        print("multi: " + gameObject.name + " taking on children");

        for (int i = 0; i < transform.childCount; i++)
            triggers.Add(transform.GetChild(i).GetComponent<ModeTriggerGroup>());


        hasTriggers = true;
    }


    protected virtual void InitializeChildTriggers(AttackMultiAbility attackMultiAbility)
    {
        for (int i = 0; i < triggers.Count; i++)
        {
            print("multi: initialiing trigger: " + triggers[i].name);
            triggers[i].InitializeSelf(combatFunctionality, attackMultiAbility.GetAbilities()[i]);
        }

        initializedChildTriggers = true;
    }

    protected virtual void Reset()
    {
        DisableAllChildTriggers();

        triggerBeingUsed = null;
    }

    #endregion









    #region Methods
    // Methods
    //==========================================================================================================================================================================

    protected void DisableAllChildTriggers()
    {
        //print("Disabling all child triggers");
        foreach (ModeTriggerGroup trigger in triggers)
        {
           // print("locally disabling trigger: " + trigger.gameObject.name);
            trigger.DisableThisTriggerOnlyLocally();
        }

    }

    #endregion

}
