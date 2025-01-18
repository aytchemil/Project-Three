using UnityEngine;

[CreateAssetMenu(fileName = "BlockAbility", menuName = "ScriptableObjects/Abilities/Block Ability")]
public class BlockAbility : Ability
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
