using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class MAT_ChoiceGroup : MultiAttackTriggerGroup
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
            print("combo");
            StartCoroutine(ComboOffOfHitNowAvaliable());
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


    public virtual IEnumerator MultiChoiceAttack(AbilityMulti multiAbility, float delay, string choice)
    {
        print("Attacking with multi attack choice trigger");

        triggerBeingUsed = null;

        //CHOSE THE TRIGGER
        triggerBeingUsed = ChooseTrigger(choice);

        if (triggerBeingUsed == null) { print("Chosen attack trigger choice unavaliable, returning"); yield break;  }

        print("Chosen attack trigger is : " + triggerBeingUsed.name);

        //MultiChoice's Ability Trigger
        StartUsingAbilityTrigger(multiAbility, multiAbility.initialUseDelay[0]);

        if (!initializedChildTriggers)
            InitializeChildTriggers(myMultiAbility);

        //The chosen Ability Trigger's Ability
        triggerBeingUsed.gameObject.SetActive(true);
        triggerBeingUsed.StartUsingAbilityTrigger(triggerBeingUsed.myAbility, multiAbility.initialUseDelay[0]);

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
