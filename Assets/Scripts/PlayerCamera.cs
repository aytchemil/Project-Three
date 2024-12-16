using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ControllsHandler))]
public class PlayerCamera : MonoBehaviour
{
    //Real-Time Adjustable Variables
    public float xSens;
    public float ySens;

    //Adjustable Component References
    public Transform camOrientation;
    public Transform cameraPosition;
    public Camera myCamera;

    //Cached Component References
    ControllsHandler controls;
    Transform camTransform;
    CombatEntity target;

    //Privates
    public float savedTransformY;
    public float savedOrientationX;
    public Vector3 savedTargetLocation;

    //Flags
    bool inCombat = false;


    private float yRot;
    private float xRot;

    private float savedLookInputX;
    private float savedLookInputY;

    private void Awake()
    {
        controls = GetComponent<ControllsHandler>();
        camTransform = myCamera.GetComponent<Transform>();  //CACHE
        //Debug.Log(camTransform);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        inCombat = false;

        //Callback Additions
        controls.EnterCombat += EnterCombat;
        controls.ExitCombat += ExitCombat;
    }


    private void Update()
    {

        if (!inCombat)
        {

            //Looks with the mouse
            MouseLooking();

        }
        else
        {
            if (target != null)
            {
                Debug.Log("Locked onto target by cam");
                CameraLookAtLockTarget(target.transform.position);
                TransformLookAtTarget(target.transform.position);

            }
            else
                Debug.LogError("Player Camera: In combat with enemy that doesnt exist");
        }


        UpdateCamPosition();
    }

    //
    void MouseLooking()
    {
        Debug.Log("Mouse Looking");
        //Stores the look input from the Input System
        Vector2 lookInput = controls.look.ReadValue<Vector2>();

        //Cache's the X and Y positions from the Input System
        xRot += lookInput.x * (xSens / 5);
        yRot += lookInput.y * (ySens / 5) * -1;

        //Restricts the looking of UP and DOWN
        yRot = Mathf.Clamp(yRot, -90f, 90f);

        //Sets the rotation to the player inputted rotations
        transform.rotation = Quaternion.Euler(0, xRot, 0);
        camOrientation.rotation = Quaternion.Euler(yRot, xRot, 0);
    }

    void CameraLookAtLockTarget(Vector3 target)
    {
        camOrientation.LookAt(target);
        camOrientation.localEulerAngles = new Vector3(camOrientation.localEulerAngles.x, 0, 0);

    }

    void TransformLookAtTarget(Vector3 target)
    {
        transform.LookAt(target);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }



    void UpdateCamPosition()
    {
        //Update the Camera's position to the desired location set by this script
        camTransform.position = cameraPosition.position;
        camTransform.rotation = cameraPosition.rotation;
    }

    void EnterCombat(CombatEntity target)
    {
        inCombat = true;
        this.target = target;
    }

    void ExitCombat(CombatEntity target)
    {
        Debug.Log("Exiting Combat");
        inCombat = false;
        this.target = null;
    }
    


}
