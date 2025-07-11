using System;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "AttackAbility", menuName = "ScriptableObjects/Abilities/Attack Ability")]
public class AbilityAttack : Ability, IAbilityAnims
{
    public Type type { get => typeof(AM.AtkAnims); }
    public int Enum { get => (int)Attack; }

    public float damage;
    public float flinchAmount = 1f;
    public AM.AtkAnims.Anims Attack;
    public GameObject triggerCollider;

    public override GameObject prefab
    {
        get => triggerCollider;
        set => triggerCollider = value;
    }

}
