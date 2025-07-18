using NUnit.Framework;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;




[Serializable]
public abstract class AbilityEffect
{
    public virtual void Execute(GameObject attacker) { }
}

public interface IAEffectDirectional
{
    public virtual void Execute(GameObject attacker) { }
}

public interface IAEffectOnHit
{
    public abstract void OnHit(GameObject attacker, GameObject target);
}

public interface IAEffectRuntime<T>
{
    public T data { get; set; }

    public abstract void Execute(GameObject attacker, T type);
}




[Serializable]
public class DirectionalOnHitEffects : AbilityEffect, IAEffectOnHit
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

    public void OnHit(GameObject attacker, GameObject target)
    {
        if (CanExecute(attacker, target) == false) return;

        foreach(IAEffectOnHit effect in directionals)
            effect.OnHit(attacker, target);
    }

    [SerializeReference] public List<IAEffectDirectional> directionals;



}

[Serializable]
class MoveAttackerEffect : AbilityEffect
{
    public string direction;

    public float amount;

    public override void Execute(GameObject attacker)
    {
        attacker.GetComponent<Movement>().Lunge(direction, amount);
        attacker.GetComponent<Movement>().DisableMovement();
        WaitExtension.Wait(attacker.GetComponent<EntityController>(), attacker.GetComponent<Movement>().entityStates.dashTime, () =>
        {
            attacker.GetComponent<Movement>().EnableMovement();
        });
    }
}

[Serializable]
class RuntimeMoveAttackerEffect : AbilityEffect, IAEffectRuntime<string>
{
    public string direction;

    public float amount;

    public string data { get => direction; set => direction = value; }

    public void Execute(GameObject attacker, string direction)
    {
        data = direction;

        attacker.GetComponent<Movement>().Lunge(direction, amount);
        attacker.GetComponent<Movement>().DisableMovement();
        WaitExtension.Wait(attacker.GetComponent<EntityController>(), attacker.GetComponent<Movement>().entityStates.dashTime, () =>
        {
            attacker.GetComponent<Movement>().EnableMovement();
        });
    }
}


[Serializable]
class DamageEffect : IAEffectDirectional, IAEffectOnHit
{
    public float amount;

    public void OnHit(GameObject attacker, GameObject target)
    {
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
class FlinchEffect : IAEffectDirectional, IAEffectOnHit
{
    public float duration;

    public void OnHit(GameObject attacker, GameObject target)
    {
        target.GetComponent<AttackbleEntity>().Flinch(duration);
        Debug.Log($"{attacker.name} made {target.name} flinch for {duration} seconds");
    }
}

[Serializable]
class HitVFX : IAEffectDirectional, IAEffectOnHit
{
    public GameObject hitVFXPrefab;
    public float duration;
    public void OnHit(GameObject attacker, GameObject target)
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