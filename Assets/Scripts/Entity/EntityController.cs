using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine.Scripting.APIUpdating;

/// <summary>
/// Centralized Controller and controls for every combat entity.
/// </summary>
/// 
public class EntityController : MonoBehaviour
{
    //Rule of thumb : Keep these such that they are always set on the outside, never during gameplay
    public static int AMOUNT_OF_ABIL_SLOTS = 4;
    public enum MoveDirections
    {
        NONE = 0,
        RIGHT = 1,
        LEFT = 2,
        FORWARD = 3,
        BACK = 4,
        JUMP = 5
    }
    public Action<MoveDirections, int> MoveDirection;

    [BoxGroup("Mutatable Variables")] public string mode = "Attack";
    [BoxGroup("Mutatable Variables")] public bool waitingToReattack;
    [BoxGroup("Mutatable Variables")] public bool didReattack = false;
    [BoxGroup("Mutatable Variables")] public bool reattackChecking = false;

    #region Controls and Event Publishers
    //Controls
    public Func<Vector2> look;
    public Func<Vector2> move;
    public Action<Vector2> moveDirInput;
    public Action sprintStart;
    public Action sprintStop;
    public Action lockOn;
    public Action dash;
    public Action<string> useAbility; //param: ?
    public Action switchAbilityMode;

    //Event Publishers
    public Func<EntityController> GetTarget;
    public Action EnterCombat;
    public Action ExitCombat;
    public Action<EntityController> CombatFollowTarget;
    public Action<string> CombatWheelSelectDirection; //param: dir
    public Action<EntityController> TargetDeath;
    public Action blockStart;
    public Action blockStop;
    public Action ResetAttack;
    public Action MissedAttack;
    public Action CompletedAttack;
    public Action<string> BlockedAttack; //param: dir
    public Action<string, Ability> MyAttackWasBlocked; //param dir
    public Action Countered;
    public Action<float> Flinch; //param: flinchTime
    public Func<string> getMoveDirection; //ret: moveDir
    public Action Initialize;
    public Action<ModeTriggerGroup> UseCombatAdditionalFunctionality;
    public Action Death;
    public Action<int>[] abilitySlots = new Action<int>[AMOUNT_OF_ABIL_SLOTS];

    #endregion

    [BoxGroup("Dependancy Injection Variables")] public Transform animControllerParent;
    [BoxGroup("Dependancy Injection Variables")] public List<RuntimeModeData> modes;
    [BoxGroup("Dependancy Injection Variables")] public List<AbilitySet> abilitySetInputs;
    [BoxGroup("Dependancy Injection Variables")] public CharacterAnimationController animController;

    [SerializeField]
    [BoxGroup("Flags")] public bool cantUseAbility => (!isLockedOn || Mode("Attack").isUsing || isFlinching || Mode("Counter").isUsing);
    [BoxGroup("Flags")] public bool dashing;
    [BoxGroup("Flags")] public bool dashOnCooldown;
    [BoxGroup("Flags")] public bool isLockedOn;
    [BoxGroup("Flags")] public bool currentlyRetargetting;
    [BoxGroup("Flags")] public bool isAlive = true;
    [BoxGroup("Flags")] public bool isFlinching = false;
    [BoxGroup("Flags")] public bool usedCombo;
    [BoxGroup("Flags")] public bool targetIsDodging;

    [BoxGroup("Mutatable Variables")][SerializeField] private string _lookDir;
    public string lookDir
    {
        get => _lookDir;
        set
        {
            if (_lookDir != value)
                StartCoroutine(Wait(value));
        }
    }

    IEnumerator Wait(string value)
    {
        while (cantUseAbility)
            yield return new WaitForEndOfFrame();

        _lookDir = value;
        CombatWheelSelectDirection?.Invoke(lookDir);
    }

    Transform modeParent;
    bool modesInitialized = false;

    public virtual void Init(List<ModeData> _modes)
    {
        CreateMyOwnModeInstances(_modes);
        isAlive = true;
        ResetAttack += ResetmyAttack;
    }

    protected virtual void OnEnable()
    {
        useAbility += (input) => 
        {
            if (usedCombo)
            {
                ReAttackCheck(input);
                usedCombo = false;
            }
        };

        Flinch += BaseFlinch;
    }

    protected virtual void OnDisable()
    {
        look = null;
        move = null;
        sprintStart = null;
        sprintStop = null;
        lockOn = null;
        dash = null;
        useAbility = null;
        blockStart = null;
        blockStop = null;
        GetTarget = null;
        CombatWheelSelectDirection = null;
        Flinch -= BaseFlinch;
    }

    private static GameObject debugTextObj;
    private static TextMesh debugTextMesh;

    private static void DebugStepMarker(int step)
    {
        // Create it only once
        if (debugTextObj == null)
        {
            debugTextObj = new GameObject("DebugStepText");
            debugTextMesh = debugTextObj.AddComponent<TextMesh>();

            // Transform
            debugTextObj.transform.position = new Vector3(25f, 19f, -25f);
            debugTextObj.transform.rotation = Quaternion.identity;
            debugTextObj.transform.localScale = new Vector3(6f, 6f, 6f);

            // Style
            debugTextMesh.characterSize = 0.5f;
            debugTextMesh.fontStyle = FontStyle.Bold;
            debugTextMesh.anchor = TextAnchor.MiddleCenter;
            debugTextMesh.alignment = TextAlignment.Center;
            debugTextMesh.color = Color.cyan;
        }

        // Just update the text
        debugTextMesh.text = $"STEP {step}";
    }

    void CreateMyOwnModeInstances(List<ModeData> _modes)
    {
        DebugStepMarker(40); // entered CreateMyOwnModeInstances

        if (modesInitialized) return;

        modes.Clear();
        DebugStepMarker(41);

        CreateAssignModeParent();
        DebugStepMarker(42);

        foreach (ModeData templateData in _modes)
        {
            DebugStepMarker(43);

            RuntimeModeData newMode = new RuntimeModeBuilder()
                .WithData(templateData)
                .WithModeName(templateData.name)
                .WithIndividualParent(modeParent)
                .WithIndividualParentNamed(templateData.name)
                .WithComponentAddedToEntityController(gameObject) // <-- LIKELY FAILS HERE
                .Build();

            modes.Add(newMode);
            DebugStepMarker(44);
        }

        AssignAbilitySets();
        DebugStepMarker(45);

        CreateModeParents();
        DebugStepMarker(46);

        modesInitialized = true;
        Debug.Log($"[{gameObject.name}] Copied Modes from ModeManager COMPLETED");

        void CreateAssignModeParent()
        {
            if (modeParent == null)
                modeParent = Instantiate(new GameObject(), transform, false).transform;
            modeParent.name = "Mode Parent";
        }

        void AssignAbilitySets()
        {
            foreach (RuntimeModeData myMode in modes)
            {
                print(myMode);
                print(myMode.data);
                print(myMode.data.abilitySet);
                print(AbilitySet(myMode.data.name));

                myMode.data.abilitySet = AbilitySet(myMode.data.name);   //set abilit sets
                myMode.ability = myMode.data.abilitySet.right;           //Default Setting the ability in the mode
            }
        }

        void CreateModeParents()
        {
            foreach (RuntimeModeData mode in modes)
                if (mode.individualParent == null)
                {
                    // print($"Assigning parent for {mode.data.modeName} ");
                    mode.individualParent = Instantiate(new GameObject(), transform, false).transform;
                    mode.individualParent.rotation = Quaternion.identity;
                    mode.individualParent.name = mode.data.name + " Triggers Parent";
                }
        }

    }


    class RuntimeModeBuilder
    {
        RuntimeModeData runtimeData;

        public RuntimeModeBuilder WithData(ModeData data)
        {
            runtimeData = new RuntimeModeData(data.name);
            runtimeData.data = data;
            DebugStepMarker(2);

            return this;
        }

        public RuntimeModeBuilder WithModeName(string _name)
        {
            runtimeData.name = _name;
            DebugStepMarker(3);

            return this;
        }

        public RuntimeModeBuilder WithIndividualParent(Transform modeParent)
        {
            runtimeData.individualParent = Instantiate(new GameObject(), modeParent, false).transform;
            DebugStepMarker(4);

            return this;
        }

        public RuntimeModeBuilder WithIndividualParentNamed(string _name)
        {
            runtimeData.individualParent.gameObject.name = _name;
            DebugStepMarker(5);

            return this;
        }

        public RuntimeModeBuilder WithComponentAddedToEntityController(GameObject entity)
        {
            DebugStepMarker(6);

            ICombatMode mode = (ICombatMode)entity.AddComponent(ModesRegistery.modes[(int)runtimeData.data.mode]);
            DebugStepMarker(7);

            print("BIG CF (entity)" + entity.name + " cf: " + entity.GetComponent<CombatFunctionality>());
            mode.Init(entity.GetComponent<CombatFunctionality>());
            DebugStepMarker(8);

            runtimeData.functionality = mode;
            return this;
        }

        public RuntimeModeData Build()
        {
            return runtimeData;
        }
    }

    [System.Serializable]
    public class RuntimeModeData
    {
        public static int AMOUNT_OF_TRIGGERS = 4;

        public string name;
        public Ability ability;
        public ModeTriggerGroup trigger;
        public bool isUsing;
        public ModeData data;
        public ICombatMode functionality;
        public Transform individualParent;
        public GameObject[] triggers = new GameObject[AMOUNT_OF_TRIGGERS];
        public bool initializedTriggers;

        public RuntimeModeData(string _mode)
        {
            ability = null;
            name = _mode.Replace("Mode: ", "");
        }

        public void SetAbility(Ability _ability)
        {
            ability = _ability;
        }
    }



    public RuntimeModeData Mode(string mode)
    {
        foreach(RuntimeModeData _mode in modes)
            if(_mode.name == mode)
                return _mode;

        if (modes.Count == 0)
            Debug.LogError($"[{gameObject.name}] No modes detected ");


        //Debug.LogError($"[{gameObject.name}] Could not find a mode of the name [{mode}] in currentModeData");
        //throw new InvalidOperationException("Cannot return a ref to a new struct instance.");
        return null;
    }

    public RuntimeModeData CurMode()
    {
        return Mode(mode);
    }

    public void SetAllModesNotUsing()
    {
        foreach (RuntimeModeData m in modes)
            m.isUsing = false;
    }

    public AbilitySet AbilitySet(string modeName)
    {
        AbilitySet retAbilitySet = null;

        foreach (AbilitySet abilitySet in abilitySetInputs)
        {
            //print(gameObject.name + "Ability Set Search: " + abilitySet.mode.ToString() + " against " + modeName);
            if (abilitySet.mode.ToString() == modeName)
                retAbilitySet = abilitySet;
        }


        if (retAbilitySet == null)
            Debug.LogError(gameObject.name + $"Ability Set Search: Did not find an ability set with the modename {modeName}");


        return retAbilitySet;
    }


    public void ReAttackCheck(string dir)
    {
        if (waitingToReattack)
            didReattack = true;
        else
            didReattack = false;

        //print($"[{gameObject.name}] Reattack check: didReattack = {didReattack}");
    }

    IEnumerator DelayReAttackCheck()
    {
        yield return new WaitForSeconds(0.05f);
        if (waitingToReattack)
            didReattack = true;
        else
            didReattack = false;
    }

    void BaseFlinch(float flinchTime)
    {
        SetAllModesNotUsing();
        ResetAttack?.Invoke();
    }
    void ResetmyAttack()
    {
        //print("Reattack System: RESSETING ATTACKING VARIABLES");
        waitingToReattack = false;
        didReattack = false;
        reattackChecking = false;
        dashing = false;
        Mode("Attack").isUsing = false;
    }

}
