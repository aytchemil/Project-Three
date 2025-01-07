using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTriggerMulti : AttackTriggerGroup
{
    public List<AttackTriggerGroup> triggers;
    public AttackTriggerGroup usingAttackTrigger;
    protected bool hasTriggers = false;
    protected bool initializedChildTriggers = false;

    protected virtual void OnEnable()
    {
        if (!hasTriggers)
            TakeOnChildrenAttackTriggers();
    }

    public override void InitSelf(CombatFunctionality combatFunctionality)
    {
        base.InitSelf(combatFunctionality);

        foreach (var trigger in triggers)
            trigger.InitSelf(combatFunctionality);

    }


    protected virtual void TakeOnChildrenAttackTriggers()
    {
        //print("taking on children");

        for (int i = 0; i < transform.childCount; i++)
            triggers.Add(transform.GetChild(i).GetComponent<AttackTriggerGroup>());


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

    protected void DisableAllChildTriggers()
    {
        print("Disabling all child triggers");
        foreach (AttackTriggerGroup trigger in triggers)
        {
            print("locally disabling trigger: " + trigger.gameObject.name);
            trigger.DisableThisTriggerOnlyLocally();
        }

    }

    public override void DisableTrigger()
    {
        //print("disabling trigger");
        Reset();

        base.DisableTrigger();
    }

    protected virtual void Reset()
    {
        DisableAllChildTriggers();

        usingAttackTrigger = null;
    }
}
