using UnityEngine;

[CreateAssetMenu(fileName = "MultiAbility", menuName = "ScriptableObjects/Abilities/Multi/Multi Ability")]

public class AbilityMulti : Ability
{
    public GameObject MultiAbilityGroupingPrefab;
    public override GameObject prefab
    {
        get => MultiAbilityGroupingPrefab;
        set => MultiAbilityGroupingPrefab = value;
    }

    public Ability[] abilities;

}
