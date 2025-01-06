using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        MovePlayer();
    }


    //Moves the Player Forward
    void MovePlayer()
    {
        //Stores input values from the InputSystem Controls
        Vector2 moveInput = Controls.move != null ? Controls.move.Invoke() : Vector2.zero;


        //Debug.Log(moveInput);
        //Results in Move Input: (0.0  ,   0.0)
        //                       (+-1.0,   0.0)
        //                       (0.0  , +-1.0)
        //                       (+-1.0, +-1.0)

        //Stores the move direction of the player, which is always set to where the orientaion's forward and right is 
        //the player facing forward * the move input of y (which is either neg or pos)
        //the combined vector of all of those
        Vector3 moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;

        SpeedHandler();


        if (moveInput.x == 0 && moveInput.y == 0) return;

        //Adds a pushing force to the RigidBody based on movement speed
        if (isGrounded)
        {
            if (!onSlope)
            {
                //print("moving");
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

                if(Controls.getMoveDirection?.Invoke() == "left" || Controls.getMoveDirection?.Invoke() == "right")
                {
                    rb.AddForce(orientation.forward * 15f, ForceMode.Force);
                }
            }
            else
                rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 10f, ForceMode.Force);

        }
        else
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airAccelerationMultiplier, ForceMode.Force);

        //Debug.Log(rb.linearVelocity);
    }


    public override void EnableMovement()
    {
        playerControls.ia_move.Enable();
       // print("enabling movement");
    }

    public override void DisableMovement()
    {
        playerControls.ia_move.Disable();
      //  print("disabling movement from playercontrols");
    }

}
