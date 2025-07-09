using System;
using System.Collections;
using UnityEngine;
using static AM;

public class CharacterAnimationController : AnimatorSystem
{
    public float crossFade = 0.4f;
    public CombatEntityController CEC;
    public Weapon wpn;

    public Action<AtkAnims.Anims, float> AtkAnimation;
    public Action<BlkAnims.Anims, float> BlckAnimation;

    public const int UPPERBODY = 0;
    public bool cantAnimateUpperBody => CantAnimateUpperBody();
    public const int LOWERBODY = 1;

    [SerializeField]
    public AM.CyclePackage idleCycle = new CyclePackage(
        (int)MoveAnims.Anims.IDLE1,
        2f,
        MoveAnims.idles.Length
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
            typeof(MoveAnims),
            typeof(AtkAnims),
            typeof(BlkAnims)
        };
    }


    public void OnEnable()
    {
        CEC.Flinch += Flinch;
        CEC.Init += Init;
        CEC.moveDirInput += SetMoveDirInput;
        CEC.Death += DeathAnimation;

    }

    public void OnDisable()
    {
        CEC.Flinch -= Flinch;
        CEC.Init -= Init;
        CEC.moveDirInput -= SetMoveDirInput;
        CEC.Death -= DeathAnimation;
    }

    void Init()
    {
        string defaultAnim = MoveAnims.Anims.IDLE1.ToString();
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
        Play(typeof(MoveAnims), (int)MoveAnims.Anims.DEATH1, UPPERBODY, true, true);
        Play(typeof(MoveAnims), (int)MoveAnims.Anims.DEATH1, LOWERBODY, true, true);
    }


    void CheckInputsLAYER_UPPERBODY(Vector2 moveInput)
    {
        //print("cant use ability?" + CEC.cantUseAbility);
        if (!cantAnimateUpperBody)
            MoveAnimationsInvoker(UPPERBODY, moveInput);
    }

    bool CantAnimateUpperBody()
    {
        bool ret = false;

        if (!CEC.cantUseAbility) ret = true;
        if (CEC.Mode("Attack").isUsing) ret = true;


        return ret;
    }

    void CheckInputs_LAYER_LOWERBODDY(Vector2 moveInput)
    {
        MoveAnimationsInvoker(LOWERBODY, moveInput);
    }

    void MoveAnimationsInvoker(int layer, Vector2 moveInput)
    {
        if (moveInput.y >= 1)
            Play(typeof(MoveAnims), (int)MoveAnims.Anims.FORWARD, layer, false, false);
        else if (moveInput.y <= -1)
            Play(typeof(MoveAnims), (int)MoveAnims.Anims.BACK, layer, false, false);
        else if (moveInput.x >= 1)
            Play(typeof(MoveAnims), (int)MoveAnims.Anims.RIGHT, layer, false, false);
        else if (moveInput.x <= -1)
            Play(typeof(MoveAnims), (int)MoveAnims.Anims.LEFT, layer, false, false);
        else
        {
            if (!idleCycle.cycling)
                StartCoroutine(idleCycle.Cycle());

            Play(typeof(MoveAnims), (int)MoveAnims.idles[idleCycle.curr], layer, false, false);
        }
    }


}
