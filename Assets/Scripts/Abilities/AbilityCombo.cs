using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ComboAbility", menuName = "ScriptableObjects/Abilities/Multi/Combo Ability")]
public class AbilityCombo : AbilityMulti
{
    public float reattackTimeUntilReset;

    [HideInInspector]
    public override float maxInitialUseDelay => reattackTimeUntilReset;

    public enum ComboType
    {
        Linear = 0,
    }
    public ComboType comboType;

}
