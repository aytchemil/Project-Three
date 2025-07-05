using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CharacterAnimationController : AnimatorSystem
{
    public Action<string, float> UseAnimation;
    public float crossFade = 0.4f;
    public CombatEntityController CEC;
    Vector2 moveDirInput;

    private void Awake()
    {
        CEC = GetComponentInParent<CombatEntityController>();
        CEC.animController = this;
        if (GetComponent<Animator>() == null)
            gameObject.AddComponent<Animator>();
        animator = GetComponent<Animator>();
    }

    private void Init()
    {
        animator.runtimeAnimatorController = wpn.animationController;
        InitializeAnimatorSystem(animator.layerCount, wpn.GetAnimation("MoveIdle"), animator, PlayDefaultAnimation);
    }

    public void OnEnable()
    {
        UseAnimation += PlayAnimation;
        CEC.Flinch += Flinch;
        CEC.Init += Init;
        CEC.MoveDirection += MoveDir;
        CEC.moveDirInput += SetMoveDirInput;

    }

    public void OnDisable()
    {
        UseAnimation -= PlayAnimation;
        CEC.Flinch -= Flinch;
        CEC.Init -= Init;
        CEC.moveDirInput -= SetMoveDirInput;
    }

    void SetMoveDirInput(Vector2 moveInput)
    {
        print($"[{gameObject.name}] Setting move direction input: {moveInput}");
        moveDirInput = moveInput;

        UpperBodyAnimationsInvoker(moveInput);
        LowerBodyAnimationsInvoker(moveInput);
    }

    private void PlayAnimation(string animName, float delay)
    {
        animator.Rebind();
        print($"[Animation Controller] playing animation name [{animName}]");
        StartCoroutine(Animate(animName, delay));
    }

    IEnumerator Animate(string animName, float delay)
    {
        yield return new WaitForSeconds(delay);


        float speed = animator.speed = wpn.GetAnimation(animName).speed;

        Debug.Log($"[AnimtationController] playing animation: [{animName}], speed: [{speed}]");
        animator.speed = speed;
        animator.CrossFade(animName, crossFade);
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

    protected override void PlayDefaultAnimation(int layer)
    {
        base.PlayDefaultAnimation(layer);

        if (layer == UPPERBODY)
            UpperBodyAnimationsInvoker(moveDirInput);
        else 
            LowerBodyAnimationsInvoker(moveDirInput);
    }

    protected void MoveDir(CombatEntityController.MoveDirections moveDirs, int layer)
    {
        print($"[{gameObject.name}] AnimationSystem Entity Moving: ({moveDirs.ToString()}) layer: ({layer})");

        if (moveDirs == CombatEntityController.MoveDirections.FORWARD)
            Play("MoveForward", layer, false, false);
        else if (moveDirs == CombatEntityController.MoveDirections.BACK)
            Play("MoveBack", layer, false, false);
        else if(moveDirs == CombatEntityController.MoveDirections.RIGHT)
            Play("MoveRight", layer, false, false);
        else if(moveDirs == CombatEntityController.MoveDirections.LEFT)
            Play("MoveLeft", layer, false, false);
        else
            Play("MoveIdle", layer, false, false);

    }

    void UpperBodyAnimationsInvoker(Vector2 moveInput)
    {
        MoveAnimationsInvoker(AnimatorSystem.UPPERBODY, moveInput);
    }

    void LowerBodyAnimationsInvoker(Vector2 moveInput)
    {
        MoveAnimationsInvoker(AnimatorSystem.LOWERBODY, moveInput);
    }

    void MoveAnimationsInvoker(int layer, Vector2 moveInput)
    {
        if (moveInput.y >= 1)
            CEC.MoveDirection?.Invoke(CombatEntityController.MoveDirections.FORWARD, layer);
        else if (moveInput.y <= -1)
            CEC.MoveDirection?.Invoke(CombatEntityController.MoveDirections.BACK, layer);
        else if (moveInput.x >= 1)
            CEC.MoveDirection?.Invoke(CombatEntityController.MoveDirections.RIGHT, layer);
        else if (moveInput.x <= -1)
            CEC.MoveDirection?.Invoke(CombatEntityController.MoveDirections.LEFT, layer);
        else
            CEC.MoveDirection?.Invoke(CombatEntityController.MoveDirections.NONE, layer);
    }


}
