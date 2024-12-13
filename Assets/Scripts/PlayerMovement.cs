using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    //Variables
    public float speedMoving;

    public PlayerControlls controls;
    
    //References
    public Rigidbody rb;
    [SerializeField] private Transform lookPivot;

    //Input Action References
    private InputAction move;


    private void Awake()
    {
        controls = new PlayerControlls();
        move = controls.Player.Move;
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
        
    }



    private void FixedUpdate()
    {
        Vector3 forward = lookPivot.forward; 
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = lookPivot.right;
        right.y = 0f;
        right.Normalize();

        Vector2 inputDirection = move.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(inputDirection.x, 0, inputDirection.y).normalized;



        rb.linearVelocity = moveDirection * speedMoving + new Vector3(0, rb.linearVelocity.y, 0);
    }



    //Custom Functions

    private void CachePivotDirections()
    {
        
    }

}
