using System;
using System.Collections;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    public Action<string, float> UseAnimation;

    public Weapon weapon;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnEnable()
    {
        UseAnimation += PlayAnimation;
    }

    public void OnDisable()
    {
        UseAnimation -= PlayAnimation;
    }


    private void PlayAnimation(string animName, float delay)
    {
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
}
