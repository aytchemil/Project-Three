using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockAbility", menuName = "ScriptableObjects/Abilities/Block Ability")]
public class AbilityBlock : Ability
{
    public AM.BlockAnimations.Anims Block;

    public enum Type
    {
        Regular = 0
    }
    public Type type;

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
