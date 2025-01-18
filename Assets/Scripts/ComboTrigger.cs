using UnityEngine;

public class ComboTrigger : AttackTriggerInputFollowThrough
{
    public virtual ComboAbility myComboAbility { get; set; }
    public override Ability myAbility
    {
        get => myComboAbility;
        set => myComboAbility = value as ComboAbility;

    }
}
