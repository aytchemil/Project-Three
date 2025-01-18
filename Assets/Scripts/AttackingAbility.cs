using UnityEngine;

public class AttackingAbility : Ability
{
    public float[] initialAttackDelay = { 0.3f };
    public float missDelay;
    public float comnboDelayOnHit = 0.3f;
    protected float damage;
    public float flinchAmount = 1f;

    public float GetDamage()
    {
        return damage;
    }
}
