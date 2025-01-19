using System.Collections;
using UnityEngine;

public class CombotTriggerGroup : MAT_FollowupGroup
{
    public virtual AbilityCombo myComboAbility { get; set; }
    public override AbilityMulti myMultiAbility
    {
        get => myComboAbility;
        set => myComboAbility = value as AbilityCombo;
    }
    public bool countingToReattackCooldown = false;

    public override void CheckForTriggerUpdates_ReturnDelay(int i)
    {

        if (!combatFunctionality.Controls.waitingToReattack)
            StartCoroutine(WaitForContinuation(i));
        else
        {
            StartCoroutine(CountToReattackCuttoff(i));
        }
        

        if (!(triggerBeingUsed as GeneralAttackTriggerGroup).hitAttack && !(triggerBeingUsed as GeneralAttackTriggerGroup).missedAttack) return;

        //print("checking for continuation");
        if (combatFunctionality.Controls.didReattack)
        {
            //print("followupInput reattack... ");
            triggerProgress[i] = true;

            combatFunctionality.Controls.waitingToReattack = false;
            combatFunctionality.Controls.didReattack = false;
        }

        if (combatFunctionality.Controls.Mode("Combo").data.currentAbility != myComboAbility)
            DisableThisTrigger();


    }


    IEnumerator WaitForContinuation(int i)
    {

        combatFunctionality.Controls.waitingToReattack = true;
        yield return new WaitForEndOfFrame();
        combatFunctionality.Controls.didReattack = false;


        yield return new WaitForSeconds(myComboAbility.reattackTimeUntilReset);

        combatFunctionality.Controls.waitingToReattack = false;


        if (combatFunctionality.Controls.didReattack || triggerProgress[i] == true) yield break;
    }

    IEnumerator CountToReattackCuttoff(int i)
    {
        yield return new WaitForSeconds(myComboAbility.reattackTimeUntilReset);

        if (triggerProgress[i] == false)
            DisableThisTrigger();
    }


    protected override void DisableThisTriggerImplementation()
    {
        combatFunctionality.Controls.waitingToReattack = false;
        combatFunctionality.Controls.didReattack = false;

        base.DisableThisTriggerImplementation();
    }
}
