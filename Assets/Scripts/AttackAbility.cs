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
    public enum Trait
    {
        MovementForward = 4,
        MovementLeftOrRight = 5,
        DoubleFrontSlash = 6,
    }
    public Trait trait;
    public GameObject attackTriggerCollider;

    public float movementAmount;

}
