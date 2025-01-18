using UnityEngine;

[CreateAssetMenu(fileName = "AttackMultiAbility", menuName = "ScriptableObjects/Abilities/Multi/Attack Multi Ability")]

public class AttackMultiAbility : AttackAbility
{
    protected Ability[] abilities;

    public Ability[] GetAbilities()
    {
        return abilities;
    }
}
