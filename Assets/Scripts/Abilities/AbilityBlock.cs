using Sirenix.OdinInspector;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "BlockAbility", menuName = "ScriptableObjects/Abilities/Block Ability")]
public class AbilityBlock : Ability, IAbilityAnims, IAbilityDirectional
{
    public Type type { get => typeof(AM.BlkAnims); }
    public int Enum { get => (int)Block; }
    public IAbilityDirectional.Direction Dir { get => dir; }

    [SerializeField] private IAbilityDirectional.Direction dir;

    public AM.BlkAnims.Anims Block;

    public enum Collision
    {
        Regular,
    }
    public Collision collision;


    public GameObject blockTriggerCollider;
    public override GameObject prefab
    {
        get => blockTriggerCollider;
        set => blockTriggerCollider = value;
    }

    public bool hasBlockUpTime = false;
    [ShowIf("hasBlockUpTime")]
    public float blockUpTime;
    public float damagePercentageBlocked;
    public float slowdownMovementPercentage;
    public string animationUnblock;
    public float defaultBlockTimeToBlocking = 0.5f;
}
