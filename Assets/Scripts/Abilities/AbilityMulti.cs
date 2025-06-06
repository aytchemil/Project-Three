using UnityEngine;

[CreateAssetMenu(fileName = "MultiAbility", menuName = "ScriptableObjects/Abilities/Multi/Multi Ability")]

public class AbilityMulti : Ability
{
    public float[] successDelay;

    public GameObject MultiAbilityGroupingPrefab;
    public override GameObject prefab
    {
        get => MultiAbilityGroupingPrefab;
        set => MultiAbilityGroupingPrefab = value;
    }

    public Ability[] abilities;

    public override string AnimName 
    {
        get
        {
            Debug.LogError($"[AbilityMulti] Multi Ability has no Animation for the Parent");
            return null;
        }
    }
}
