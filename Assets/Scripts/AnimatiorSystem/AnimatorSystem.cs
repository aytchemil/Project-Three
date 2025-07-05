using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public const int UPPERBODY = 0;
    public const int LOWERBODY = 1;
    
    [System.Serializable]
    public struct Animation
    {
        public string name; // e.g., "attack left", "attack right", "uppercut"
        public int hashIndex;
        public float speed;
        public AnimationClip clip;    // The corresponding animation clip

        public AnimationClip GetAnimationClip()
        {
            return clip;
        }
    }

    [System.Serializable]
    public class AnimationLayer
    {
        public Animation currentAnimation;
        public bool locked;

        public AnimationLayer(Animation animation)
        {
            currentAnimation = animation;
            locked = false;
        }
    }


    public Weapon wpn;


    public Animator animator;
    protected AnimationLayer[] currLayer; //Array for layers
    public Action<int> DefaultAnimation;

    protected void InitializeAnimatorSystem(int layers, Animation startAnim, Animator animator, Action<int> DefaultAnimation) 
    {
        //print($"[{gameObject.name}] AnimatiorSystem initialization STARTED...");

        currLayer = new AnimationLayer[layers];
        for (int i = 0; i < layers; i++)
            currLayer[i] = new AnimationLayer(startAnim);

        this.animator = animator;
        this.DefaultAnimation = DefaultAnimation;

        print($"[{gameObject.name}] AnimatiorSystem initialization COMPLETED");

    }

    protected Animation GetCurrentAnimation(int layer)
    {
        return currLayer[layer].currentAnimation;
    } 

    public void SetLocked(bool locklayer, int layer)
    {
        currLayer[layer].locked = locklayer;
    }

    public void Play(string animationName, int layer, bool lockLayer, bool bypassLock, float crossfade = 0.2f)
    {
        if(animationName == "NONE")
        {
            DefaultAnimation(layer);
            return;
        }

        if (currLayer[layer].locked && !bypassLock) return;
        if (currLayer[layer].currentAnimation.name == animationName) return;

        currLayer[layer].locked = lockLayer;
        currLayer[layer].currentAnimation.name = animationName;
        animator.CrossFade(animationName, crossfade, layer);

        print($"[{gameObject.name}] Playing Animation ({animationName}) COMPLETE");
    }


    protected virtual void PlayDefaultAnimation(int layer)
    {
            
    }

}
