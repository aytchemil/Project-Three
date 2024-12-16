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


    public float yRot;
    public float xRot;

    public Vector2 currentXY;
    public Vector2 newXY;
    public Vector2 newRotXY;

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
            MouseLooking(controls.look.ReadValue<Vector2>());

            GetCurrentXY();

            if (xRot > 180f) xRot -= 360f;
            if (yRot > 180f) yRot -= 360f;
            if (xRot < -180f) xRot += 360f;
            if (yRot < -180f) yRot += 360f;
        }
        else
        {
            if (target != null)
            {
                Debug.Log("Locked onto target by cam");
                CameraLookAtLockTarget(target.transform.position);
                TransformLookAtTarget(target.transform.position);
                UpdateNewXY();
            }
            else
                Debug.LogError("Player Camera: In combat with enemy that doesnt exist");
        }


        UpdateCamPosition();
    }

    //
    void MouseLooking(Vector2 lookInput)
    {
        //Debug.Log("Mouse Looking");
        //Stores the look input from the Input System

        //Cache's the X and Y positions from the Input System
        Vector2 xyRot = CalculateXYRot(lookInput);

       // Debug.Log("X : " + xRot + " Y: " + yRot);

        //Sets the rotation to the player inputted rotations
        transform.rotation = Quaternion.Euler(0, xyRot.x, 0);
        camOrientation.rotation = Quaternion.Euler(xyRot.y, xyRot.x, 0);
    }

    Vector2 CalculateXYRot(Vector2 rawXY)
    {
        //Cache's the X and Y positions from the Input System
        xRot += rawXY.x * (xSens / 5);
        yRot += rawXY.y * (ySens / 5) * -1;

        //Restricts the looking of UP and DOWN
        yRot = Mathf.Clamp(yRot, -90f, 90f);

        return new Vector2(xRot, yRot);
    }


    Vector2 MakeRaw(float transformEulerX, float camOrientationEulerY)
    {
        // Retrieve the local rotation angles
        float xRot = transformEulerX;
        float yRot = camOrientationEulerY;

        // Handle Unity's 360-degree wraparound for angles
        if (yRot > 180f) yRot -= 360f;

        // Undo clamping for yRot
        yRot = Mathf.Clamp(yRot, -90f, 90f);

        // Reverse the input transformation
        float rawX = xRot / (xSens / 5);
        float rawY = -yRot / (ySens / 5);

        return new Vector2(rawX, rawY);
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

        UpdateNewXY();
        ApplyNewViewPosition();

        inCombat = false;
        this.target = null;
    }


    Vector2 GetCurrentXY()
    {
        currentXY.y = transform.localEulerAngles.y;
        currentXY.x = camTransform.localEulerAngles.x;

        if (currentXY.y > 180f) currentXY.y -= 360f;
        if (currentXY.x > 180f) currentXY.x -= 360f;
        if (currentXY.y < -180f) currentXY.y += 360f;
        if (currentXY.x < -180f) currentXY.x += 360f;

        return new Vector2(currentXY.x, currentXY.y);
    }

    void ApplyNewViewPosition()
    {
        xRot = newXY.y;
        yRot = newXY.x;

        Debug.Log("NEW X : " + xRot + " NEW Y: " + yRot);
    }

    void UpdateNewXY()
    {
        newXY = GetCurrentXY();
    }


}
