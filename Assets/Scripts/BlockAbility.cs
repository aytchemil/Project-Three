using UnityEngine;

[CreateAssetMenu(fileName = "BlockAbility", menuName = "ScriptableObjects/Abilities/Block Ability")]
public class BlockAbility : Ability
{
    public enum Archetype
    {
        Regular = 0,
    }
    public Archetype archetype;
    public enum Collision
    {
        Box = 0,
    }

    public GameObject blockTriggerCollider;
    public float damagePercentageBlocked;
    public float slowdownMovementPercentage;
}
