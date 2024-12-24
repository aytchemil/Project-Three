using UnityEngine;

public class EntityLook : MonoBehaviour
{
    CombatEntityController controls;

    //Adjustable Component References
    public Transform camOrientation;
    public Transform cameraPosition;
    public Camera myCamera;

    private void Awake()
    {
        controls = GetComponent<CombatEntityController>();
    }

    private void OnEnable()
    {
        controls.CombatFollowTarget += CameraLookAtLockTarget;
        controls.CombatFollowTarget += TransformLookAtTarget;
    }

    private void OnDisable()
    {
        controls.CombatFollowTarget -= CameraLookAtLockTarget;
        controls.CombatFollowTarget -= TransformLookAtTarget;
    }




    /// <summary>
    /// Makes the camera look at a given target restricted with X
    /// </summary>
    /// <param name="target"></param>
    public void CameraLookAtLockTarget(CombatEntityController target)
    {
        //Debug.Log("entity look : cam look at target");
        camOrientation.LookAt(target.transform.position);
        camOrientation.localEulerAngles = new Vector3(camOrientation.localEulerAngles.x, 0, 0);

    }

    /// <summary>
    /// Transform looks at a target restricted to Y
    /// </summary>
    /// <param name="target"></param>
    public void TransformLookAtTarget(CombatEntityController target)
    {
        //Debug.Log("entity look : transform look at target");

        transform.LookAt(target.transform.position);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }


}

