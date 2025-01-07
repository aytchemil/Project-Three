using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTriggerMultiChoice : AttackTriggerMulti
{
    public virtual void MultiChoiceAttack(Ability currentAbility, float delay, string choice)
    {
        print("Attacking with multi attack choice trigger");

        usingAttackTrigger = null;

        for (int i = 0; i < triggers.Count; i++)
            if (triggers[i].name == choice)
                usingAttackTrigger = triggers[i];


        print("Chosen attack trigger is : " + usingAttackTrigger.name);

        StartAttackFromAttackTrigger(currentAbility, currentAbility.initialAttackDelay[0]);

        if (!initializedChildTriggers)
            InitializeChildTriggers();

        usingAttackTrigger.gameObject.SetActive(true);
        usingAttackTrigger.StartAttackFromAttackTrigger(currentAbility, currentAbility.initialAttackDelay[0]);

    }

}
