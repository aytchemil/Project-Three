using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AM;

public class CharacterAnimationController : AnimatorSystem
{
    public float crossFade = 0.4f;
    public EntityController EC;

    public Action<AtkAnims.Anims, float> AtkAnimation;
    public Action<BlkAnims.Anims, float> BlckAnimation;

    public const int UPPERBODY = 0;
    public bool cantAnimateUpperBody => CantAnimateUpperBody();
    public const int LOWERBODY = 1;


    Vector2 moveDirInput;


    public void OnEnable()
    {
        WaitExtension.WaitAFrame(this, () =>
        {
            EC.Flinch += Flinch;
            EC.moveDirInput += SetMoveDirInput;
            EC.Death += DeathAnimation;
        });
    }

    public void OnDisable()
    {
        EC.Flinch -= Flinch;
        EC.moveDirInput -= SetMoveDirInput;
        EC.Death -= DeathAnimation;
    }

    public void Init(int layerCount, EntityController entity, RuntimeAnimatorController wpnAnims)
    {
        EC = entity;
        EC.animController = this;

        if (GetComponent<Animator>() == null)
            gameObject.AddComponent<Animator>();
        animator = GetComponent<Animator>();

        InitializeAnimationSystem(layerCount, animator, wpnAnims);

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
        //print($"[AnimatorController] [{EC.gameObject.name}] FlinchAnim");
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
        //print("cant use ability?" + EC.cantUseAbility);
        if (!cantAnimateUpperBody)
            MoveAnimationsInvoker(UPPERBODY, moveInput);
    }

    bool CantAnimateUpperBody()
    {
        bool ret = false;

        if (!EC.cantUseAbility) ret = true;
        if (EC.Mode("Attack").isUsing) ret = true;


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
            if (CyclePackages.TryGetValue("idle", out var idleCycle) && !idleCycle.cycling)
                StartCoroutine(idleCycle.Cycle());

            Play(typeof(MoveAnims), (int)MoveAnims.idles[idleCycle.curr], layer, false, false);
        }
    }


}
