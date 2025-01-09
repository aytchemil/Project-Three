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







    #region Methods
    //Methods
    //=================================================================================================================================================

    public virtual IEnumerator FollowUpAttack(AttackAbility currentAbility)
    {
        StartUsingAbilityTrigger(currentAbility, currentAbility.initialAttackDelay[0]);

        //Set them all to false
        for (int i = 0; i < triggers.Count-1; i++)
        {
            triggerProgress[i] = false;
            triggers[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < triggerProgress.Count; i++)
        {
            print(i + " :");
            usingAttackTrigger = triggers[i];
            print("attacking with : " + usingAttackTrigger.name);
            usingAttackTrigger.gameObject.SetActive(true);
            usingAttackTrigger.StartUsingAbilityTrigger(currentAbility, currentAbility.initialAttackDelay[i]);



            while (triggerProgress[i] == false)
            {

                if (!gameObject.activeSelf) yield break;

                if (!usingAttackTrigger.gameObject.activeSelf)
                { triggerProgress[i] = true; }

                if (i == triggerProgress.Count - 1) //last
                {
                    print("LAST");
                    if (usingAttackTrigger.hitAttack) { HitAttack(); ComboOffOfHitNowAvaliable(); DisableThisTrigger(); yield break; }
                    if (usingAttackTrigger.missedAttack) { MissAttackCuttoff(); yield break; }
                }



                //print("Waiting for attack : " + usingAttackTrigger.name + " to finish");


                yield return new WaitForEndOfFrame();
            }
        }

    }

    #endregion

}
