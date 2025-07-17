using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AttackbleEntity : MonoBehaviour
{
    public EntityController controls;

    [SerializeField] private float _health;
    public float health
    {
        get => _health;
        set
        {
            //Debug.Log("Setting Health");
            _health = value;
        }
    }

    private void Awake()
    {
        health = maxHealth;
        controls = GetComponent<EntityController>();
        if(animator == null)
            animator = GetComponent<Animator>();
    }

    public float maxHealth;

    public Animator animator;
    public bool invincibility;
    public float invincibiliyTime;
    public float deathTime;
    public float corpseDeathTime;


    public void Attacked(float amount)
    {
        invincibility = true;
        this.Wait(invincibiliyTime, () => StopAttacked());
        health -= amount;
        CheckDeath();



        void CheckDeath()
        {
            if (health > 0) return;

            animator.SetBool("Die", true);
            controls.isAlive = false;
            Invoke(nameof(Die), deathTime);
        }

        void Die()
        {
            Destroy(gameObject);
        }

        void StopAttacked()
        {
            invincibility = false;
        }


    }

    public void Flinch(float duration)
    {
        FlinchCaller(duration);

        void FlinchCaller(float flinchTime)
        {
            controls.Flinch?.Invoke(flinchTime);
            controls.isFlinching = true;
            this.Wait(flinchTime, () => StopFlinching());
        }

        void StopFlinching()
        {
            controls.isFlinching = false;
        }
    }


    public void Heal(float amount)
    {
        health += amount;
        if(health > maxHealth)
            health = maxHealth;
    }

    private void OnDestroy()
    {
        
    }


}
