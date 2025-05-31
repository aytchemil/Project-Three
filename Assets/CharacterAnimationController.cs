using System;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    public Action<string> UseAnimation;

    private Weapon weapon;
    private Animator animator;
    
    public void OnEnable()
    {
        UseAnimation += PlayAnimation;
    }

    public void OnDisable()
    {
        UseAnimation -= PlayAnimation;
    }


    private void PlayAnimation(string animName)
    {
        animator.Play(animName);
    }
}
