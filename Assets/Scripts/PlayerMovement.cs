using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float moveSpeed;
    public float sprintSpeed;

    public PlayerControlls controls;


    private InputAction move;

    Vector3 moveDirection;

    private void Awake()
    {
        controls = new PlayerControlls();
    }

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, 0, moveDirection.y * moveSpeed);
    }
}
