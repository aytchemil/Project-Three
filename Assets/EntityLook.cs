using UnityEngine;

public class EntityLook : MonoBehaviour
{
    //Adjustable Component References
    public Transform camOrientation;
    public Transform cameraPosition;
    public Camera myCamera;


    private void OnEnable()
    {
        ////Mouse lock states
        //UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        //UnityEngine.Cursor.visible = false;

        //inCombat = false;

        ////Callback Additions
        //controls.CombatFollowTarget += EnterCombatAndFollowTarget;
        //controls.ExitCombat += ExitCombat;
    }

    private void OnDisable()
    {
        //controls.CombatFollowTarget -= EnterCombatAndFollowTarget;
        //controls.ExitCombat -= ExitCombat;
    }




    /// <summary>
    /// Makes the camera look at a given target restricted with X
    /// </summary>
    /// <param name="target"></param>
    public void CameraLookAtLockTarget(Vector3 target)
    {
        Debug.Log("entity look : cam look at target");
        camOrientation.LookAt(target);
        camOrientation.localEulerAngles = new Vector3(camOrientation.localEulerAngles.x, 0, 0);

    }

    /// <summary>
    /// Transform looks at a target restricted to Y
    /// </summary>
    /// <param name="target"></param>
    public void TransformLookAtTarget(Vector3 target)
    {
        Debug.Log("entity look : transform look at target");

        transform.LookAt(target);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }


}

