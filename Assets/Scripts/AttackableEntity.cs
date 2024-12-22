using UnityEngine;

public class AttackbleEntity : MonoBehaviour
{
    [SerializeField] private float _health;
    public float health
    {
        get => _health;
        set
        {
            Debug.Log("Setting Health");
            _health = value;
        }
    }

    public GameObject attackedEffect;
    public bool invincibility;
    public float invincibiliyTime;
    
    public virtual void Attacked(Ability atkedWithAbility)
    {
        if (!invincibility)
        {
            invincibility = true;
            print("I was attacked");
            attackedEffect.GetComponent<ParticleSystem>().Play();
            Invoke("StopAttacked", invincibiliyTime);
            TakeDamage(atkedWithAbility.damage);
        }

    }

    void StopAttacked()
    {
        invincibility = false;
    }

    void TakeDamage(float dmg)
    {
        health -= dmg;

        if(health < 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }


    //Tick damage over a certain time?


}
