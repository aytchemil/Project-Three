using UnityEngine;

[CreateAssetMenu(fileName = "CounterAbility", menuName = "ScriptableObjects/Abilities/Counter Ability")]
public class CounterAbility : AttackMultiAbility
{
    public enum CounterArchetype
    {
        StandingRiposte = 0,
        ParryAndRepositionBackward = 1,
        DeflectIntoAndCounter = 2,
        BlockHoldAndCounter = 3,
    }

    public CounterArchetype counterArchetype;

    public enum CounterCollisionType
    {
        Top = 0,
    }

    public GameObject counterTriggerGroup;
    public float counterUpTime = 1f;

}
