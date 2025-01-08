using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTriggerMultiChoice : AttackTriggerMulti
{
    public virtual void MultiChoiceAttack(AttackAbility currentAttackAbility, float delay, string choice)
    {
        print("Attacking with multi attack choice trigger");

        usingAttackTrigger = null;

        print(gameObject.name +  " | given choice: " + choice);

        for (int i = 0; i < triggers.Count; i++)
            if (triggers[i].name == choice)
                usingAttackTrigger = triggers[i];

        if (usingAttackTrigger == null)
        {
            print("Chosen attack trigger choice unavaliable, returning");
            return;
        }

            print("Chosen attack trigger is : " + usingAttackTrigger.name);

        StartAttackFromAttackTrigger(currentAttackAbility, currentAttackAbility.initialAttackDelay[0]);

        if (!initializedChildTriggers)
            InitializeChildTriggers();

        usingAttackTrigger.gameObject.SetActive(true);
        usingAttackTrigger.StartAttackFromAttackTrigger(currentAttackAbility, currentAttackAbility.initialAttackDelay[0]);

    }

}
