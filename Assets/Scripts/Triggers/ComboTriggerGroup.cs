using UnityEngine;

public class ComboTriggerGroup : MAT_FollowupInputGroup
{
    public virtual AbilityCombo myComboAbility { get; set; }
    public override Ability myAbility
    {
        get => myComboAbility;
        set => myComboAbility = value as AbilityCombo;
    }
}
