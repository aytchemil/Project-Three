using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "ScriptableObjects/Weapon")]
public class Weapon : ScriptableObject
{
    public AbilitySet chosenAbilitySet;
    public GameObject prefab;
    public AnimatorController animationController;

    public List<AbilitySet> allAbilitySets;


}
