using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct EntityStates
{
    public enum CurrentState
    {
        notSprinting = 0,
        sprinting = 1,
        combat = 2,
        dashing = 3,
    }

    [Space]
    public float walkSpeed;
    public float sprintSpeed;
    public float combatSpeed;
    public float dashSpeed;
    public float dashTime;

    //Constructor
    public EntityStates(float walkSpeed, float sprintSpeed, float combatSpeed, float dashSpeed, float dashTime)
    {
        this.walkSpeed = walkSpeed;
        this.sprintSpeed = sprintSpeed;
        this.combatSpeed = combatSpeed;
        this.dashSpeed = dashSpeed;
        this.dashTime = dashTime;
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
            case CurrentState.dashing:
                playerSpeed = dashSpeed;
                break;
        }


        return playerSpeed;
    }
}
public class Movement : MonoBehaviour
{

    [Header("Adjustable Variables")]
    //Adjustable Variables
    public EntityStates entityStates;
    public EntityStates.CurrentState state;
    [Space]

    public float groundLinearDampeningDrag;
    public float height;
    public float weightMultiplier;
    public LayerMask whatIsGroundMask;
    public float airAccelerationMultiplier;
    public float maxSlopeAngle;
    [Space]
    public float dashSpeedMultiplier;
    public float dashCooldown;

    [Header("Flags")]
    //Flags
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool onSlope;

    [Header("Adjustable Component Refernces")]
    //Adjustable Component Refernces
    public Transform lookPivot;

    //Private Variables
    public float moveSpeed;
    public RaycastHit slopeHit;

    //Cached Component Refernces
    public Transform orientation;
    public Rigidbody rb;
    public virtual CombatEntityController Controls { get; set; }


    protected virtual void Awake()
    {
        //Cache
        Controls = gameObject.GetComponent<CombatEntityController>();
        rb = GetComponent<Rigidbody>();
        orientation = transform;

    }


    // Start is called before the first frame update
    void Start()
    {
        //Stops the player from tipping over
        rb.freezeRotation = true;
    }

    private void OnEnable()
    {
        state = EntityStates.CurrentState.notSprinting;

        //Func Callback Additions
        Controls.sprintStart += OnSprintPressed;
        Controls.sprintStop += OnSprintReleased;
        Controls.dash += DashDirection;

        //Action Callback additions
        Controls.EnterCombat += EnterCombat;
        Controls.ExitCombat += ExitCombat;
    }

    private void OnDisable()
    {
        //Func Callback Additions
        Controls.sprintStart -= OnSprintPressed;
        Controls.sprintStop -= OnSprintReleased;
        Controls.dash -= DashDirection;

        //Action Callback additions
        Controls.EnterCombat -= EnterCombat;
        Controls.ExitCombat -= ExitCombat;
    }


    private void FixedUpdate()
    {
        GroundedCheck();
        OnSlope();
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

        if (moveInput.x == 0 && moveInput.y == 0) return;

        //Stores the move direction of the player, which is always set to where the orientaion's forward and right is 
        //the player facing forward * the move input of y (which is either neg or pos)
        //the combined vector of all of those
        Vector3 moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;

        SpeedHandler();

        //Adds a pushing force to the RigidBody based on movement speed
        if (isGrounded)
        {
            if (!onSlope)
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            else
                rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 10f, ForceMode.Force);

        }
        else
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airAccelerationMultiplier, ForceMode.Force);

        //Debug.Log(rb.linearVelocity);
    }


    //Checks if the player is grounded to apply linear damping on the rigid body
    void GroundedCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, height * 0.5f, whatIsGroundMask);
        if (isGrounded && state != EntityStates.CurrentState.dashing)
            rb.linearDamping = groundLinearDampeningDrag;
        else if (state != EntityStates.CurrentState.dashing)
        {
            rb.linearDamping = 0;
            rb.AddForce(Vector3.down.normalized * weightMultiplier, ForceMode.Acceleration);
        }
        else
        {
            rb.linearDamping = 0;
        }
    }

    /// <summary>
    /// Caps the max linear velocity of the player
    /// </summary>
    void SpeedHandler()
    {

        //Limit X and Z velocity's but not Y (Because falling)
        if (rb.linearVelocity.x > moveSpeed || rb.linearVelocity.x < -moveSpeed)
        {
            float velx = Mathf.Clamp(rb.linearVelocity.x, -moveSpeed, moveSpeed);
            rb.linearVelocity = new Vector3(velx, rb.linearVelocity.y, rb.linearVelocity.z);
        }
        if (rb.linearVelocity.z > moveSpeed || rb.linearVelocity.z < -moveSpeed)
        {
            float velz = Mathf.Clamp(rb.linearVelocity.z, -moveSpeed, moveSpeed);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, velz);
        }



        moveSpeed = entityStates.UpdateSpeed(state);
    }

    /// <summary>
    /// Detects if the player is on a slope, sets a global flag
    /// </summary>
    void OnSlope()
    {
        RaycastHit slopeHit;
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, height * 0.5f + 0.4f))
        {
            this.slopeHit = slopeHit;
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            onSlope = (angle < maxSlopeAngle && angle != 0);
        }
    }

    /// <summary>
    /// Calculates the move direction when the player is on a plane
    /// </summary>
    /// <param name="moveDirection"></param>
    /// <returns></returns>
    Vector3 GetSlopeMoveDirection(Vector3 moveDirection)
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    /// <summary>
    /// Button press methods
    /// </summary>
    #region Button Presses
    void OnSprintPressed()
    {
        //Debug.Log("sprint pressed");
        if (isGrounded && state != EntityStates.CurrentState.combat)
            state = EntityStates.CurrentState.sprinting;
    }
    void OnSprintReleased()
    {
        //Debug.Log("sprint releasd");

        if (state != EntityStates.CurrentState.combat)
            state = EntityStates.CurrentState.notSprinting;
    }
    #endregion

    /// <summary>
    /// Observer method for when the player enters combat
    /// </summary>
    /// <param name="target"></param>
    void EnterCombat()
    {
        //Debug.Log("Player movement entering combat");
        state = EntityStates.CurrentState.combat;
    }

    /// <summary>
    /// Observer method for when the player exits combat
    /// </summary>
    /// <param name="target"></param>
    private void ExitCombat()
    {
        //Debug.Log("Player movement exiting combat");
        state = EntityStates.CurrentState.notSprinting;
    }


    /// <summary>
    /// Attempts a dash in a direction relative to the given player inputs
    /// </summary>
    void DashDirection()
    {
        Debug.Log("Attempted a dash");
        if (state != EntityStates.CurrentState.combat) return;
        if (Controls.dashOnCooldown)
            return;
        else
            Controls.dashOnCooldown = true;
        //Debug.Log("Dashing");

        Vector2 moveInput = Controls.move != null ? Controls.move.Invoke() : Vector2.zero;
        //Results in Move Input: (0.0  ,   0.0)
        //                       (+-1.0,   0.0)
        //                       (0.0  , +-1.0)
        //                       (+-1.0, +-1.0)

        Dash(moveInput);
    }

    /// <summary>
    /// Adds force to the player for a dash direction
    /// </summary>
    /// <param name="dir"></param>
    void Dash(Vector2 dir)
    {
        //Debug.Log("Attempting Dashing In Direction:  " + dir);


        //The vector which is left of the orientation
        Vector3 moveDirection = new Vector3();
        if (dir.x < 0)
            moveDirection = -orientation.right;
        else if (dir.x > 1)
            moveDirection = orientation.right;

        if (dir.y < 0)
            moveDirection = -orientation.forward;
        else if (dir.y > 1)
            moveDirection = orientation.forward;

        if (isGrounded)
        {
            //If the player is not on a slope, dash regularly
            if (!onSlope)
            {
                //Debug.Log("Dashing Left");
                rb.AddForce(moveDirection.normalized * dashSpeedMultiplier, ForceMode.VelocityChange);

            }
            else //if the player is on a slope, dash relative to the slope's move direction
                rb.AddForce(GetSlopeMoveDirection(moveDirection) * dashSpeedMultiplier, ForceMode.VelocityChange);

            state = EntityStates.CurrentState.dashing;
        }

        //Dash cooldown
        Invoke("StopDash", entityStates.dashTime);
        Invoke("DashCooldown", dashCooldown);
    }

    /// <summary>
    /// Method that is invoked with a delay to for dash cooldown
    /// </summary>
    void StopDash()
    {
        state = EntityStates.CurrentState.combat;
    }

    /// <summary>
    /// Method that is invoked with a delay to for dash cooldown
    /// </summary>
    void DashCooldown()
    {
        Controls.dashOnCooldown = false;
    }

}
