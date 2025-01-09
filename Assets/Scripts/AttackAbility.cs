using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "AttackAbility", menuName = "ScriptableObjects/Abilities/Attack Ability")]
public class AttackAbility : AttackingAbility
{
    public enum Archetype
    {
        Singular = 0,
        MultiChoice = 1,
        FollowUp = 2,
    }
    public Archetype archetype;
    public enum Collision
    {
        Box = 0,
        Overhead = 1,
        Pierce = 2,
        SideSlash = 3,
        MovementForward = 4,
        MovementLeftOrRight = 5,
        DoubleFrontSlash = 6,
    }
    public Collision collision;
    public GameObject attackTriggerCollider;

    public float movementAmount;

}
