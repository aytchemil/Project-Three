using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    //Real-Time Adjustable Variables
    public float xSens;
    public float ySens;

    //Adjustable Component References
    public Transform orientation;
    public Transform cameraPosition;
    public Camera myCamera;

    //Cached Component References
    PlayerInputActions controls;

    private float yRot;
    private float xRot;


    private InputAction look;
    private float lookX;
    private float lookY;


    private void Awake()
    {
        //Cache controls
        controls = new PlayerInputActions();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    private void OnEnable()
    {
        look = controls.Player.Look;
        look.Enable();
    }

    private void OnDisable()
    {
        look.Disable();
    }


    private void Update()
    {
        //Looks with the mouse
        MouseLooking(lookX, lookY);

        //Update the Camera's position to the desired location set by this script
        Transform camTransform = myCamera.GetComponent<Transform>();  //CACHE
        camTransform.position = cameraPosition.position;
        camTransform.rotation = cameraPosition.rotation;
    }

    //
    void MouseLooking(float x, float y)
    {
        //Stores the look input from the Input System
        Vector2 lookInput = look.ReadValue<Vector2>();

        //Cache's the X and Y positions from the Input System
        xRot += lookInput.x * (xSens / 5);
        yRot += lookInput.y * (ySens / 5) * -1;

        //Restricts the looking of UP and DOWN
        yRot = Mathf.Clamp(yRot, -90f, 90f);

        //Sets the rotation to the player inputted rotations
        transform.rotation = Quaternion.Euler(0, xRot, 0);
        orientation.rotation = Quaternion.Euler(yRot, xRot, 0);
    }
}
