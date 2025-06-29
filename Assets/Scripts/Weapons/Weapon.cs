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

    [System.Serializable]
    public struct NamedAnimation
    {
        public string animationName; // e.g., "attack left", "attack right", "uppercut"
        public float speed;
        public AnimationClip clip;    // The corresponding animation clip

        public AnimationClip GetAnimationClip()
        {
            return clip;
        }
    }
    public NamedAnimation[] animations;


    public NamedAnimation GetAnimation(string name)
    {
        for(int i = 0; i < animations.Length; i++)
        {
            if (animations[i].animationName == name)
                return animations[i];
        }

        Debug.LogError($"No Animation of [{name}] found.");
        return new NamedAnimation();
    }
}
