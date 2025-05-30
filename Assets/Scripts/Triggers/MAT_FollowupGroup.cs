using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAT_FollowupGroup : MultiAttackTriggerGroup
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

        StartCoroutine(FollowUpUse(currentAbility));
    }



    public void SetAllTriggersToFalse()
    {
        print($"set all {triggers.Count} triggers to false");
        for (int i = 0; i < triggers.Count - 1; i++)
        {
            triggerProgress[i] = false;
            triggers[i].gameObject.SetActive(false);
        }
    }

    void TakeOnTriggerBeingUsed(Ability currentAbility, int i)
    {
        //print($"FOLLOWUP Cur INDEX: {i} Ability GROUP: {currentAbility} ");
        triggerBeingUsed = triggers[i];
        triggerBeingUsed.gameObject.SetActive(true);


        triggerBeingUsed.StartUsingAbilityTrigger(triggerBeingUsed.myAbility, currentAbility.initialUseDelay[i]);
    }


    public virtual void CheckForTriggerUpdates_ReturnDelay(int i)
    {
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

    }

    #region Methods
    //Methods
    //=================================================================================================================================================

    public virtual IEnumerator FollowUpUse(Ability currentAbility)
    {
        //print("FollowUpAttack()");

        //Set them all to false
        SetAllTriggersToFalse();


        for (int i = 0; i < triggerProgress.Count; i++)
        {
            print("starting to move through child triggers");
            //Applies the trigger on whatever current index its on


            foreach(var trigger in triggers)
                trigger.gameObject.SetActive(false);


            TakeOnTriggerBeingUsed(currentAbility, i);



            print($"FOLLOW UP TRIGGER LOOP {i} : " + triggerBeingUsed.name);


            while (triggerProgress[i] == false)
            {
                //print($"Waiting for index: {i}");

                if (!gameObject.activeSelf) {  print("This trigger has been disabled, breaking out of loop");   yield break;  }


                CheckForTriggerUpdates_ReturnDelay(i);


                if (i == triggerProgress.Count - 1) //last
                {
                    //print("LAST");

                    if (triggerBeingUsed.used) { HitAttack(); MissAttackCuttoff(); yield break; }
                    if ((triggerBeingUsed as GeneralAttackTriggerGroup).missedAttack) { MissAttackCuttoff(); yield break; }
                }

                yield return new WaitForEndOfFrame();
            }

            //print("after while loop");

            yield return new WaitForSeconds(DelayNextTrigger(i));

        }

        //print("finished follow up attack");

    }

    public virtual float DelayNextTrigger(int prog)
    {
        float delay = 0f;

        if (triggerBeingUsed.used)
        {
            delay = myMultiAbility.successDelay[prog];
        }

        //miss
        if (triggerBeingUsed.unused)
        {
            delay = myMultiAbility.unsuccessDelay[prog];
        }

        return delay;
    }

    public override IEnumerator SuccessfullyFinishedAttacked()
    {
        print("MultiFollowup: Succesfuly finifhsed attack");
        float delay = myMultiAbility.successDelay[triggerProgress.Count - 1];
        print($"{this.name} : Comboing, delay is: {delay}");
        print("ability is: " + myAbility);

        yield return new WaitForSeconds(delay);

        DisableThisTrigger();
    }

    #endregion

}
