using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    //Adjustable Variables
    public float moveSpeed;
    public float groundLinearDampeningDrag;
    public float height;
    public LayerMask whatIsGroundMask;

    //Flags
    [SerializeField] private bool isGrounded;

    //Adjustable Component Refernces
    public Rigidbody rb;
    [SerializeField] private Transform lookPivot;

    //Cached Component Refernces
    Transform orientation;
    PlayerInputActions controls;

    //Input Actions
    private InputAction move;

 

    private void Awake()
    {
        //Cache
        controls = new PlayerInputActions();
        move = controls.Player.Move;
        orientation = transform;
    }

    #region enable/disable movement

    private void OnEnable()
    {
        move = controls.Player.Move;
        move.Enable();
        Debug.Log("Moving Enabled");
    }

    private void OnDisable()
    {
        move.Disable();
        Debug.Log("Moving Disabled");

    }
    #endregion 

    // Start is called before the first frame update
    void Start()
    {
        //Stops the player from tipping over
        rb.freezeRotation = true;
    }



    private void FixedUpdate()
    {
        MovePlayer();
        GroundedCheck();
        SpeedCap();
    }


    //Moves the Player Forward
    void MovePlayer()
    {
        //Stores input values from the InputSystem Controls
        Vector2 moveInput = move.ReadValue<Vector2>();
        //Results in Move Input: (0.0, 0.0)
        //                       (1.0, 0.0)
        //                       (0.0, 1.1)
        //                       (1.1, 1.1)

        //Stores the move direction of the player, which is always set to where the orientaion's forward and right is 
        Vector3 moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;

        //Adds a pushing force to the RigidBody based on movement speed
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    //Checks if the player is grounded to apply linear damping on the rigid body
    void GroundedCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, height * 0.5f + 0.2f, whatIsGroundMask);
        if (isGrounded)
            rb.linearDamping = groundLinearDampeningDrag;
        else
            rb.linearDamping = 0;
    }

    //Caps the max linear velocity of the player
    void SpeedCap()
    {
        rb.maxLinearVelocity = moveSpeed;
    }



}
