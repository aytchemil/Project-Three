using UnityEngine;

[CreateAssetMenu(fileName = "BlockAbility", menuName = "ScriptableObjects/Abilities/Block Ability")]
public class AbilityBlock : Ability
{
    public enum Collision
    {
        Top = 0,
        Left = 1,
        Right = 2,
        Bottom = 3,
        LeftAndRight = 4,
        Front = 5,
    }
    public Collision collision;

    public enum AnimationConnection
    {
        Idle = 0,
        Block_Top = 1
    }
    public AnimationConnection anim_name;

    public GameObject blockTriggerCollider;
    public override GameObject prefab
    {
        get => blockTriggerCollider;
        set => blockTriggerCollider = value;
    }
    public float blockUpTime;
    public float damagePercentageBlocked;
    public float slowdownMovementPercentage;
}
