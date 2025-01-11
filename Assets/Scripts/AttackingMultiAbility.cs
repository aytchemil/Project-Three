using UnityEngine;

[CreateAssetMenu(fileName = "AttackingMultiAbility", menuName = "ScriptableObjects/Abilities/Multi Attacking Ability")]

public class AttackingMultiAbility : AttackAbility
{
    public bool presetChildTriggers;
    public Ability[] abilities;
}
