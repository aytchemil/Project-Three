using UnityEngine;

public class ComboTrigger : AttackTriggerInputFollowThrough
{
    public virtual ComboAbility myComboAbility { get; set; }
    public override AttackMultiAbility myAttackMultiAbility
    {
        get => myComboAbility;
        set => myComboAbility = value as ComboAbility;

    }
}
