using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEditor.Playables;
using UnityEngine;

public class MAT_ChoiceGroup : MultiAttackTriggerGroup
{

    #region Methods
    // Methods
    //=========================================================================================================================

    ModeTriggerGroup ChooseTrigger(string choice)
    {
        for (int i = 0; i < triggers.Count; i++)
            if (triggers[i].gameObject.GetComponent<MultiChoiceOption>().choiceName == choice)
                return triggers[i];

        return null;
    }

    public virtual bool CheckChoiceForUpdates_AndContinue()
    {
        bool _continue = true;

        if (!gameObject.activeInHierarchy) return false;

        //print(triggerBeingUsed);

        //Hit
        if (triggerBeingUsed.used)
        {
            print("combo");
            StartCoroutine(SuccessfullyFinishedAttacked());
            return false;
        }

        //miss
        if (triggerBeingUsed.unused)
        {
            print("miss");

            MissAttackCuttoff();
            return false;
        }

        return _continue;
    }

    public virtual IEnumerator MultiChoiceAttack(AbilityWrapper usingAbility, float delay, string choice, AbilityWrapper location)
    {
        print("[MAT_ChoiceGroup] Attacking with multi attack choice trigger");

        triggerBeingUsed = null;

        //CHOSE THE TRIGGER
        triggerBeingUsed = ChooseTrigger(choice);

        if (triggerBeingUsed == null) { print("Chosen attack trigger choice unavaliable, returning"); yield break;  }

        print("Chosen attack trigger is : " + triggerBeingUsed.name);

        // (PARENT TRIGGER)
        //MultiChoice's Ability Trigger
        StartUsingAbilityTrigger(usingAbility, usingAbility.parentAbility.InitialUseDelay[0]);

        if (!initializedChildTriggers)
            InitializeChildTriggers(myMultiAbility);

        //(CHILD TRIGGER)
        //The chosen Ability Trigger's Ability 
        triggerBeingUsed.gameObject.SetActive(true);

        location.abilities.Add(triggerBeingUsed.myAbility);
        triggerBeingUsed.StartUsingAbilityTrigger(usingAbility, usingAbility.abilities[0].InitialUseDelay[0]);

        while (gameObject.activeInHierarchy)
        {
            //CHECK IF THE CHOICE HAS FINISHED
            if (!CheckChoiceForUpdates_AndContinue()) yield break;

            yield return new WaitForEndOfFrame();
        }

    }


    protected override void InitializeChildTriggers(AbilityMulti multiAbility)
    {
        base.InitializeChildTriggers(multiAbility);

        //Sets the choiceoption names
        for (int i = 0; i < triggers.Count; i++)
        {
            MultiChoiceOption choice = triggers[i].gameObject.AddComponent<MultiChoiceOption>();
            choice.choiceName = (multiAbility as AbilityMultiChoice).choices[i];
        }
    }


    #endregion

}
