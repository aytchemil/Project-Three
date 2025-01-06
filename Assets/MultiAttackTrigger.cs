using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MultiAttackTrigger : AttackTriggerGroup
{
    public List<AttackTriggerGroup> triggers;
    public AttackTriggerGroup usingAttackTrigger;
    bool hasTriggers = false;
    bool initializedChildTriggers = false;
    private void OnEnable()
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


    void TakeOnChildrenAttackTriggers()
    {
        print("taking on children");

        for (int i = 0; i < transform.childCount; i++)
        {
            triggers.Add(transform.GetChild(i).GetComponent<AttackTriggerGroup>());
            print("Adding to triggers: " + transform.GetChild(i).name);
        }

        hasTriggers = true;
    }

    public override void StartAttackFromAttackTrigger(Ability currentAbility)
    {
        base.StartAttackFromAttackTrigger(currentAbility);

        usingAttackTrigger = null;

        if (!initializedChildTriggers)
            InitializeChildTriggers();



        string choice = combatFunctionality.Controls.getMoveDirection?.Invoke();

        combatFunctionality.gameObject.GetComponent<Movement>().Lunge(choice, currentAbility.movementAmount);

        print("multi attack trigger, movementatttackrightorleft : lunging in dir " + choice);



        for(int i = 0; i < triggers.Count; i++)
            if (triggers[i].name == choice)
                usingAttackTrigger = triggers[i];

        usingAttackTrigger.gameObject.SetActive(true);
        usingAttackTrigger.StartAttackFromAttackTrigger(currentAbility);

    }


    void InitializeChildTriggers()
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

    void DisableAllChildTriggers()
    {
        foreach (AttackTriggerGroup trigger in triggers)
            trigger.DisableTrigger();

    }

    public override void DisableTrigger()
    {
        DisableAllChildTriggers();

        base.DisableTrigger();
    }
}
