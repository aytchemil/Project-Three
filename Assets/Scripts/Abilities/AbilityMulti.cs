using UnityEngine;

[CreateAssetMenu(fileName = "MultiAbility", menuName = "ScriptableObjects/Abilities/Multi/Multi Ability")]

public class AbilityMulti : Ability
{
    public GameObject MultiAbilityGroupingPrefab;
    public override GameObject ColliderPrefab
    {
        get => MultiAbilityGroupingPrefab;
        set => MultiAbilityGroupingPrefab = value;
    }

    public Ability[] abilities;

}
