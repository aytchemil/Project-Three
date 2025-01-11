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
        base.StartUsingAbilityTrigger(currentAbility, delay);

        StartCoroutine(FollowUpUse(currentAbility as AttackingAbility));
    }



    #region Methods
    //Methods
    //=================================================================================================================================================

    public virtual IEnumerator FollowUpUse(AttackingAbility currentAbility)
    {
        print("FollowUpAttack()");

        //Set them all to false
        for (int i = 0; i < triggers.Count-1; i++)
        {
            triggerProgress[i] = false;
            triggers[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < triggerProgress.Count; i++)
        {
            print("current index on follow up: " + i);
            triggerBeingUsed = triggers[i];
            triggerBeingUsed.gameObject.SetActive(true);
            triggerBeingUsed.StartUsingAbilityTrigger(currentAbility, currentAbility.initialAttackDelay[i]);

            print("using : " + triggerBeingUsed.name);

            while (triggerProgress[i] == false)
            {

                if (!gameObject.activeSelf) yield break;

                //Once the indexed trigger disables itself
                if (!triggerBeingUsed.gameObject.activeSelf)
                { triggerProgress[i] = true; }


                if (i == triggerProgress.Count - 1) //last
                {
                    //print("LAST");

                    if (triggerBeingUsed.used) { HitAttack(); MissAttackCuttoff(); yield break; }
                    if ((triggerBeingUsed as AttackTriggerGroup).missedAttack) { MissAttackCuttoff(); yield break; }
                }



                //print("Waiting for attack : " + usingAttackTrigger.name + " to finish");


                yield return new WaitForEndOfFrame();
            }
        }

    }

    #endregion

}
