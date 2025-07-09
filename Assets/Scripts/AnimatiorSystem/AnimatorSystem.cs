using NUnit.Framework;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class AnimationLayer
{
    public object currAnimation;
    public bool locked;

    public AnimationLayer()
    {
        locked = false;
        currAnimation = "NONE";
    }
}

[System.Serializable]
public class AnimatorSystem : MonoBehaviour
{
    public Animator animator;
    public System.Type[] animations = { };
    public AnimationSet[] animationSets;
    [SerializeField] public AnimationLayer[] layers;
    public Action<int> DefaultAnimation;
    private readonly Dictionary<Type, AnimationSet> _animationSetsDictionary = new Dictionary<Type, AnimationSet>();

    public void InitializeAnimationSystem(int amountOfLayers, Animator animator)
    {
        this.animator = animator ?? throw new ArgumentNullException(nameof(animator));

        // Initialize animationSets and map
        animationSets = new AnimationSet[animations.Length];
        _animationSetsDictionary.Clear();
        for (int i = 0; i < animations.Length; i++)
        {
            if (animations[i] == null || animations[i].GetNestedType("Anims") == null)
            {
                Debug.LogError($"Invalid or missing Anims enum in animation type at index {i}: {animations[i]?.Name}");
                continue;
            }
            animationSets[i] = new AnimationSet(animations[i]);
            _animationSetsDictionary[animations[i]] = animationSets[i];
        }

        // Initialize layers
        layers = new AnimationLayer[amountOfLayers];
        for (int i = 0; i < amountOfLayers; i++)
        {
            layers[i] = new AnimationLayer();
        }
    }

    public void SetLocked(bool lockLayer, int layer)
    {
        if(lockLayer)
            layers[layer].locked = true;
    }

    public void Play(Type animationSetType, int animationValue, int layer, bool lockLayer, bool bypassLock, float crossfade = 0.2f)
    {


        // Validate animationSetType
        if (!_animationSetsDictionary.TryGetValue(animationSetType, out AnimationSet set))
        {
            Debug.LogError($"No AnimationSet found for type {animationSetType?.Name}");
            return;
        }

        // Get the enum type and value
        Type animsType = set.AnimationType;
        if (animsType == null)
        {
            Debug.LogError($"No Anims enum found for set {animationSetType.Name}");
            return;
        }

        // Validate inputs
        if (layer < 0 || layer >= layers.Length || layers[layer] == null)
        {
            Debug.LogError($"Invalid layer index: {layer}");
            return;
        }
        if (animationValue < 0 || animationValue >= set._anims.Length)
        {
            Debug.LogError($"Invalid animation value: {animationValue} for set {set.AnimationType?.Name}");
            return;
        }

        // Check for NONE animation
        var animEnum = (Enum)Enum.ToObject(animsType, animationValue);
        if (animEnum.ToString() == "NONE")
        {
            Debug.Log("[AS] PLAYING NONE ANIMATION");
            return;
        }

        // Check if the current animation is the same or if the layer is locked
        if (animEnum.ToString() == layers[layer].currAnimation?.ToString())
            return;
        if (layers[layer].locked && !bypassLock)
            return;

        // Log the animation being played
        Debug.Log($"[AS] LAYER ({layer}) ({set.AnimationType?.Name}) PLAYING : {animEnum}");

        // Functionality
        animator.CrossFade(set._anims[animationValue], crossfade, layer);

        // Mutations
        layers[layer].locked = lockLayer;
        layers[layer].currAnimation = animEnum;
    }
}