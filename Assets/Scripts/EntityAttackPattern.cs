using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EntityAttackPattern", menuName = "ScriptableObjects/Attack Pattern")]
public class EntityAttackPattern : ScriptableObject
{
    public List<int> attackDir;
}
