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
    [SerializeField] protected bool isGrounded;
    [SerializeField] protected bool onSlope;

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


    protected virtual void FixedUpdate()
    {
        GroundedCheck();
        OnSlope();
    }



    //Checks if the player is grounded to apply linear damping on the rigid body
    protected void GroundedCheck()
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
    protected void SpeedHandler()
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
    protected void OnSlope()
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
    protected Vector3 GetSlopeMoveDirection(Vector3 moveDirection)
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    /// <summary>
    /// Button press methods
    /// </summary>
    #region Button Presses
    protected void OnSprintPressed()
    {
        //Debug.Log("sprint pressed");
        if (isGrounded && state != EntityStates.CurrentState.combat)
            state = EntityStates.CurrentState.sprinting;
    }
    protected void OnSprintReleased()
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
    protected void EnterCombat()
    {
        //Debug.Log("Player movement entering combat");
        state = EntityStates.CurrentState.combat;
    }

    /// <summary>
    /// Observer method for when the player exits combat
    /// </summary>
    /// <param name="target"></param>
    protected private void ExitCombat()
    {
        Debug.Log("Player movement exiting combat");
        state = EntityStates.CurrentState.notSprinting;
    }


    /// <summary>
    /// Attempts a dash in a direction relative to the given player inputs
    /// </summary>
    protected void DashDirection()
    {
        //Debug.Log("Attempted a dash");
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
    protected void Dash(Vector2 dir)
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
        Controls.dashing = true;
        Invoke("StopDash", entityStates.dashTime);
        Invoke("DashCooldown", dashCooldown);
    }

    /// <summary>
    /// Method that is invoked with a delay to for dash cooldown
    /// </summary>
    protected void StopDash()
    {
        state = EntityStates.CurrentState.combat;
        Controls.dashing = false;
    }

    /// <summary>
    /// Method that is invoked with a delay to for dash cooldown
    /// </summary>
    protected void DashCooldown()
    {
        Controls.dashOnCooldown = false;
    }

}
