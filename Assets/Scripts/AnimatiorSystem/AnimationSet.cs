using NUnit.Framework;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationSet 
{
    public int[] _anims;
    public int currAnimationIndx;
    public int defaultAnimIndx = 0;
    public virtual string typeName { get => GetType().ToString(); }
    public virtual int[] Animations
    {
        get => _anims;
        set => _anims = value;

    }

    public virtual System.Type AnimationType => this.GetType();

    public void PopulateAnimations(System.Type AnimationsEnum)
    {
        Debug.Log("Populating AnimationSet Animations");

        int length = AnimationsEnum.GetEnumValues().GetUpperBound(0);
        _anims = new int[length];

        for (int i = 0; i < length; i++)
        {
            int hash = Animator.StringToHash(AnimationsEnum.GetEnumValues().GetValue(i).ToString());
            _anims[i] = hash;
            //Debug.Log("hash: " + hash);
        }


    }

    // Non-generic GetAnim using reflection
    public object GetAnim(string name)
    {
        System.Type animsType = AnimationType.GetNestedType("Anims");
        if (animsType == null)
            throw new System.InvalidOperationException("Child class must define an 'Anims' enum.");
        return System.Enum.Parse(animsType, name);
    }

    public System.Type GetEnums()
    {
        return AnimationType.GetNestedType("Anims");
    }

}

public static class AM 
{
    [System.Serializable]
    public class MovementAnimationSet : AnimationSet
    {
        public enum Anims
        {
            IDLE,
            RIGHT,
            LEFT,
            FORWARD,
            BACK,
            NONE
        }
    }
    [System.Serializable]
    public class AttackAnimationSet : AnimationSet
    {
        public enum Anims
        {
            A_IDLE,
            A_HIT,
            A_OVERHEAD_C,
            A_UP_L,
            A_UP_R,
            A_UPPERCUT,
            A_FLAT_L,
            A_FLAT_R,
            A_DIAG_L,
            A_DIAG_R
        }
    }

}







