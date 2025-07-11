using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AttackbleEntity : MonoBehaviour
{
    public CombatEntityController controls;

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
        controls = GetComponent<CombatEntityController>();
        if(animator == null)
            animator = GetComponent<Animator>();
    }

    public float maxHealth;

    public GameObject attackedEffect;
    public Animator animator;
    public bool invincibility;
    public float invincibiliyTime;
    public float deathTime;
    public float corpseDeathTime;


    public virtual float Attacked(AbilityAttack atkedWithAbility, string dir)
    {
        if (WasBlocked(dir))
        {
            print($"[AttackableEntity] [{gameObject.name}] blocked an attack");
            return _health;
        }


        float newHealth;
        if (!invincibility)
        {
            invincibility = true;

            print(gameObject.name + " - I was attacked by : " + atkedWithAbility);
            attackedEffect.GetComponent<ParticleSystem>().Play();
            Invoke("StopAttacked", invincibiliyTime);
            FlinchCaller(atkedWithAbility.flinchAmount);

            //print(atkedWithAbility);
           // print(atkedWithAbility.damage);

            newHealth = TakeDamage(atkedWithAbility.damage);
        }
        else
        {
            //print("Attacked while invincible, or already taken damage");
            newHealth = health;
        }

        if (health < 0)
        {
            animator.SetBool("Die", true);
            controls.isAlive = false;
            Invoke("Die", deathTime);
            //For right now invoke death at a delay, later do a death animation, and have the animation event on finish call death
        }

        return newHealth;

    }

    void StopAttacked()
    {
        invincibility = false;
    }

    float TakeDamage(float dmg)
    {
        health -= dmg;

        return health;
    }

    [Button]
    void TestTakeDamage(float dmg)
    {
        TakeDamage(dmg);
        if (health <= 0)
        {
            animator.SetBool("Die", true);
            controls.isAlive = false;
            controls.Death?.Invoke();
            Invoke("Die", deathTime);
            //For right now invoke death at a delay, later do a death animation, and have the animation event on finish call death
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    #region Flinching

    void FlinchCaller(float flinchTime)
    {
        controls.Flinch?.Invoke(flinchTime);
        controls.isFlinching = true;
        Invoke(nameof(StopFlinching), flinchTime);
    }
    void StopFlinching()
    {
        //print(gameObject.name +  " | Stopped flinching");
        controls.isFlinching = false;
    }

    #endregion

    public void Heal(float amount)
    {
        health += amount;
        if(health > maxHealth)
            health = maxHealth;
    }

    bool WasBlocked(string dir)
    {
        bool ret = false;
        if (!controls.Mode("Block").isUsing) return ret;

        if (dir == "right" && controls.lookDir == "right")
            ret = true;
        else if (dir == "left" && controls.lookDir == "left")
            ret = true;
        else if (dir == "up" && controls.lookDir == "up")
            ret = true;
        else if (dir == "right" && controls.lookDir == "right")
            ret = true;

        return ret;


    }

}
