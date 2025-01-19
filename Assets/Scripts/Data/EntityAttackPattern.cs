using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EntityAttackPattern", menuName = "ScriptableObjects/Attack Pattern")]
public class EntityAttackPattern : ScriptableObject
{
    public enum AttackDirection
    {
        right = 0,
        down = 1,
        up = 2,
        left = 3,
    }

    public List<AttackDirection> attackDir;


}
