using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    public float xSens;
    public float ySens;

    public Transform orientation;

    [SerializeField] private float yRot;
    [SerializeField] private float xRot;

    public PlayerControlls controls;
    public GameObject MainCamera;

    private InputAction look;
    [SerializeField] private float lookX;
    [SerializeField] private float lookY;


    private void Awake()
    {
        controls = new PlayerControlls();
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
        Vector2 lookInput = look.ReadValue<Vector2>();
        

        lookX = lookInput.x * (xSens / 5);
        lookY = lookInput.y * (ySens / 5) * -1;
        MouseLooking(lookX, lookY);
    }

    void MouseLooking(float x, float y)
    {
        yRot += y;
        xRot += x;

        yRot = Mathf.Clamp(yRot, -90f, 90f);

        transform.rotation = Quaternion.Euler(0, xRot, 0);
        orientation.rotation = Quaternion.Euler(yRot, xRot, 0);
    }
}
