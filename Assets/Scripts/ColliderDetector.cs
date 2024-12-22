using NUnit.Framework;
using System.Collections.Generic;
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

    public GameObject closestCombatEntity;
    public GameObject previousClosestCombatEntity;
    public List<GameObject> collidedWithCombatEntities;
    public bool targetDescisionMade = false;
        
    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        col.includeLayers = collideWith;
        col.excludeLayers = ~collideWith;
    }

    private void OnTriggerEnter(Collider other)
    {
        collidedWithCombatEntities.Add(other.gameObject); //must Be first

        CollideWithNewCombatEntity(other);
    }

    private void OnTriggerStay(Collider other)
    {
        //CollideWithNewCombatEntity(other);
        combatLock.combatEntityInLockedZone = true;


        if (combatLock.isLockedOnto && !targetDescisionMade)
        {
            Debug.Log("Stay");
            combatLock.ColliderLockOntoTarget();

            previousClosestCombatEntity = closestCombatEntity;

            if(collidedWithCombatEntities.Count > 1)
                DetermineIfWantToSwitchToOtherTargetAvaliable();
            targetDescisionMade = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        collidedWithCombatEntities.Remove(other.gameObject);

        if(collidedWithCombatEntities.Count == 1)
            if (collidedWithCombatEntities[0] != combatLock.lockedTarget.gameObject)
                combatLock.lockedTarget = collidedWithCombatEntities[0].GetComponent<CombatEntityController>();

        bool switchingOffWhileTargetting = false;
        if (other.gameObject != previousClosestCombatEntity && other.gameObject != closestCombatEntity)
        {
            switchingOffWhileTargetting = true;
        }

        if (other.gameObject == previousClosestCombatEntity)
            previousClosestCombatEntity = null;

        if (other.gameObject == closestCombatEntity)
            closestCombatEntity = null;

        if (collidedWithCombatEntities.Count == 0 && !switchingOffWhileTargetting)
        {
            Delock();


            transform.localEulerAngles = Vector3.zero;


        }

        //bool lockOnNewBecauseOfClosestGoingOutOfRange = false;

        //if (other.gameObject == closestCombatEntity)
        //    if (collidedWithCombatEntities.Count-1 > 0)
        //        lockOnNewBecauseOfClosestGoingOutOfRange = true;



        //if (lockOnNewBecauseOfClosestGoingOutOfRange)
        //    OnTriggerStay(collidedWithCombatEntities[0].GetComponent<Collider>());

    }

    void Delock()
    {
        combatLock.combatEntityInLockedZone = false;
        combatLock.lockedTarget = null;
        if (combatLock.isLockedOnto)
        {
            Debug.Log("Delock from Collider Detector");
            combatLock.DeLock();
        }
    }

    void CollideWithNewCombatEntity(Collider other)
    {
        closestCombatEntity = DetermineWhichCombatEntityIsClosest();
        combatLock.lockedTarget = closestCombatEntity.GetComponent<CombatEntityController>();
        if (closestCombatEntity.GetComponent<CombatEntityController>() == null)
            Debug.LogError("Error: Combat entity controller script not given to a combat entity : " + other.name);
    }

    void RetargetTo(GameObject newTarget)
    {
        combatLock.combatEntityInLockedZone = true;
        closestCombatEntity = newTarget;
        combatLock.lockedTarget = newTarget.GetComponent<CombatEntityController>();
        combatLock.ColliderLockOntoTarget();

        previousClosestCombatEntity = newTarget;
        targetDescisionMade = true;


    }

    GameObject DetermineWhichCombatEntityIsClosest()
    {
        if(collidedWithCombatEntities.Count == 1)
            return collidedWithCombatEntities[0];

        GameObject closest = null;
        float closestDistance = Mathf.Infinity;
        Vector3 playerPos = transform.position;


        foreach(GameObject target in collidedWithCombatEntities)
        {
            //Debug.Log(target);
            Vector3 targetPos = target.transform.position;

            float dist = Vector3.Distance(playerPos, targetPos);
            if(dist < closestDistance)
            {
                closestDistance = dist;
                closest = target;
            }
        }

        return closest;
    }

    void DetermineIfWantToSwitchToOtherTargetAvaliable()
    {
        Debug.Log("determing if we want to switch targets");
        if (closestCombatEntity == previousClosestCombatEntity)
        {
            Debug.Log("Yes, switch");

            GameObject newTargetLock = RetargetLock();
            Debug.Log("retargetting to :" + collidedWithCombatEntities.IndexOf(newTargetLock));
            RetargetTo(newTargetLock);
        }

    }

    GameObject RetargetLock()
    {
        int indexOfCurrentClosestCombatEntity = collidedWithCombatEntities.IndexOf(closestCombatEntity);
        Debug.Log("Current closest's index:" + indexOfCurrentClosestCombatEntity);

        if (indexOfCurrentClosestCombatEntity >= collidedWithCombatEntities.Count - 1)
            return collidedWithCombatEntities[0];
        else
            return collidedWithCombatEntities[indexOfCurrentClosestCombatEntity + 1];

    }

    public void UnLockFromCombatLock()
    {
        targetDescisionMade = false;
    }

}
