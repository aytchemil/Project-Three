using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "ScriptableObjects/Weapon")]
public class Weapon : ScriptableObject
{
    public AbilitySet chosenAbilitySet;
    public GameObject prefab;
    public AnimatorController animationController;

    public List<AbilitySet> allAbilitySets;

    public AnimatorSystem.Animation[] animations;


    public AnimatorSystem.Animation GetAnimation(string name)
    {
        for(int i = 0; i < animations.Length; i++)
        {
            if (animations[i].name == name)
                return animations[i];
        }

        Debug.LogError($"No Animation of [{name}] found.");
        return new AnimatorSystem.Animation();
    }
}
