using System;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerController))]
public class PlayerCombatLock : CombatLock
{
    PlayerController playerController;

    public override EntityController Controls 
    {
        get => playerController;
        set => playerController = value as PlayerController;
    }

    public override void InternalInit()
    {
        playerController = GetComponent<PlayerController>();
    }

    protected override void Start()
    {
        base.Start();
        //Debug.Log("Start : PLayer");

    }



}
