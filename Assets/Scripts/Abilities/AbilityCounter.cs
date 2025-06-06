using UnityEngine;

[CreateAssetMenu(fileName = "CounterAbility", menuName = "ScriptableObjects/Abilities/Counter Ability")]
public class AbilityCounter : AbilityMulti
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
    public float counterUpTime = 1f;

    public override string AnimName
    {
        get
        {
            Debug.LogError($"[AbilityCounter] Multi Ability has no Animation for the Parent");
            return null;
        }
    }

}
