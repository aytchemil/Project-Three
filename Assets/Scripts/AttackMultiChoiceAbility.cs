using UnityEngine;

[CreateAssetMenu(fileName = "AttackMultiChoiceAbility", menuName = "ScriptableObjects/Abilities/Multi/Attack Multi Choice Ability")]
public class AttackMultiChoiceAbility : AttackMultiAbility
{
    public string[] choices;
}
