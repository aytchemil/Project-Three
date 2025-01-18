using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class AttackTriggerMultiChoice : AttackTriggerMulti
{

    #region Methods
    // Methods
    //=========================================================================================================================

    ModeTriggerGroup ChooseTrigger(string choice)
    {
        ModeTriggerGroup chosenMode = null;

        print(gameObject.name + " | given choice: " + choice);

        for (int i = 0; i < triggers.Count; i++)
        {
            print($"checking choice: {i} which is choiceName : {triggers[i].gameObject.GetComponent<MultiChoiceOption>().choiceName}");

            if (triggers[i].gameObject.GetComponent<MultiChoiceOption>().choiceName == choice)
            {
                chosenMode = triggers[i];
                return chosenMode;
            }
        }

        Debug.LogError("Choice not made, potentially mismatched choice name comparison, or multichoiceoption script not applied");

        return null;

    }
    
    public virtual bool CheckChoiceForUpdates_AndContinue()
    {
        bool _continue = true;

        if (!gameObject.activeInHierarchy) return false;

        print(triggerBeingUsed);

        //Hit
        if (triggerBeingUsed.used)
        {
            ComboOffOfHitNowAvaliable();
            return false;
        }

        //miss
        if (triggerBeingUsed.unused)
        {
            MissAttackCuttoff();
            return false;
        }

        return _continue;
    }


    public virtual IEnumerator MultiChoiceAttack(AttackMultiAbility mltiAbility, float delay, string choice)
    {
        print("Attacking with multi attack choice trigger");

        triggerBeingUsed = null;

        //CHOSE THE TRIGGER
        triggerBeingUsed = ChooseTrigger(choice);

        if (triggerBeingUsed == null) { print("Chosen attack trigger choice unavaliable, returning"); yield break;  }

        print("Chosen attack trigger is : " + triggerBeingUsed.name);

        //MultiChoice's Ability Trigger
        StartUsingAbilityTrigger(mltiAbility, mltiAbility.initialUseDelay[0]);

        if (!initializedChildTriggers)
            InitializeChildTriggers(myAttackMultiAbility);

        //The chosen Ability Trigger's Ability
        triggerBeingUsed.gameObject.SetActive(true);
        triggerBeingUsed.StartUsingAbilityTrigger(mltiAbility, mltiAbility.initialUseDelay[0]);

        while (gameObject.activeInHierarchy)
        {
            //CHECK IF THE CHOICE HAS FINISHED
            if (!CheckChoiceForUpdates_AndContinue()) yield break;

            yield return new WaitForEndOfFrame();
        }

    }


    protected override void InitializeChildTriggers(AttackMultiAbility attackMultiAbility)
    {
        base.InitializeChildTriggers(attackMultiAbility);

        //Sets the choiceoption names
        for (int i = 0; i < triggers.Count; i++)
        {
            MultiChoiceOption choice = triggers[i].gameObject.AddComponent<MultiChoiceOption>();
            choice.choiceName = (attackMultiAbility as AttackMultiChoiceAbility).choices[i];
        }
    }


    #endregion

}
