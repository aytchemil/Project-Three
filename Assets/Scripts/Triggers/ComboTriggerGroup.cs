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

    Coroutine waitForContinuation = null;
    Coroutine countToReattackCuttoff = null;

    /// <summary>
    /// override for Combo
    /// 
    /// checks for trigger updates (used vs unused)
    /// assigns coroutines to wait for reattacks and continuation attacks
    /// </summary>
    /// <param name="i"></param>
    public override void CheckForTriggerUpdates_ReturnDelay(int i)
    {
        print("Combo checking for reattacks");

        //Couroutine variable null checks/assignments for when the coroutine starts its no longer null, meaning were not spamming start coroutines
        if (!combatFunctionality.Controls.waitingToReattack && waitForContinuation == null)
            waitForContinuation = StartCoroutine(WaitForContinuationAttack(i));
        else if(countToReattackCuttoff == null)
            countToReattackCuttoff = StartCoroutine(CountToReattackCuttoff(i));
    }


    /// <summary>
    /// Timer coroutine for waiting on the continuation attack 
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    IEnumerator WaitForContinuationAttack(int i)
    {
        print("Waiting for a contination");

        //Setup flags,
        // -> is waiting for reattack  (waitingToReattack)
        // -> has not reattacked yet   (didReattack)
        combatFunctionality.Controls.waitingToReattack = true;
        yield return new WaitForEndOfFrame();
        combatFunctionality.Controls.didReattack = false;

        //Waits the reattack delay
        yield return new WaitForSeconds(myComboAbility.reattackTimeUntilReset);

        //Reattack Check
        DidReattack(i);

        //When reattack delay over 
        // -> no longer waiting to reattack
        // -> Couroutine (null check) is reset to null -> allows for couroutine to start over
        combatFunctionality.Controls.waitingToReattack = false;
        waitForContinuation = null;

        print("FINISHED WAITING FOR CONT");
        //Check if we actually did reattack, or if the trigger progress continued by itself, we break out of tihs coroutine 
        //I dont think this is needed
        if (combatFunctionality.Controls.didReattack || triggerProgress[i] == true) yield break;
    }


    public void DidReattack(int i)
    {
        print("Didreattack?");
        if (i == triggerProgress.Length)
        {
            print("didreattack: last ability in triggerproggress.. ignoring reattack check");
            return;
        }
        if (combatFunctionality.Controls.didReattack)
        {
            print("YES");
            triggerProgress[i] = true;

            //Reset reattack flags
            combatFunctionality.Controls.waitingToReattack = false;
            combatFunctionality.Controls.didReattack = false;
        }
    }

    /// <summary>
    /// Timer coroutine to wait for the reattack cuttoff
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    IEnumerator CountToReattackCuttoff(int i)
    {
        print("Waiting for reattack cuttoff");

        //wait out the reattack time until reset attacking + the initial use delay (because the player isnt actually attacking yet)
        yield return new WaitForSeconds(myComboAbility.reattackTimeUntilReset + myComboAbility.InitialUseDelay[i]);

        //If player has not gone on to reattack, Disable the combo parent trigger entirely
        if (triggerProgress[i] == false)
        {
            print("No Reattack Found Disabling combo trigger");
            DisableThisTrigger();
        }
        

        //Coroutine (null check) reset to null -> we want to restart this if its already done 
        // -> I dont think this is needed tho
        countToReattackCuttoff = null;
    }

    /// <summary>
    /// Delay coroutine for changing the combo
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator DelayChangeOfCombo(float delay)
    {
        yield return new WaitForSeconds(delay);
        DisableThisTrigger();
    }


    protected override void DisableThisTriggerImplementation()
    {
        combatFunctionality.Controls.waitingToReattack = false;
        combatFunctionality.Controls.didReattack = false;

        base.DisableThisTriggerImplementation();
    }

    protected override void InitializeSelfImplementation(CombatFunctionality combatFunctionality, Ability abilty)
    {
        base.InitializeSelfImplementation(combatFunctionality, abilty);

        void ResetComboTrigger()
        {
            print("reset trigger: combo trigger");
            
            waitForContinuation = null;
            countToReattackCuttoff = null;
        }

        combatFunctionality.Controls.ResetAttack += ResetComboTrigger;
    }

}
