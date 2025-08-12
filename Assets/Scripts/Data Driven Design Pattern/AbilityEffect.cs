using log4net.Util;
using System;
using System.Collections.Generic;
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
    public virtual void OnHit(GameObject attacker, string attackingDirection, GameObject target) { }
    public virtual void OnHit(GameObject attacker, GameObject target) { }

}

public interface IAEffectRuntime<T>
{
    public T Data { get; set; }

    public abstract void Execute(GameObject attacker, T type);
}

public interface IAEffectCreateVFX
{
    public GameObject PrefabVFX { get; set; }
    public float Duration { get; set; }
    public virtual void Execute(GameObject attacker, Vector3 position) { }

}




[Serializable]
public class DirectionalOnHitEffects : AbilityEffect, IAEffectOnHit
{
    public static bool CanExecute(string attackingDirection, GameObject target)
    {
        Debug.Log("Determing if directionals can execute");
        if (TargetBlockedAttacker(attackingDirection, target)) return false;
        Debug.Log("target didnt block attacker");
        if (TargetInvicincible(target)) return false;
        Debug.Log("target not invincable");

        return true;
    }

    static bool TargetBlockedAttacker(string attackingDirection, GameObject target)
    {
        string targDir = target.GetComponent<EntityController>().lookDir;
        
        Debug.Log($"Determining... attackingDir is {attackingDirection}, target look dir is {targDir}");

        if (BlockTriggerCollider.DidAttackGetBlocked(attackingDirection, targDir))
            return true;

        return false;
    }

    static bool TargetInvicincible(GameObject target)
    {
        if (target.GetComponent<AttackbleEntity>().invincibility)
            return true;
        return false;
    }

    public void OnHit(GameObject attacker, string attackingDirection, GameObject target)
    {
        Debug.Log("AFs : APLLYING Directionals");

        if (CanExecute(attackingDirection, target) == false) return;

        foreach(IAEffectOnHit effect in directionals)
            effect.OnHit(attacker, target);

        Debug.Log("AFs : Applied Directionals");
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

    public string Data { get => direction; set => direction = value; }

    public void Execute(GameObject attacker, string direction)
    {
        Data = direction;

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
class HitVFX : IAEffectDirectional, IAEffectOnHit, IAEffectCreateVFX
{
    [field:SerializeField] public GameObject PrefabVFX { get; set; }
    [field: SerializeField] public float Duration { get; set; }

    public void OnHit(GameObject attacker, GameObject target)
    {
        if (PrefabVFX == null) return;
        AttackHit(attacker, target);

        Debug.Log($"{attacker.name} created Hit Effect {PrefabVFX} on {target.name} for {Duration} seconds");
    }

    void AttackHit(GameObject attacker, GameObject target)
    {
        Vector3 spawnPos = target.transform.position;
        GameObject newHitVFX = GameObject.Instantiate(PrefabVFX, spawnPos, Quaternion.identity);
        newHitVFX.GetComponent<ParticleSystem>().Play();
        WaitExtension.Wait(attacker.GetComponent<EntityController>(), Duration, () => GameObject.Destroy(newHitVFX));
    }
}

[Serializable]
class CreateVFX : AbilityEffect, IAEffectCreateVFX
{
    [field: SerializeField] public GameObject PrefabVFX { get; set; }
    [field: SerializeField] public float Duration { get; set; }

    public void Execute(GameObject attacker, Vector3 position)
    {
        Debug.Log("BlockSys: Block effect being created");
        // --- spawn block effect ---
        GameObject fx = GameObject.Instantiate(PrefabVFX, position, Quaternion.identity);
        MonoBehaviour.Destroy(fx, Duration);
        // --------------------------
    }

}