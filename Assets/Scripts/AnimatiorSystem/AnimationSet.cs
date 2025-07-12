using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using static CombatEntityController;

public class AnimationSet
{
    private readonly Type _animationType;
    private readonly int[] _animsCache;
    public int curr;
    public int defaultAnim;

    public AnimationSet(Type animationSetType)
    {
        _animationType = animationSetType ?? throw new ArgumentNullException(nameof(animationSetType));
        _animsCache = AM.AnimsHashes.TryGetValue(animationSetType, out var hashes) ? hashes : Array.Empty<int>();
    }

    /// <summary>
    /// Returns the AnimationSet's Anim Enum from _animEnum
    /// </summary>
    public Type EnumsType => _animationType.GetNestedType("Anims");
    public int[] _anims => _animsCache;
}

public static class AM
{
    // Dictionary, (KEY: AnimationType (ex. move, attack, etc), VALUE: animation's enum's hash)
    public static readonly Dictionary<Type, int[]> AnimsHashes;

    static AM()
    {
        // + Initialize dictionary of (KEY animationtype VALUE hashes)
        // + Ienumerable<type> object is created (basically a list)
        //      that looks for AM's nested types (GetNestedTypes) that are only PUBLIC and STATIC
        //      And .Where() the types it finds (t) that itself has a .GetNestedType() called Anims (and its an ENUM)
        AnimsHashes = new Dictionary<Type, int[]>();
        IEnumerable<Type> animTypes = typeof(AM).GetNestedTypes(BindingFlags.Public | BindingFlags.Static)
            .Where(t => t.GetNestedType("Anims")?.IsEnum == true);

        foreach (Type animType in animTypes)
        {
            Type animEnum = animType.GetNestedType("Anims");
            Enum[] _enums = Enum.GetValues(animEnum).Cast<Enum>().ToArray();
            int[] hashes = Array.ConvertAll<Enum, int>(_enums, anim => Animator.StringToHash(anim.ToString()));
            AnimsHashes[animType] = hashes;
        }
    }
    public static Type GetEnumsType(Type animationType)
    {
        return animationType.GetNestedType("Anims");
    }

    public static Array GetEnums(Type EnumType)
    {
        return Enum.GetValues(EnumType);
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

    [System.Serializable]
    public class FollowUpPackage
    {
        public bool[] triggerProg;
        public Enum[] Enums;
        CombatEntityModeData mode;
        Type type;
        Type EnumType;
        int layer;
        bool locklayer;
        bool bypassLock;
        float crossfade;

        public FollowUpPackage(ModeTriggerGroup trigger, CombatEntityModeData _mode, Enum[] _Enums, Type type, Type _EnumType, int layer, bool locklayer, bool bypassLock, float crossfade)
        {
            triggerProg = trigger.GetComponent<MAT_FollowupGroup>().triggerProgress;
            mode = _mode ?? throw new ArgumentNullException(nameof(_mode));
            this.Enums = _Enums;
            this.type = type;
            this.EnumType = _EnumType;
            this.layer = layer;
            this.locklayer = locklayer;
            this.bypassLock = bypassLock;
            this.crossfade = crossfade;
        }

        public IEnumerator PlayFollowUp(System.Action<Type, int, int, bool, bool, float> Play)
        {
            for (int i = 0; i < triggerProg.Length; i++)
            {
                //Debug.Log($"[AS] Followup {i}");
                int EnumsIndx = (int)Enum.ToObject(EnumType, Enums[i]);

                Play?.Invoke(type, (int)AM.GetEnums(EnumType).GetValue(EnumsIndx), layer, locklayer, bypassLock, crossfade);
                //Debug.Log($"[AS] [FOLLOWUP] [PACKG] PLAYING {AM.GetEnums(EnumType).GetValue(EnumsIndx)}");

                if (!mode.isUsing)
                {
                    i = triggerProg.Length;
                    yield break;
                }

                while (triggerProg[i] == false)
                    yield return new WaitForEndOfFrame();
            }
        }
    }

    public static class MoveAnims
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

    public static class AtkAnims
    {
        public enum Anims
        {
            Atk_Idle,
            Atk_Hit,
            Atk_Overhead_T,
            Atk_Up_D,
            Atk_Up_L,
            Atk_Up_R,
            Atk_Flat_L,
            Atk_Flat_R,
            Atk_Diag_L,
            Atk_Diag_R,
            NONE
        }
    }

    public static class BlkAnims
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