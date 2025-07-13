using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[Serializable]
public abstract class AbilityEffect
{
    public abstract void Execute(GameObject caster, GameObject target);
}

[Serializable]
public abstract class DirectionalEffect : AbilityEffect
{
    public static bool CanExecute(GameObject attacker, GameObject target)
    {
        if (TargetBlockedAttacker(attacker, target)) return false;
        if (TargetInvicincible(target)) return false;

        return true;
    }

    static bool TargetBlockedAttacker(GameObject attacker, GameObject target)
    {
        if (attacker.GetComponent<EntityController>().lookDir == target.GetComponent<EntityController>().lookDir)
            return true;
        return false;
    }

    static bool TargetInvicincible(GameObject target)
    {
        if (target.GetComponent<AttackbleEntity>().invincibility)
            return true;
        return false;
    }
}

[Serializable]
class DamageEffect : DirectionalEffect
{
    public float amount;

    public override void Execute(GameObject attacker, GameObject target)
    {
        if (CanExecute(attacker, target) == false) return;

        target.GetComponent<AttackbleEntity>().Attacked(amount);
        Debug.Log($"{attacker.name} dealt {amount} damage to {target.name}");

        if (CheckIfEnemyDead(target))
        {
            Debug.Log("Target is Dead");
            attacker.GetComponent<EntityController>().TargetDeath?.Invoke(target.GetComponent<EntityController>());
            attacker.GetComponent<CombatLock>().ExitCombatCaller();
        }
    }

    bool CheckIfEnemyDead(GameObject target)
    {
        if (target.GetComponent<AttackbleEntity>().health <= 0)
            return true;
        return false;
    }
}

[Serializable]
class FlinchEffect : DirectionalEffect
{
    public float duration;

    public override void Execute(GameObject attacker, GameObject target)
    {
        target.GetComponent<AttackbleEntity>().Flinch(duration);
        Debug.Log($"{attacker.name} made {target.name} flinch for {duration} seconds");
    }
}

[Serializable]
class HitVFX : AbilityEffect
{
    public GameObject hitVFXPrefab;
    public float duration;
    public override void Execute(GameObject attacker, GameObject target)
    {
        if (hitVFXPrefab == null) return;
        AttackHit(attacker, target);

        Debug.Log($"{attacker.name} created Hit Effect {hitVFXPrefab} on {target.name} for {duration} seconds");
    }

    void AttackHit(GameObject attacker, GameObject target)
    {
        Vector3 spawnPos = target.transform.position;
        GameObject newHitVFX = GameObject.Instantiate(hitVFXPrefab, spawnPos, Quaternion.identity);
        newHitVFX.GetComponent<ParticleSystem>().Play();
        WaitExtension.Wait(attacker.GetComponent<EntityController>(), duration, () => GameObject.Destroy(newHitVFX));
    }
}