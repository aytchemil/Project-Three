using UnityEngine;

[RequireComponent(typeof(Collider))]


public class ColliderDetector : MonoBehaviour
{
/// <summary>
/// Colliider detector class controls : Itself, Attack collide triggers
/// Turns on and off the attack triggers
/// </summary>
    public CombatEntity myCombatEntity;
    public LayerMask collideWith;

    GameObject[] myAttackTriggers = new GameObject[4];
        
    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        col.includeLayers = collideWith;
        col.excludeLayers = ~collideWith;
    }
    private void OnEnable()
    {
        //Init();
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
        myCombatEntity.combatEntityInLockedZone = true;
        myCombatEntity.lockedTarget = other.GetComponent<CombatEntity>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (myCombatEntity.isLockedOnto)
        {
            myCombatEntity.ColliderLockOntoTarget();
        }
    }


    private void OnTriggerExit(Collider other)
    {
        myCombatEntity.combatEntityInLockedZone = false;
        myCombatEntity.lockedTarget = null;
        if (myCombatEntity.isLockedOnto)
        {
            myCombatEntity.DeLock();
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

    public void InstantiateAttackTriggers(Ability[] abilites)
    {
        for(int i = 0; i < abilites.Length; i++)
        {
            //Debug.Log("Collider detector given ability i: " + i + " ability :" + abilites[i].name);
            myAttackTriggers[i] = Instantiate(abilites[i].attackTriggerCollider, transform, false);
            myAttackTriggers[i].GetComponent<AttackTriggerCollider>().myCombatEntity = myCombatEntity;
        }
    }
}
