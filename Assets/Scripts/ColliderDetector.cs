using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColliderDetector : MonoBehaviour
{
/// <summary>
/// Colliider detector class controls : Itself, Attack collide triggers
/// Turns on and off the attack triggers
/// </summary>
    public CombatLock combatLock;
    public LayerMask collideWith;

    GameObject[] myAttackTriggers = new GameObject[4];
    AttackTriggerCollider attackTrigger_right;
    AttackTriggerCollider attackTrigger_left;
    AttackTriggerCollider attackTrigger_up;
    AttackTriggerCollider attackTrigger_down;
        
    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        col.includeLayers = collideWith;
        col.excludeLayers = ~collideWith;
    }
    private void OnEnable()
    {
        //myCombatEntity.control
    }

    /// <summary>
    /// Initializes the Collider Detector
    /// </summary>
    public void Init()
    {
        Debug.Log("Initizalizing Collider Detector");
        CacheAttackTriggers();
        DisableAttackTriggers();
    }

    private void OnTriggerEnter(Collider other)
    {
        combatLock.combatEntityInLockedZone = true;
        combatLock.lockedTarget = other.GetComponent<CombatEntityController>();
        if (other.GetComponent<CombatEntityController>() == null)
            Debug.LogError("Error: Combat entity controller script not given to a combat entity : " + other.name);
    }

    private void OnTriggerStay(Collider other)
    {
        if (combatLock.isLockedOnto)
        {
            combatLock.ColliderLockOntoTarget();
        }
    }


    private void OnTriggerExit(Collider other)
    {
        combatLock.combatEntityInLockedZone = false;
        combatLock.lockedTarget = null;
        if (combatLock.isLockedOnto)
        {
            combatLock.DeLock();
        }


        transform.localEulerAngles = Vector3.zero;
    }

    void CacheAttackTriggers()
    {
        myAttackTriggers = new GameObject[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
        {
            myAttackTriggers[i] = transform.GetChild(i).gameObject;
        }
    }

    public void EnableAttackTriggers()
    {
        foreach (GameObject attkTrigger in myAttackTriggers)
            attkTrigger.SetActive(true);
    }

    public void DisableAttackTriggers()
    {
        //Debug.Log("Disabling Attack Triggers");
        foreach (GameObject attkTrigger in myAttackTriggers)
            attkTrigger.SetActive(false);
    }

    public void InstantiateAttackTriggers(Ability right, Ability left, Ability up, Ability down)
    {
        attackTrigger_right = Instantiate(right.attackTriggerCollider, transform, false).GetComponent<AttackTriggerCollider>();
        attackTrigger_left = Instantiate(left.attackTriggerCollider, transform, false).GetComponent<AttackTriggerCollider>();
        attackTrigger_up = Instantiate(up.attackTriggerCollider, transform, false).GetComponent<AttackTriggerCollider>();
        attackTrigger_down = Instantiate(down.attackTriggerCollider, transform, false).GetComponent<AttackTriggerCollider>();
        CacheAttackTriggers();
        foreach (GameObject attkTrigger in myAttackTriggers)
            attkTrigger.GetComponent<AttackTriggerCollider>().combatLock = combatLock;
    }

    public void EnableCurrentAttackTrigger(string dir)
    {

    }
}
