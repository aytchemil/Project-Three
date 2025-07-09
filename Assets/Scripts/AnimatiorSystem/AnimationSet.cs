using NUnit.Framework;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationSet
{
    private readonly Type _animationSetType;
    private readonly int[] _animsCache;
    public int curr;
    public int defaultAnim;

    public AnimationSet(Type animationSetType)
    {
        _animationSetType = animationSetType ?? throw new ArgumentNullException(nameof(animationSetType));

        // Compute and cache the hashes during construction
        Array animsArr = AnimationType != null ? Enum.GetValues(AnimationType) : Array.Empty<Enum>();
        _animsCache = Array.ConvertAll(animsArr.Cast<Enum>().ToArray(), anim => Animator.StringToHash(anim.ToString()));
    }

    public Type AnimationType => _animationSetType.GetNestedType("Anims");

    public Array AnimsArr => AnimationType != null ? Enum.GetValues(AnimationType) : Array.Empty<Enum>();

    public int[] _anims => _animsCache;
}

public static class AM
{
    // Dictionary to store cached hashes for each animation set's Anims enum
    public static readonly Dictionary<Type, int[]> AnimsHashes;

    static AM()
    {
        // Initialize the dictionary and cache hashes for all nested animation sets
        AnimsHashes = new Dictionary<Type, int[]>();
        var animationSetTypes = typeof(AM).GetNestedTypes(BindingFlags.Public | BindingFlags.Static)
            .Where(t => t.GetNestedType("Anims") != null);

        foreach (var setType in animationSetTypes)
        {
            var animsType = setType.GetNestedType("Anims");
            var anims = Enum.GetValues(animsType).Cast<Enum>().ToArray();
            var hashes = Array.ConvertAll(anims, anim => Animator.StringToHash(anim.ToString()));
            AnimsHashes[setType] = hashes;
        }
    }

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

                increment?.Invoke();
            }
        }
    }

    public static class MovementAnimations
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

    public static class AttackAnimations
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

    public static class BlockAnimations
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