using UnityEngine;

[CreateAssetMenu(fileName = "AttackMultiAbility", menuName = "ScriptableObjects/Abilities/Multi/Attack Multi Ability")]

public class AttackMultiAbility : AttackAbility
{
    public bool presetChildTriggers;
    public Ability[] abilities;
}
