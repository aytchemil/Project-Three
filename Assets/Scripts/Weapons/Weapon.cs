using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "ScriptableObjects/Weapon")]
public class Weapon : ScriptableObject
{
    public AbilitySet chosenAbilitySet;
    public GameObject prefab;

    public List<AbilitySet> allAbilitySets;
}
