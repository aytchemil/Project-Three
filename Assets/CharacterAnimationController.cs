using System;
using System.Collections;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    public Action<string, float> UseAnimation;
    public CombatEntityController CEC;

    public Weapon weapon;
    public Animator animator;

    private void Awake()
    {
        CEC = GetComponentInParent<CombatEntityController>();
        CEC.animController = this;
        animator = GetComponent<Animator>();
    }

    public void OnEnable()
    {
        UseAnimation += PlayAnimation;
        CEC.Flinch += Flinch;
    }

    public void OnDisable()
    {
        UseAnimation -= PlayAnimation;
        CEC.Flinch -= Flinch;
    }


    private void PlayAnimation(string animName, float delay)
    {
        print($"[Animation Controller] playing animation name [{animName}]");
        StartCoroutine(Animate(animName, delay));
    }

    IEnumerator Animate(string animName, float delay)
    {
        yield return new WaitForSeconds(delay);


        float speed = animator.speed = weapon.GetAnimation(animName).speed;

        Debug.Log($"[AnimtationController] playing animation: [{animName}], speed: [{speed}]");
        animator.speed = speed;
        animator.Play(animName);
    }

    public void SetBool (string boolname, bool val)
    {
        animator.SetBool(boolname, val);
    }


    public void Flinch(float time)
    {
        print($"[AnimatorController] [{CEC.gameObject.name}] FlinchAnim");
        animator.Play("Idle");
    //Fix
    }
}
