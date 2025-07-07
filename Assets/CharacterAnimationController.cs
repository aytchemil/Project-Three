using System;
using System.Collections;
using UnityEngine;
using static AM;

public class CharacterAnimationController : AnimatorSystem
{
    public float crossFade = 0.4f;
    public CombatEntityController CEC;
    public Weapon wpn;

    public System.Type[] animationSets = { 
        typeof(AM.MovementAnimationSet),
        typeof(AM.AttackAnimationSet),
        typeof(AM.BlockAnimationSet)
    };

    public Action<AM.AttackAnimationSet.Anims, float> AtkAnimation;
    public Action<AM.BlockAnimationSet.Anims, float> BlckAnimation;


    private const int UPPERBODY = 0;
    private const int LOWERBODY = 1;

    [SerializeField]
    public AM.CyclePackage idleCycle = new CyclePackage(
        (int)AM.MovementAnimationSet.Anims.IDLE1,
        2f,
        AM.MovementAnimationSet.idles.Length);


    Vector2 moveDirInput;

    private void Awake()
    {
        CEC = GetComponentInParent<CombatEntityController>();
        CEC.animController = this;
        if (GetComponent<Animator>() == null)
            gameObject.AddComponent<Animator>();
        animator = GetComponent<Animator>();
    }


    public void OnEnable()
    {
        AtkAnimation += AttackAnimation;
        BlckAnimation += BlockAnimation;

        CEC.Flinch += Flinch;
        CEC.Init += Init;
        CEC.moveDirInput += SetMoveDirInput;
        CEC.Death += DeathAnimation;

    }

    public void OnDisable()
    {
        AtkAnimation -= AttackAnimation;
        BlckAnimation -= BlockAnimation;


        CEC.Flinch -= Flinch;
        CEC.Init -= Init;
        CEC.moveDirInput -= SetMoveDirInput;
        CEC.Death -= DeathAnimation;
    }

    void Init()
    {
        string defaultAnim = AM.MovementAnimationSet.Anims.IDLE1.ToString();
        InitializeAnimationSystem(animator.layerCount, animationSets, animator);
    }


    void SetMoveDirInput(Vector2 moveInput)
    {
        //print($"[{gameObject.name}] Setting move direction input: {moveInput}");
        moveDirInput = moveInput;

        CheckInputsLAYER_UPPERBODY(moveInput);
        CheckInputs_LAYER_LOWERBODDY(moveInput);
    }

    private void AttackAnimation(AM.AttackAnimationSet.Anims anim, float delay)
    {
        Play(anim, UPPERBODY, false, false);
    }
    private void BlockAnimation(AM.BlockAnimationSet.Anims anim, float delay)
    {
        print($"BLOCK ANIMATION : {anim}");
        Play(anim, UPPERBODY, false, false);
    }


    public void SetBool(string boolname, bool val)
    {
        animator.SetBool(boolname, val);
    }


    public void Flinch(float time)
    {
        print($"[AnimatorController] [{CEC.gameObject.name}] FlinchAnim");
        animator.Play("Idle");
        //Fix
    }

    protected void DeathAnimation()
    {
        Play(AM.MovementAnimationSet.Anims.DEATH1, UPPERBODY, true, true);
        Play(AM.MovementAnimationSet.Anims.DEATH1, LOWERBODY, true, true);
    }


    void CheckInputsLAYER_UPPERBODY(Vector2 moveInput)
    {
        MoveAnimationsInvoker(UPPERBODY, moveInput);
    }

    void CheckInputs_LAYER_LOWERBODDY(Vector2 moveInput)
    {
        MoveAnimationsInvoker(LOWERBODY, moveInput);
    }

    void MoveAnimationsInvoker(int layer, Vector2 moveInput)
    {
        if (moveInput.y >= 1)
            Play(AM.MovementAnimationSet.Anims.FORWARD, layer, false, false);
        else if (moveInput.y <= -1)
            Play(AM.MovementAnimationSet.Anims.BACK, layer, false, false);
        else if (moveInput.x >= 1)
            Play(AM.MovementAnimationSet.Anims.RIGHT, layer, false, false);
        else if (moveInput.x <= -1)
            Play(AM.MovementAnimationSet.Anims.LEFT, layer, false, false);
        else
        {
            if (!idleCycle.cycling)
                StartCoroutine(idleCycle.Cycle());
            Play(AM.MovementAnimationSet.idles[idleCycle.curr], layer, false, false);
        }
    }


}
