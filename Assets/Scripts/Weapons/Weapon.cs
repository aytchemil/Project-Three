using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "ScriptableObjects/Weapon")]
public class Weapon : ScriptableObject
{
    public AbilitySet chosenAbilitySet;
    public GameObject prefab;

    public List<AbilitySet> allAbilitySets;

    [System.Serializable]
    public struct NamedAnimation
    {
        public string animationName; // e.g., "attack left", "attack right", "uppercut"
        public AnimationClip clip;    // The corresponding animation clip
    }
    public NamedAnimation[] animations;
}
