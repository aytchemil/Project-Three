using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTriggerFollowUp : AttackTriggerMulti
{
    public List<bool> triggerProgress;

    #region Override Methods
    //Overide Methods
    //=================================================================================================================================================

    protected override void TakeOnChildrenAttackTriggers()
    {
        base.TakeOnChildrenAttackTriggers();

        foreach (var trigger in triggers)
            triggerProgress.Add(new bool());
    }

    protected override void Reset()
    {
        base.Reset();

        for (int i = 0; i <= triggers.Count - 1; i++)
            triggerProgress[i] = false;

    }

    #endregion



    public override void StartUsingAbilityTrigger(Ability currentAbility, float delay)
    {
        print("started using follow up atack");
        base.StartUsingAbilityTrigger(currentAbility, delay);

        StartCoroutine(FollowUpUse(currentAbility as AttackingAbility));
    }



    void SetAllTriggersToFalse()
    {
        for (int i = 0; i < triggers.Count - 1; i++)
        {
            triggerProgress[i] = false;
            triggers[i].gameObject.SetActive(false);
        }
    }

    void TakeOnTriggerBeingUsed(AttackingAbility currentAbility, int i)
    {
        print($"current follow up index: {i} given ability {currentAbility} ");
        triggerBeingUsed = triggers[i];
        triggerBeingUsed.gameObject.SetActive(true);
        triggerBeingUsed.StartUsingAbilityTrigger(currentAbility, currentAbility.initialAttackDelay[i]);
    }


    public virtual float CheckForTriggerUpdates_ReturnDelay(int i)
    {
        float delay = (triggerBeingUsed.myAbility as AttackingAbility).initialAttackDelay[i];

        if (triggerBeingUsed.used)
        { 
            print("following up... hit");
            triggerProgress[i] = true;
        }

        //miss
        if (triggerBeingUsed.unused)
        {
            print("following up... miss");
            triggerProgress[i] = true;
        }

        return delay;
    }

    #region Methods
    //Methods
    //=================================================================================================================================================

    public virtual IEnumerator FollowUpUse(AttackingAbility currentAbility)
    {
        print("FollowUpAttack()");

        //Set them all to false
        SetAllTriggersToFalse();

        for (int i = 0; i < triggerProgress.Count; i++)
        {
            //Applies the trigger on whatever current index its on


            foreach(var trigger in triggers)
                trigger.gameObject.SetActive(false);


            TakeOnTriggerBeingUsed(currentAbility, i);

      
            print($"FOLLOW UP TRIGGER LOOP {i} : " + triggerBeingUsed.name);


            while (triggerProgress[i] == false)
            {
                print($"Waiting for index: {i}");

                if (!gameObject.activeSelf) {  print("This trigger has been disabled, breaking out of loop");   yield break;  }


                yield return new WaitForSeconds(CheckForTriggerUpdates_ReturnDelay(i));



                if (i == triggerProgress.Count - 1) //last
                {
                    print("LAST");

                    if (triggerBeingUsed.used) { HitAttack(); MissAttackCuttoff(); yield break; }
                    if ((triggerBeingUsed as AttackTriggerGroup).missedAttack) { MissAttackCuttoff(); yield break; }
                }

                yield return new WaitForEndOfFrame();
            }

            print("after while loop");

        }

        print("finished follow up attack");

    }



    #endregion

}
