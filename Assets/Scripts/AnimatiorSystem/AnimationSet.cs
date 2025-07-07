using NUnit.Framework;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationSet 
{
    public int[] _anims;
    public int currAnimationIndx;
    public int defaultAnimIndx = 0;
    public string typeName;
    public virtual int[] Animations
    {
        get => _anims;
        set => _anims = value;

    }

    public virtual System.Type AnimationType => this.GetType();

    public void PopulateAnimations(System.Type AnimationsEnum)
    {
        typeName = GetType().ToString();
        //Debug.Log($"Populating AnimationSet Animations. Type of: {AnimationsEnum}");

        int length = AnimationsEnum.GetEnumValues().GetUpperBound(0);
        _anims = new int[length];

        for (int i = 0; i < length; i++)
        {
            int hash = Animator.StringToHash(AnimationsEnum.GetEnumValues().GetValue(i).ToString());
            _anims[i] = hash;
            //Debug.Log("hash: " + hash);
        }

    }

    public System.Type GetEnums()
    {
        return AnimationType.GetNestedType("Anims");
    }


}

public static class AM 
{
    [System.Serializable]
    public class CyclePackage
    {
        public bool cycling;
        public int length;
        [SerializeField]
        public int curr;
        public float period = 2f;
        public Action increment;


        public CyclePackage(int startIndx, float period, int length)
        {
            cycling = false;
            curr = startIndx;
            this.period = period;
            this.length = length;

            if (length < 1)
            {
                Debug.LogError($"Invalid length {length}. Setting to 1 to prevent errors.");
                this.length = 1;
            }
        }

        public IEnumerator Cycle()
        {
            //Debug.Log($"Starting Cycle: period={period}, length={length}, initial curr={curr}");

            if (period <= 0f)
            {
                Debug.LogError($"Invalid period {period}. Must be positive.");
                cycling = false;
                yield break;
            }
            if (length < 1)
            {
                Debug.LogError($"Invalid length {length}. Must be at least 1.");
                cycling = false;
                yield break;
            }

            cycling = true;
            while (cycling)
            {
                yield return new WaitForSeconds(period);
                curr++;
                if (curr >= length)
                    curr = 0;

                //Debug.Log($"Cycle tick: curr={curr}");
                increment?.Invoke();
            }

            //Debug.Log("Cycle coroutine stopped");
        }
    }

    [System.Serializable]
    public class MovementAnimationSet : AnimationSet
    {
        public enum Anims
        {
            IDLE1,
            IDLE2,
            IDLE3,
            RIGHT,
            LEFT,
            FORWARD,
            BACK,
            DEATH1,
            NONE
        }

        public static readonly Anims[] idles =
        {
            Anims.IDLE1,
            Anims.IDLE2,
            Anims.IDLE3
        };

    }
    [System.Serializable]
    public class AttackAnimationSet : AnimationSet
    {
        public enum Anims
        {
            Atk_Idle,
            Atk_Hit,
            Atk_Overhead_T,
            Atk_Uppercut,
            Atk_Up_L,
            Atk_Up_R,
            Atk_Flat_L,
            Atk_Flat_R,
            Atk_Diag_L,
            Atk_Diag_R,
            NONE
        }
    }

    [System.Serializable]
    public class BlockAnimationSet : AnimationSet
    {
        public enum Anims
        {
            Block_D,
            Block_T,
            Block_L,
            Block_R,
            NONE
        }
    }

}







