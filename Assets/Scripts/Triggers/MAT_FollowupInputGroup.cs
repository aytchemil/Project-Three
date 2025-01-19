using UnityEngine;

public class MAT_FollowupInputGroup : MAT_FollowupGroup
{
    public virtual AbilityMultiFolloupInput myMultiFollowInputAbility { get; set; }
    public override AbilityMulti myMultiAbility
    {
        get => myMultiFollowInputAbility;
        set => myMultiFollowInputAbility = value as AbilityMultiFolloupInput;
    }

    public override void AdditionalSetup()
    {
        combatFunctionality.Controls.comboReattackDelay?.Invoke(myMultiFollowInputAbility.reattackTimeUntilReset);
        print($"MAT_FollowupInput: Set up successfull");

    }

    public override void CheckForTriggerUpdates_ReturnDelay(int i)
    {
        print("checking for continuation");
        if (combatFunctionality.Controls.didReattack)
        {
            print("followupInput reattack... ");
            triggerProgress[i] = true;
        }

    }

}
