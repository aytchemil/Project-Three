using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerMovement : Movement
{
    PlayerController playerControls;

    public override CombatEntityController Controls 
    { 
        get => playerControls; 
        set => playerControls = value as PlayerController;
    }


    protected override void Awake()
    {
        //Cache
        playerControls = gameObject.GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        orientation = transform;

    }


}
