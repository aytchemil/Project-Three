
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiAttackTriggerGroup : GeneralAttackTriggerGroup
{
    public virtual AbilityMulti myMultiAbility { get; set; }
    public override Ability ability
    {
        get => myMultiAbility;
        set => myMultiAbility = value as AbilityMulti;

    }

    public List<ModeTriggerGroup> triggers;
    public ModeTriggerGroup chosenChildTrigger;
    protected bool hasTriggers = false;
    protected bool initializedChildTriggers = false;


    public virtual void Use(float delay, out ModeTriggerGroup _chosenChildTrigger)
    {
        _chosenChildTrigger = chosenChildTrigger;
    }





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

    protected override void InitializeSelfImplementation(CombatFunctionality cf, Ability abilty)
    {
        base.InitializeSelfImplementation(cf, abilty);

        CreateChildrenTriggers(myMultiAbility.abilities);

        TakeOnChildrenAttackTriggers();

        InitializeChildTriggers((AbilityMulti)abilty);

        //print("MLTI: Successfully CREATED, TOOK ON, INITIALIZED CHILDREN and SELF");
    }

    #endregion






    #region Virtual Methods
    //Virtual Methods
    //==========================================================================================================================================================================

    public virtual void CreateChildrenTriggers(Ability[] abilities)
    {
        foreach (Ability ability in abilities)
        {
            if (ability == null)
                throw new System.Exception("Ability null");
            if (ability.ColliderPrefab == null) Debug.LogError("prefab not set on ability : " + ability.abilityName);

            //print($"MLTI Ability: {ability} -> Creating CHILD prefab: " + ability.prefab);
            GameObject newChildPrefab = Instantiate(ability.ColliderPrefab, transform, false) ?? throw new System.Exception("MTLI Child prefab not correctly created");
            newChildPrefab.GetComponent<ModeTriggerGroup>().isLocal = true;
            newChildPrefab.GetComponent<ModeTriggerGroup>().parentTrigger = this;
        }
    }


    protected virtual void TakeOnChildrenAttackTriggers()
    {
        //print("multi: " + gameObject.name + " taking on children");

        for (int i = 0; i < transform.childCount; i++)
            triggers.Add(transform.GetChild(i).GetComponent<ModeTriggerGroup>());


        hasTriggers = true;
    }


    protected virtual void InitializeChildTriggers(AbilityMulti multiAbility)
    {
        for (int i = 0; i < triggers.Count; i++)
        {
            //print("multi: initialiing trigger: " + triggers[i].name);
            triggers[i].InitializeSelf(cf, multiAbility.abilities[i]);
        }

        initializedChildTriggers = true;
    }

    protected virtual void Reset()
    {
        DisableAllChildTriggers();

        chosenChildTrigger = null;
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


    /// <summary>
    /// Sets all Triggers to False
    /// </summary>
    public void SetAllTriggersToFalse()
    {
        //print($"[TRIGGER] [MAT] SET all triggers false ({triggers.Count} ct.)");
        for (int i = 0; i < triggers.Count - 1; i++)
            triggers[i].gameObject.SetActive(false);
    }

    #endregion

}
