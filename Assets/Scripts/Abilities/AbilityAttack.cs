using System;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "AttackAbility", menuName = "ScriptableObjects/Abilities/Attack Ability")]
public class AbilityAttack : Ability, IAbilityAnims, IAbilityDirectional
{
    public Type type { get => typeof(AM.AtkAnims); }
    public int Enum { get => (int)Attack; }
    public IAbilityDirectional.Direction Dir { get => dir; }

    [SerializeField] private IAbilityDirectional.Direction dir;

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
