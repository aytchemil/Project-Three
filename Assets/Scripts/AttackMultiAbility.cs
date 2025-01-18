using UnityEngine;

[CreateAssetMenu(fileName = "AttackMultiAbility", menuName = "ScriptableObjects/Abilities/Multi/Attack Multi Ability")]

public class AttackMultiAbility : Ability
{
    public GameObject MultiAbilityGroupingPrefab;
    public override GameObject prefab
    {
        get => MultiAbilityGroupingPrefab;
        set => MultiAbilityGroupingPrefab = value;
    }

    public Ability[] abilities;
}
