using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AttackbleEntity : MonoBehaviour
{
    CombatEntityController controls;

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
        controls = GetComponent<CombatEntityController>();
        if(animator == null)
            animator = GetComponent<Animator>();
    }

    public bool isAlive = true;
    public GameObject attackedEffect;
    public Animator animator;
    public bool invincibility;
    public float invincibiliyTime;
    public float deathTime;
    public float corpseDeathTime;

    public virtual float Attacked(Ability atkedWithAbility)
    {
        float newHealth;
        if (!invincibility)
        {
            invincibility = true;
           // print("I was attacked");
            attackedEffect.GetComponent<ParticleSystem>().Play();
            Invoke("StopAttacked", invincibiliyTime);
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
            isAlive = false;
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

    void Die()
    {
        Destroy(gameObject);
    }

    //Tick damage over a certain time?


}
