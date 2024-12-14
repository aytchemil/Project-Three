using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct PlayerStates
{
    public enum CurrentState
    {
        notSprinting = 0,
        sprinting = 1,
        combat = 2,
    }

    [Space]
    public float walkSpeed;
    public float sprintSpeed;
    public float combatSpeed;

    //Constructor
    public PlayerStates(float walkSpeed, float sprintSpeed, float combatSpeed)
    {
        this.walkSpeed = walkSpeed;
        this.sprintSpeed = sprintSpeed;
        this.combatSpeed = combatSpeed;
    }

    public float UpdateSpeed(CurrentState state)
    {
        float playerSpeed = walkSpeed;

        switch (state)
        {
            case CurrentState.notSprinting:
                playerSpeed = walkSpeed;
                break;
            case CurrentState.sprinting:
                playerSpeed = sprintSpeed;
                break;
            case CurrentState.combat:
                playerSpeed = combatSpeed;
                break;
        }


        return playerSpeed;
    }
}

public class PlayerMovement : MonoBehaviour
{

    [Header("Adjustable Variables")]
    //Adjustable Variables
    public PlayerStates playerStates;
    public PlayerStates.CurrentState state;
    [Space]

    public float groundLinearDampeningDrag;
    public float height;
    public LayerMask whatIsGroundMask;

    [Header("Flags")]
    //Flags
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool inCombat;

    [Header("Adjustable Component Refernces")]
    //Adjustable Component Refernces
    [SerializeField] private Transform lookPivot;

    //Private Variables
    float moveSpeed;

    //Cached Component Refernces
    Transform orientation;
    PlayerInputActions controls;
    Rigidbody rb;

    //Input Actions
    private InputAction move;
    private InputAction sprint;
    private InputAction combat;

 

    private void Awake()
    {
        //Cache
        rb = GetComponent<Rigidbody>();
        controls = new PlayerInputActions();
        orientation = transform;

        //Input Cache
        move = controls.Player.Move;
        sprint = controls.Player.Sprint;
        combat = controls.Player.Combat;

        //InputAction Callbacks
        sprint.started += ctx => OnSprintPressed();
        sprint.canceled += ctx => OnSprintReleased();
        combat.started += ctx => OnCombatToggle();

    }

    #region enable/disable movement

    private void OnEnable()
    {
        move.Enable();
        sprint.Enable();
        combat.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        sprint.Disable();
        combat.Disable();
    }
    #endregion 


    // Start is called before the first frame update
    void Start()
    {
        //Stops the player from tipping over
        rb.freezeRotation = true;
        state = PlayerStates.CurrentState.notSprinting;
    }



    private void FixedUpdate()
    {
        MovePlayer();
        GroundedCheck();
        SpeedHandler();

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
    void SpeedHandler()
    {
        rb.maxLinearVelocity = moveSpeed;
        moveSpeed = playerStates.UpdateSpeed(state);
    }

    void OnSprintPressed() 
    {
        if(isGrounded && state != PlayerStates.CurrentState.combat) 
            state = PlayerStates.CurrentState.sprinting;

        Debug.Log("Sprint Pressed");
    }
    void OnSprintReleased()
    { 
        if (state != PlayerStates.CurrentState.combat) 
            state = PlayerStates.CurrentState.notSprinting;

        Debug.Log("Sprint Released");

    }
    void OnCombatToggle() 
    {
        if (inCombat)
        {
            inCombat = false;
            state = PlayerStates.CurrentState.notSprinting;
        }
        else
        {
            state = PlayerStates.CurrentState.combat;
            inCombat = true;
        }



        Debug.Log("Combat Pressed");
    }



}
