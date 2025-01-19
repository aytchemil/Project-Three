using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "AttackAbility", menuName = "ScriptableObjects/Abilities/Attack Ability")]
public class AbilityAttack : Ability
{
    public float damage;
    public float flinchAmount = 1f;

    public GameObject triggerCollider;
    public override GameObject prefab
    {
        get => triggerCollider;
        set => triggerCollider = value;
    }

}
