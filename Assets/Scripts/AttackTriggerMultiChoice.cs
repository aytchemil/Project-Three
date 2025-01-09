using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTriggerMultiChoice : AttackTriggerMulti
{

    #region Methods
    // Methods
    //=========================================================================================================================


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

        //MultiChoice's Ability Trigger
        StartUsingAbilityTrigger(currentAttackAbility, currentAttackAbility.initialAttackDelay[0]);

        if (!initializedChildTriggers)
            InitializeChildTriggers();

        //The chosen Ability Trigger's Ability
        usingAttackTrigger.gameObject.SetActive(true);
        usingAttackTrigger.StartUsingAbilityTrigger(currentAttackAbility, currentAttackAbility.initialAttackDelay[0]);

    }



    #endregion

}
