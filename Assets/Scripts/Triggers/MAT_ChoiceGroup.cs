using System;
using System.Collections;
using System.Collections.Generic;
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

    public virtual IEnumerator WaitForTriggerUpdates()
    {
        while (gameObject.activeInHierarchy)
        {
            //CHECK IF THE CHOICE HAS FINISHED
            if (!Check()) yield break;

            yield return new WaitForEndOfFrame();
        }

        bool Check()
        {
            bool _continue = true;

            if (!gameObject.activeInHierarchy) return false;

            //print(chosenChildTrigger);

            //Hit
            if (chosenChildTrigger.used)
            {
                print("combo");
                StartCoroutine(SuccessfullyFinishedAttacked());
                return false;
            }

            //miss
            if (chosenChildTrigger.unused)
            {
                print("miss");

                FinishSingleAttack();
                return false;
            }

            return _continue;
        }
    }

    public virtual void Use(Ability ability, float delay, out ModeTriggerGroup _chosenChildTrigger, string choice)
    {
        AbilityMultiChoice mltiAbility = (AbilityMultiChoice)ability;
        print($"[CHOICE] {choice}");

        chosenChildTrigger = null;
        chosenChildTrigger = ChooseTrigger(choice);
        if (chosenChildTrigger == null) { print("Chosen attack trigger choice unavaliable, returning"); _chosenChildTrigger = null;  return; }

        // (PARENT TRIGGER) MultiChoice's Ability Trigger
        Use(ability.InitialUseDelay[0], out chosenChildTrigger);
        _chosenChildTrigger = chosenChildTrigger;

        if (!initializedChildTriggers)
            InitializeChildTriggers(myMultiAbility);

        //(CHILD TRIGGER) The chosen Ability Trigger's Ability 
        // + AF
        // + SET child trigger ACTIVE
        // + Start using child trigger
        print("Using Child Trigger");
        if (ability.hasAdditionalFunctionality)
            AbilityExecutor.ExecuteRuntimeAbility(ability, cf.gameObject, typeof(IAEffectRuntime<string>), choice);
        chosenChildTrigger.gameObject.SetActive(true);
        chosenChildTrigger.Use(chosenChildTrigger.ability.InitialUseDelay[0]);

        StartCoroutine(WaitForTriggerUpdates());

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
