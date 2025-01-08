using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "AttackAbility", menuName = "ScriptableObjects/Abilities/Attack Ability")]
public class AttackAbility : Ability
{
    public enum AttackArchetype
    {
        Singular = 0,
        MultiChoice = 1,
        FollowUp = 2,
    }
    public AttackArchetype archetype;
    public enum CollisionType
    {
        Box = 0,
        Overhead = 1,
        Pierce = 2,
        SideSlash = 3,
        MovementForward = 4,
        MovementLeftOrRight = 5,
        DoubleFrontSlash = 6,
    }
    public CollisionType collisionType;
    public GameObject attackTriggerCollider;

    public float[] initialAttackDelay = { 0.3f };
    public float missDelayUntilAbleToAttackAgain = 0.6f;

    public float damage;
    public float movementAmount;

}
