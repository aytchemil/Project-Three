using UnityEngine;

public class PlayerCombatEntity : CombatEntity
{

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Respawn()
    {
        base.Respawn();
    }


    void AttemptLock()
    {
        if (isLockedOntoSomething) return;




    }



}
