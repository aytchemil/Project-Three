using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTriggerMultiChoice : AttackTriggerMulti
{

    #region Methods
    // Methods
    //=========================================================================================================================


    public virtual IEnumerator MultiChoiceAttack(AttackAbility currentAttackAbility, float delay, string choice)
    {
        print("Attacking with multi attack choice trigger");

        triggerBeingUsed = null;

        print(gameObject.name +  " | given choice: " + choice);

        for (int i = 0; i < triggers.Count; i++)
            if (triggers[i].name == choice)
                triggerBeingUsed = triggers[i];

        if (triggerBeingUsed == null)
        {
            print("Chosen attack trigger choice unavaliable, returning");
            yield break;
        }

            print("Chosen attack trigger is : " + triggerBeingUsed.name);

        //MultiChoice's Ability Trigger
        StartUsingAbilityTrigger(currentAttackAbility, currentAttackAbility.initialAttackDelay[0]);

        if (!initializedChildTriggers)
            InitializeChildTriggers();

        //The chosen Ability Trigger's Ability
        triggerBeingUsed.gameObject.SetActive(true);
        triggerBeingUsed.StartUsingAbilityTrigger(currentAttackAbility, currentAttackAbility.initialAttackDelay[0]);

        while (gameObject.activeInHierarchy)
        {
            if ((triggerBeingUsed as AttackTriggerGroup).hitAttack)
            {
                ComboOffOfHitNowAvaliable();
                yield break;
            }


            //miss
            if ((triggerBeingUsed as AttackTriggerGroup).missedAttack)
            {
                MissAttackCuttoff();
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }

    }



    #endregion

}
