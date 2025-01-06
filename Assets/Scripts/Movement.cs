using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


[System.Serializable]
public struct EntityStates
{
    public enum CurrentState
    {
        notSprinting = 0,
        sprinting = 1,
        combat = 2,
        dashing = 3,
        missedAttack = 4,
        flinching = 5,
    }

    [Space]
    public float walkSpeed;
    public float sprintSpeed;
    public float combatSpeed;
    public float dashSpeed;
    public float dashTime;
    public float missedAttackSpeed;
    public float flinchSpeed;

    //Constructor
    public EntityStates(float walkSpeed, float sprintSpeed, float combatSpeed, float dashSpeed, float dashTime, float missedAttackSpeed, float flinchSpeed)
    {
        this.walkSpeed = walkSpeed;
        this.sprintSpeed = sprintSpeed;
        this.combatSpeed = combatSpeed;
        this.dashSpeed = dashSpeed;
        this.dashTime = dashTime;
        this.missedAttackSpeed = missedAttackSpeed;
        this.flinchSpeed = flinchSpeed;
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
            case CurrentState.missedAttack:
                playerSpeed = missedAttackSpeed;
                break;
            case CurrentState.flinching:
                playerSpeed = flinchSpeed;
                break;
        }


        return playerSpeed;
    }
}
public class Movement : MonoBehaviour
{
    [Header("Movement")]
    [Header("Movement : Adjustable Variables")]
    public EntityStates entityStates;
    [field: SerializeField] public EntityStates.CurrentState state { get; private set; }
    public float moveSensitity = 5f;
    [Space]
    [Header("Movement: Drag")]
    public float groundLinearDampeningDrag;
    public float weightMultiplier;
    public float airAccelerationMultiplier;
    [Header("Movement: Slope Movement")]
    [SerializeField] protected bool isGrounded;
    [SerializeField] protected bool onSlope;
    public float height;
    public LayerMask whatIsGroundMask;
    public float maxSlopeAngle;
    [Header("Movement: Dodging")]
    public float dashSpeedMultiplier;
    public float afterDashPeriodTimeLength = 0.5f;
    public float dashCooldown;

    //Private Variables
    protected float moveSpeed;
    protected RaycastHit slopeHit;

    //Cached Component Refernces
    protected Transform orientation;
    public Rigidbody rb { get; protected set; }
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

        //Enter/Exit Combat
        Controls.EnterCombat += EnterCombat;
        Controls.ExitCombat += ExitCombat;


        //Missed Attack
        Controls.MissedAttack += MissedAttack;
        Controls.ResetAttack += ResetAttack;

        //Flinch
        Controls.Flinch += Flinch;

        Controls.getMoveDirection = () => GetMoveDirection();
    }

    private void OnDisable()
    {
        //Func Callback Additions
        Controls.sprintStart -= OnSprintPressed;
        Controls.sprintStop -= OnSprintReleased;
        Controls.dash -= DashDirection;

        //Action Callback additions

        //Enter/Exit Combat
        Controls.EnterCombat -= EnterCombat;
        Controls.ExitCombat -= ExitCombat;


        //Missed Attack
        Controls.MissedAttack -= MissedAttack;
        Controls.ResetAttack -= ResetAttack;

        //Flinch
        Controls.Flinch -= Flinch;
    }


    protected virtual void FixedUpdate()
    {
        GroundedCheck();
        OnSlope();
    }

    //Results in Move Input: (0.0  ,   0.0)
    //                       (+-1.0,   0.0)
    //                       (0.0  , +-1.0)
    //                       (+-1.0, +-1.0)

    //Results in Move Input: (1.0  ,   0.0) right
    //                       (-1.0 ,   0.0) left
    //                       (0.0  ,  1.0)  up
    //                       (0.0  , -1.0)  back

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
        //print("Handling speed");
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
        // print(state);
        // print(gameObject.name + " | movement's SpeedHandler() : new movespeed is : " + moveSpeed);
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
        // Debug.Log("Player movement exiting combat");
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

        //print(localVelocity);
        Dash(GetMoveDirection(), dashSpeedMultiplier);
    }

    /// <summary>
    /// Adds force to the player for a dash direction
    /// </summary>
    /// <param name="dir"></param>
    public void Dash(string dashDirection, float multiplier)
    {
        Vector3 dir = new Vector3();

        switch(dashDirection)
        {
            case "foward":
                dir = transform.TransformDirection(Vector3.forward);
                break;
            case "back":
                dir = transform.TransformDirection(Vector3.back);
                break;
            case "right":
                dir = transform.TransformDirection(Vector3.right);
                dir += transform.TransformDirection(Vector3.forward) * 2;
                break;
            case "left":
                dir = transform.TransformDirection(Vector3.left);
                dir += transform.TransformDirection(Vector3.forward) * 2;
                break;
            case "none":
                dir = transform.TransformDirection(Vector3.forward);
                break;
            default:
                dir = transform.TransformDirection(Vector3.forward);
                break;


        }

        //Debug.Log("Attempting Dashing In Direction:  " + dir);

        if (isGrounded)
        {
            //If the player is not on a slope, dash regularly
            if (!onSlope)
            {
                rb.AddForce(dir.normalized * multiplier, ForceMode.VelocityChange);

            }
            else //if the player is on a slope, dash relative to the slope's move direction
                rb.AddForce(GetSlopeMoveDirection(dir) * dashSpeedMultiplier, ForceMode.VelocityChange);

            state = EntityStates.CurrentState.dashing;
        }

        //Dash cooldown
        Controls.dashing = true;
        StartCoroutine(StopDash());
        Invoke("DashCooldown", dashCooldown + afterDashPeriodTimeLength);
    }

    public void Lunge(string dir, float multiplier)
    {
       // print("Lunging");
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;



        Dash(dir, multiplier);
    }

    public string GetMoveDirection()
    {
        Vector3 dir = rb.transform.InverseTransformDirection(rb.linearVelocity); // Velocity in local space

        string retDir = "";


        //left is -x
        //right is +x
        //fwd is +z
        //back is -z
        float absZ = Mathf.Abs(dir.z);
        float absX = Mathf.Abs(dir.x);


        if (dir.z > 1 && absZ > absX && absZ > moveSensitity)
            retDir = "foward";
        else if (dir.z < 1 && absZ > absX && absZ > moveSensitity)
            retDir = "back";

        else if (dir.x > 1 && absX > absZ && absX > moveSensitity)
            retDir = "right";

        else if (dir.x < 1 && absX > absZ && absX > moveSensitity)
            retDir = "left";
        else
            retDir = "none";


        print("Getting move dir: " + retDir);


        return retDir;
    }


    //Results in Move Input: (1.0  ,   0.0) right
    //                       (-1.0 ,   0.0) left
    //                       (0.0  ,  1.0)  up
    //                       (0.0  , -1.0)  back

    /// <summary>
    /// Method that is invoked with a delay to for dash cooldown
    /// </summary>

    protected IEnumerator StopDash()
    {
        yield return new WaitForSeconds(entityStates.dashTime);

        //print("Stop dash setting current state to combat");
        if (Controls.GetTarget?.Invoke() != null)
            state = EntityStates.CurrentState.combat;
        else
            print(gameObject.name + " | StopDash: Target null, keeping current state");

        yield return new WaitForSeconds(afterDashPeriodTimeLength);


        Controls.dashing = false;
    }

    /// <summary>
    /// Method that is invoked with a delay to for dash cooldown
    /// </summary>
    protected void DashCooldown()
    {
        Controls.dashOnCooldown = false;
    }

    protected void MissedAttack()
    {
        //Debug.Log("setting current state to missed attack");
        state = EntityStates.CurrentState.missedAttack;
    }

    protected void ResetAttack()
    {
        //Debug.Log(gameObject.name + " | Movement : resseting attack back to combat");
        state = EntityStates.CurrentState.combat;
    }

    public virtual void EnableMovement()
    {

    }

    public virtual void DisableMovement()
    {
        //print("disbaling from movement");
    }

    #region flinch

    public void Flinch(float flinchTime)
    {
        EntityStates.CurrentState prevState = state;
        if (prevState == EntityStates.CurrentState.dashing || prevState == EntityStates.CurrentState.missedAttack)
            prevState = EntityStates.CurrentState.combat;


        state = EntityStates.CurrentState.flinching;

        StartCoroutine(StopFlinch(flinchTime, prevState));
    }
    public IEnumerator StopFlinch(float flinchTime, EntityStates.CurrentState prevState)
    {
        yield return new WaitForSeconds(flinchTime);

        state = prevState;
    }
    #endregion

}
