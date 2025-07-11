using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAT_FollowupGroup : MultiAttackTriggerGroup
{
    public bool[] triggerProgress;

    #region Override Methods
    //Overide Methods
    //=================================================================================================================================================

    protected override void TakeOnChildrenAttackTriggers()
    {
        base.TakeOnChildrenAttackTriggers();

        triggerProgress = new bool[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
            triggerProgress[i] = false;

    }

    protected override void Reset()
    {
        base.Reset();

        for (int i = 0; i <= triggers.Count - 1; i++)
            triggerProgress[i] = false;

    }

    #endregion


    //Changed without knowing
    public override void Use(Ability ability, float delay)
    {
        print("[MAT_FollowupGroup] started using this ability trigger");
        base.Use(ability, delay);

        StartCoroutine(FollowUpUse(ability));
    }

    /// <summary>
    /// Returns true if its the last trigger
    /// </summary>
    /// <returns></returns>
    public bool IncrementTriggerProgress()
    {
        int currIndx = Array.FindIndex(triggerProgress, 0, b => b == false);

        if (currIndx < triggerProgress.Length)
            triggerProgress[currIndx] = true;
        else
        {
            if (chosenChildTrigger.used) { HitAttack(); MissAttackCuttoff(); }
            if ((chosenChildTrigger as GeneralAttackTriggerGroup).missedAttack) { MissAttackCuttoff(); }
            return true;
        }
        return false;

    }

    /// <summary>
    /// Sets all Triggers to False
    /// </summary>
    public void SetAllTriggersToFalse()
    {
        print($"set all {triggers.Count} triggers to false");
        for (int i = 0; i < triggers.Count - 1; i++)
        {
            triggerProgress[i] = false;
            triggers[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Uses the trigger at index i
    /// </summary>
    /// <param name="currentAbility"></param>
    /// <param name="i"></param>
    void TakeOnchosenChildTrigger(Ability ability, int i)
    {
        //print($"FOLLOWUP Cur INDEX: {i} Ability GROUP: {currentAbility} ");
        chosenChildTrigger = triggers[i];
        chosenChildTrigger.gameObject.SetActive(true);


        chosenChildTrigger.Use(ability, ability.InitialUseDelay[i]);
    }

    /// <summary>
    /// base virtual: checks for if the trigger is used or unused
    /// </summary>
    /// <param name="i"></param>
    public virtual void CheckForTriggerUpdates_ReturnDelay(int i)
    {
        print("checking trigger for usage");
        if (chosenChildTrigger.used)
        { 
            print("following up... hit");
            triggerProgress[i] = true;
        }

        //miss
        if (chosenChildTrigger.unused)
        {
            print("following up... miss");
            triggerProgress[i] = true;
        }

    }

    #region Methods
    //Methods
    //=================================================================================================================================================

    public virtual IEnumerator FollowUpUse(Ability ability)
    {
        //print("FollowUpAttack()");

        //Set them all to false
        SetAllTriggersToFalse();


        for (int i = 0; i < triggerProgress.Length; i++)
        {
            //print("starting to move through child triggers");
            //Applies the trigger on whatever current index its on


            foreach(var trigger in triggers)
                trigger.gameObject.SetActive(false);

            TakeOnchosenChildTrigger(ability, i);



            //print($"FOLLOW UP TRIGGER LOOP {i} : " + chosenChildTrigger.name);


            while (triggerProgress[i] == false)
            {
                //print($"Waiting for index: {i}");

                if (!gameObject.activeSelf) {  print("This trigger has been disabled, breaking out of loop");   yield break;  }

               // print("running trigger update check");
                CheckForTriggerUpdates_ReturnDelay(i);
                

                //Last Ability in Combo 
                if (i == triggerProgress.Length-1) 
                {
                    print("MLTI -> LAST");

                    if (chosenChildTrigger.used) { HitAttack(); MissAttackCuttoff(); yield break; }
                    if ((chosenChildTrigger as GeneralAttackTriggerGroup).missedAttack) { yield return new WaitForSeconds(DelayNextTrigger(i)); MissAttackCuttoff(); yield break; }
                }

                yield return new WaitForEndOfFrame();
            }

            //print("after while loop");

            yield return new WaitForSeconds(DelayNextTrigger(i));

        }
    }


    public virtual float DelayNextTrigger(int prog)
    {
        float delay = 0f;

        if (chosenChildTrigger.used)
        {
            delay = myMultiAbility.successDelay[prog];
        }

        //miss
        if (chosenChildTrigger.unused)
        {
            delay = myMultiAbility.unsuccessDelay[prog];
        }

        return delay;
    }

    public override IEnumerator SuccessfullyFinishedAttacked()
    {
        print("MultiFollowup: Succesfuly finished attack");
        float delay = myMultiAbility.successDelay[triggerProgress.Length];
        print($"{this.name} : Comboing, delay is: {delay}");
        print("ability is: " + myAbility);

        yield return new WaitForSeconds(delay);

        DisableThisTrigger();
    }

    #endregion

}
