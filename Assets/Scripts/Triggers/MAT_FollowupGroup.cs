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
        DisableAllChildTriggers();
    }

    #endregion


    //Changed without knowing
    public override void Use(float delay)
    {
        //print("[TRIGGER] [MLTI-ATK] Used");
        base.Use(delay);

        StartCoroutine(FollowUpUse());
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
            if (chosenChildTrigger.used) { HitAttack(); FinishSingleAttack(); }
            if ((chosenChildTrigger as GeneralAttackTriggerGroup).missedAttack) { FinishSingleAttack(); }
            return true;
        }
        return false;

    }



    /// <summary>
    /// Uses the trigger at index i
    /// </summary>
    /// <param name="currentAbility"></param>
    /// <param name="i"></param>
    void UseChildTrigger(int i)
    {
        print($"[TRIGGER] [MAT] Prog: {i} Initial Delay: {ability.InitialUseDelay[i]} ");
        chosenChildTrigger = triggers[i];
        chosenChildTrigger.gameObject.SetActive(true);


        chosenChildTrigger.Use(ability.InitialUseDelay[i]);
    }

    /// <summary>
    /// base virtual: checks for if the trigger is used or unused
    /// </summary>
    /// <param name="i"></param>
    public virtual void CheckForTriggerUpdates_ReturnDelay(int i)
    {
        if (chosenChildTrigger.used || chosenChildTrigger.unused)
            UpdateTriggerProgress(i);
    }

    #region Methods
    //Methods
    //=================================================================================================================================================

    public virtual IEnumerator FollowUpUse()
    {
        ResetTriggerProg();

        for (int i = 0; i < triggerProgress.Length; i++)
        {
            SetAllTriggersToFalse();
            UseChildTrigger(i);


            while (triggerProgress[i] == false)
            {
                if (!gameObject.activeSelf) {  print("This trigger has been disabled, breaking out of loop");   yield break;  }

                CheckForTriggerUpdates_ReturnDelay(i);

                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForSeconds(DelayNextTrigger(i));

            UpdateTriggerProgress(i);

            //Last Ability in Combo 
            if (i == triggerProgress.Length - 1)
            {
                print("[MAT] -> LAST");

                if (chosenChildTrigger.used) { HitAttack(); FinishSingleAttack(); yield break; }
                if ((chosenChildTrigger as GeneralAttackTriggerGroup).missedAttack) { FinishSingleAttack(); yield break; }
            }
        }
    }

    protected virtual void UpdateTriggerProgress(int i)
    {
        //print($"[MAT] Updating Trigger Progress: {i} = TRUE");
        triggerProgress[i] = true;
    }

    protected void ResetTriggerProg()
    {
        for (int i = 0; i < triggerProgress.Length; i++)
            triggerProgress[i] = false;
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

        print($"[MAT] Success/Unsuccess Delay {delay}");

        return delay;
    }

    #endregion

}
