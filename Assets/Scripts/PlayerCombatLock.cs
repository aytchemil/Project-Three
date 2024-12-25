using System;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerController))]
public class PlayerCombatLock : CombatLock
{
    PlayerController playerController;

    public override CombatEntityController Controls 
    {
        get => playerController;
        set => playerController = value as PlayerController;
    }

    protected override void Awake()
    {
        //Dont use base, because we want the player controller not the CombatEntityController

        playerController = GetComponent<PlayerController>();
    }

    protected override void Start()
    {
        base.Start();
        //Debug.Log("Start : PLayer");

    }



}
