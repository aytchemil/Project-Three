using UnityEngine;

[CreateAssetMenu(fileName = "ComboAbility", menuName = "ScriptableObjects/Abilities/Multi/Combo Ability")]
public class AbilityCombo : AbilityMulti
{
    public float reattackTimeUntilReset;

    public enum ComboType
    {
        Linear = 0,
    }
    public ComboType comboType;
}
