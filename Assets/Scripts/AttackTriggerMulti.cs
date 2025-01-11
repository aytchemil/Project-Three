using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTriggerMulti : AttackTriggerGroup
{
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

        if(!(abilty as AttackingMultiAbility).presetChildTriggers)
            CreateChildrenTriggers((abilty as AttackingMultiAbility).abilities);

        TakeOnChildrenAttackTriggers();

        foreach (var trigger in triggers)
            trigger.InitializeSelf(combatFunctionality, abilty);
    }

    #endregion






    #region Virtual Methods
    //Virtual Methods
    //==========================================================================================================================================================================

    public virtual void CreateChildrenTriggers(Ability[] abilities)
    {
        foreach (Ability ability in abilities)
        {
             
            print("ability: " + (ability));
            print("Creating child trigger for ability: " + (ability as AttackAbility));
            print("Creating child trigger: " + (ability as AttackAbility).triggerCollider);
            GameObject newChildTrigger = Instantiate((ability as AttackAbility).triggerCollider, transform, false);
            newChildTrigger.GetComponent<ModeTriggerGroup>().isLocal = true;
        }
    }


    protected virtual void TakeOnChildrenAttackTriggers()
    {
        print(gameObject.name + " taking on children");

        for (int i = 0; i < transform.childCount; i++)
            triggers.Add(transform.GetChild(i).GetComponent<ModeTriggerGroup>());


        hasTriggers = true;
    }


    protected virtual void InitializeChildTriggers()
    {
        foreach (AttackTriggerGroup trigger in triggers)
        {
            print("initialiing trigger: " + trigger.name);

            print(gameObject.name + " my combat functionality is : " + combatFunctionality);

            trigger.combatFunctionality = combatFunctionality;
            trigger.attacking = false;
            trigger.gameObject.SetActive(false);

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
