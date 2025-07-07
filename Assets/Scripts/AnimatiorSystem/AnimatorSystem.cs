using NUnit.Framework;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationLayer
{
    public int curr;
    [SerializeField] public AnimationSet[] sets;
    public bool locked;

    public AnimationLayer(System.Type[] _sets)
    {
        sets = new AnimationSet[_sets.Length];

        for(int i = 0; i < _sets.Length; i++)
        {
            //Checks
            if (_sets[i] == null)
            {
                Debug.LogError($"Type at index {i} is null.");
                continue;
            }
            if (!typeof(AnimationSet).IsAssignableFrom(_sets[i]))
            {
                Debug.LogError($"Type {_sets[i].Name} at index {i} is not derived from AnimationSet.");
                continue;
            } // Verify that the type derives from AnimationSet

            sets[i] = (AnimationSet)Activator.CreateInstance(_sets[i]);
            if (sets[i] == null)
            {
                Debug.LogError($"Failed to create instance of {_sets[i].Name} at index {i}.");
                continue;
            } //Completed Check

            // Initialize the AnimationSet by populating animations
            System.Type animsType = sets[i].GetEnums();

            if (animsType != null)
            {
                sets[i].PopulateAnimations(animsType);
            } //Completed Check
            else
            {
                Debug.LogError($"No 'Anims' enum found for {_sets[i].Name}.");
            }

            //Mutations
            sets[i].currAnimationIndx = sets[i].defaultAnimIndx;
        }

        locked = false;
        curr = 0;
    }

    public void ChangeCurrSet(System.Type animationType)
    {
        if (animationType == sets[curr].GetType())
            return;
        else
            for (int i = 0; i < sets.Length; i++)
                if (sets[i].GetType() == animationType)
                    curr = i;
    }
}

[System.Serializable]
public class AnimatorSystem : MonoBehaviour
{
    public Animator animator;

    [SerializeField] public AnimationLayer[] layers;
    public Action<int> DefaultAnimation;

    public void InitializeAnimationSystem(int amountOfLayers, System.Type[] sets, Animator animator)
    {
        layers = new AnimationLayer[amountOfLayers];

        for (int i = 0; i < amountOfLayers; i++)
            layers[i] = new AnimationLayer(sets);
    }

    public void SetLocked(bool lockTheLayer, int layer)
    {
        layers[layer].locked = lockTheLayer;
    }

    public void Play(object animation, int layer, bool lockLayer, bool bypassLock, float crossfade = 0.2f)
    {
        //Setup
        layers[layer].ChangeCurrSet(animation.GetType());
        AnimationSet set = layers[layer].sets[layers[layer].curr];
        int animationIndx = Convert.ToInt32(animation);

        //print($"play: layer: {layer} --- [old " + set.currAnimationIndx + " new " + animationIndx + "]");

        if (animationIndx == set.currAnimationIndx) return;
        if (layers[layer].locked && !bypassLock) return;


        // Functionality
        animator.CrossFade(set.Animations[animationIndx], crossfade, layer);
        //print($"[{gameObject.name}] Playing Animation COMPLETED,  [set {set}] [animation {set.Animations[animationIndx]}] [layer {layer}]");

        // Mutations
        layers[layer].locked = lockLayer;
        layers[layer].sets[layers[layer].curr].currAnimationIndx = animationIndx;
    }

}
