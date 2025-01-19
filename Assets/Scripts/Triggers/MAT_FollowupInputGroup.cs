using UnityEngine;

public class MAT_FollowupInputGroup : MAT_FollowupGroup
{

    public override void CheckForTriggerUpdates_ReturnDelay(int i)
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

    //public override IEnumerator FollowUpUse(AttackingAbility currentAbility)
    //{
    //    print("FollowUpAttack()");

    //    //Set them all to false
    //    SetAllTriggersToFalse();

    //    for (int i = 0; i < triggerProgress.Count; i++)
    //    {
    //        //Applies the trigger on whatever current index its on


    //        foreach (var trigger in triggers)
    //            trigger.gameObject.SetActive(false);


    //        TakeOnTriggerBeingUsed(currentAbility, i);


    //        print($"FOLLOW UP TRIGGER LOOP {i} : " + triggerBeingUsed.name);


    //        while (triggerProgress[i] == false)
    //        {
    //            print($"Waiting for index: {i}");

    //            if (!gameObject.activeSelf) { print("This trigger has been disabled, breaking out of loop"); yield break; }


    //            yield return new WaitForSeconds(CheckForTriggerUpdates_ReturnDelay(i));



    //            if (i == triggerProgress.Count - 1) //last
    //            {
    //                print("LAST");

    //                if (triggerBeingUsed.used) { HitAttack(); MissAttackCuttoff(); yield break; }
    //                if ((triggerBeingUsed as AttackTriggerGroup).missedAttack) { MissAttackCuttoff(); yield break; }
    //            }

    //            yield return new WaitForEndOfFrame();
    //        }

    //        print("after while loop");

    //    }

    //    print("finished follow up attack");

    //}
}
