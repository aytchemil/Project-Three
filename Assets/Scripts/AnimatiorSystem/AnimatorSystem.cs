
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



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
    [SerializeField] public AnimationLayer[] layers;
    public Action<int> DefaultAnimation;
    private readonly Dictionary<Type, AnimationSet> _animationSetsDictionary = new Dictionary<Type, AnimationSet>();

    public void InitializeAnimationSystem(int amountOfLayers, Animator animator)
    {
        this.animator = animator ?? throw new ArgumentNullException(nameof(animator));

        // Initialize animationSets
        // + Clear Dictionary
        // + For every animation type (ex. movement, attack, block, etc)
        //      + Null OR doesnt have "Anims" enum -> RETURN
        //      + dictionary.add (KEY: The animation type, VALUE: the animation set)
        _animationSetsDictionary.Clear();
        for (int i = 0; i < animations.Length; i++)
        {
            if (animations[i] == null || animations[i].GetNestedType("Anims") == null) {
                Debug.LogError($"Invalid or missing Anims enum in animation type at index {i}: {animations[i]?.Name}");
                continue;}
            _animationSetsDictionary.Add(animations[i], new AnimationSet(animations[i]));
        }

        // Initialize layers
        // + initialize layers list
        // + foreach layer, set its value to a new animation layer value
        layers = new AnimationLayer[amountOfLayers];
        for (int i = 0; i < amountOfLayers; i++)
            layers[i] = new AnimationLayer();
    }

    public void SetLocked(bool lockLayer, int layer)
    {
        if(lockLayer)
            layers[layer].locked = true;
    }

    public void Play(Type setType, int enumIndx, int layer, bool lockLayer, bool bypassLock, float crossfade = 0.2f, float delay = 0)
    {  
        //wait here the delay, then continue

        //Setup 1
        // + New Animation Set
        // + Get the animation set's value from the dictionary
        AnimationSet set;
        if (!_animationSetsDictionary.TryGetValue(setType, out set))
        {
            Debug.LogError($"No AnimationSet found for type {setType?.Name}");
            return;
        }

        //Setup 2
        // + Get the anim enum from the set's type
        // + Gets the Enum value (ex. NONE, FORWARD, etc) from animEnums & the index
        Type EnumType = set.EnumsType ?? throw new ArgumentNullException($"[AS] No Anims enum found for set {setType.Name}");
        Enum animEnum = (Enum)Enum.ToObject(EnumType, enumIndx);

        //Checks
        // + VALID Layer
        // + VALID (Enum to int) animation value
        // + CHECK for NONE animation
        // + CHECK if the current animation is the same
        // + CHECK if layer is locked, and can't bypass lock
        if (layer < 0 || layer >= layers.Length || layers[layer] == null) 
        {
            Debug.LogError($"Invalid layer index: {layer}");
            return;
        }
        if (enumIndx < 0 || enumIndx >= set._anims.Length)
        {
            Debug.LogError($"Invalid animation value: {enumIndx} for set {set.EnumsType?.Name}");
            return;
        }
        if (animEnum.ToString() == "NONE")
        {
            Debug.Log("[AS] PLAYING NONE ANIMATION");
            return;
        }
        //print("[AS] p");
        if (animEnum.ToString() == layers[layer].currAnimation?.ToString()) { return; }
        if (layers[layer].locked && !bypassLock) { Debug.Log("[AS] Layer Locked, unable to bypass"); return; }

        //print("[AS] Attempting Crossfade");

        // Functionality
        int targetHash = set._anims[enumIndx];
        animator.CrossFade(targetHash, crossfade, layer);


        // Mutations
        layers[layer].locked = lockLayer;
        layers[layer].currAnimation = animEnum;
    }

}