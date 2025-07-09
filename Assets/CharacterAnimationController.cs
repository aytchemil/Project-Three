using System;
using System.Collections;
using UnityEngine;
using static AM;

public class CharacterAnimationController : AnimatorSystem
{
    public float crossFade = 0.4f;
    public CombatEntityController CEC;
    public Weapon wpn;

    public Action<AM.AttackAnimations.Anims, float> AtkAnimation;
    public Action<AM.BlockAnimations.Anims, float> BlckAnimation;

    public const int UPPERBODY = 0;
    public const int LOWERBODY = 1;

    [SerializeField]
    public AM.CyclePackage idleCycle = new CyclePackage(
        (int)AM.MovementAnimations.Anims.IDLE1,
        2f,
        AM.MovementAnimations.idles.Length
    );


    Vector2 moveDirInput;

    private void Awake()
    {
        CEC = GetComponentInParent<CombatEntityController>();
        CEC.animController = this;
        if (GetComponent<Animator>() == null)
            gameObject.AddComponent<Animator>();
        animator = GetComponent<Animator>();

        animations = new Type[] {
            typeof(AM.MovementAnimations),
            typeof(AM.AttackAnimations),
            typeof(AM.BlockAnimations)
        };
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
        string defaultAnim = AM.MovementAnimations.Anims.IDLE1.ToString();
        InitializeAnimationSystem(animator.layerCount, animator);
        animator.Rebind();
    }


    void SetMoveDirInput(Vector2 moveInput)
    {
        //print($"[{gameObject.name}] Setting move direction input: {moveInput}");
        moveDirInput = moveInput;

        CheckInputsLAYER_UPPERBODY(moveInput);
        CheckInputs_LAYER_LOWERBODDY(moveInput);
    }

    private void AttackAnimation(AM.AttackAnimations.Anims anim, float delay)
    {
        print($"ATTACK ANIMATION : {anim}");
        Play(typeof(AM.AttackAnimations), (int)anim, UPPERBODY, false, false);
    }
    private void BlockAnimation(AM.BlockAnimations.Anims anim, float delay)
    {
        //print($"BLOCK ANIMATION : {anim}");
        Play(typeof(AM.BlockAnimations), (int)anim, UPPERBODY, false, false);
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
        Play(typeof(AM.MovementAnimations), (int)AM.MovementAnimations.Anims.DEATH1, UPPERBODY, true, true);
        Play(typeof(AM.MovementAnimations), (int)AM.MovementAnimations.Anims.DEATH1, LOWERBODY, true, true);
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
            Play(typeof(AM.MovementAnimations), (int)AM.MovementAnimations.Anims.FORWARD, layer, true, true);
        else if (moveInput.y <= -1)
            Play(typeof(AM.MovementAnimations), (int)AM.MovementAnimations.Anims.BACK, layer, true, true);
        else if (moveInput.x >= 1)
            Play(typeof(AM.MovementAnimations), (int)AM.MovementAnimations.Anims.RIGHT, layer, true, true);
        else if (moveInput.x <= -1)
            Play(typeof(AM.MovementAnimations), (int)AM.MovementAnimations.Anims.LEFT, layer, true, true);
        else
        {
            if (!idleCycle.cycling)
                StartCoroutine(idleCycle.Cycle());

            Play(typeof(AM.MovementAnimations), (int)AM.MovementAnimations.idles[idleCycle.curr], layer, true, true);
        }
    }


}
