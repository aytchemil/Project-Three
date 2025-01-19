using UnityEngine;

public class Look : MonoBehaviour
{
    [Header("Look")]
    public float lookAtSpeed = 26f;
    public float lockOnVerticalOffset = 1f;

    //Cached Component References
    protected CombatEntityController controls;

    [Header("Look : Adjustable Component Referehces")]
    public Transform cameraPosition;


    public virtual void Awake()
    {
        //Cache
        controls = GetComponent<CombatEntityController>();
    }

    public virtual void OnEnable()
    {
        //Callback Additions
        controls.CombatFollowTarget += InCombatFollowingTarget;
    }

    public virtual void OnDisable()
    {
        controls.CombatFollowTarget -= InCombatFollowingTarget;
    }

    public virtual void InCombatFollowingTarget(CombatEntityController target)
    {
        //print(gameObject.name + " following target : " + target.name);

        Vector3 targetLookAtPosition = new Vector3(target.transform.position.x, target.transform.position.y + lockOnVerticalOffset, target.transform.position.z);

        Vector3 direction = (targetLookAtPosition - transform.position).normalized;

        Quaternion rotationTowardsTarget = Quaternion.LookRotation(direction);


        HeadLookAtLockTarget(rotationTowardsTarget);
        BodyLookAtTarget(rotationTowardsTarget);
    }

    /// <summary>
    /// Makes the camera look at a given target restricted with X
    /// </summary>
    /// <param name="target"></param>
    void HeadLookAtLockTarget(Quaternion rotationTowardsTarget)
    {
        //print("Cam orientation changing");
        cameraPosition.rotation = Quaternion.Lerp(cameraPosition.rotation, rotationTowardsTarget, Time.deltaTime * lookAtSpeed);

        //cameraPosition.LookAt(target);
        cameraPosition.localEulerAngles = new Vector3(cameraPosition.localEulerAngles.x, 0, 0);

    }

    /// <summary>
    /// Transform looks at a target restricted to Y
    /// </summary>
    /// <param name="target"></param>
    void BodyLookAtTarget(Quaternion rotationTowardsTarget)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationTowardsTarget, Time.deltaTime * lookAtSpeed);

        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }


}
