using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "AttackAbility", menuName = "ScriptableObjects/Abilities/Attack Ability")]
public class AbilityAttack : Ability, IAbilityAnims, IAbilityDirectional
{
    public Type type { get => typeof(AM.AtkAnims); }
    public int Enum { get => (int)Attack; }
    public IAbilityDirectional.Direction Dir { get => dir; }

    [BoxGroup("Attack Ability")] [SerializeField] private IAbilityDirectional.Direction dir;
    [BoxGroup("Attack Ability")] public float damage;
    [BoxGroup("Attack Ability")] public float flinchAmount = 1f;
    [BoxGroup("Attack Ability")] public AM.AtkAnims.Anims Attack;
    [BoxGroup("Attack Ability")] [SerializeField] private GameObject triggerCollider;
    [BoxGroup("Attack Ability")] [SerializeField] private GameObject attackedEffect;

    public override GameObject ColliderPrefab 
    {
        get => triggerCollider;
        set => triggerCollider = value;
    }

}
