using System.Collections;
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
    public float retargetTime = 1f;


    private void Awake()
    {
        //Cache
        Collider col = GetComponent<Collider>();

        //Set the layers for collision
        col.includeLayers = collideWith;
        col.excludeLayers = ~collideWith;
    }


    /// <summary>
    /// If we collide with a new combat entity and its alive, then add it to the collidedWithCombatEntities list
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CombatEntityController>().isAlive)
        {
            if (collidedWithCombatEntities.Count <= 0)
            {
                //print("Enter");
                collidedWithCombatEntities.Add(other.gameObject); //must Be first
                CollideWithNewCombatEntity(other);
            }
            else
            {
                collidedWithCombatEntities.Add(other.gameObject); //must Be first
            }
        }
    }

    /// <summary>
    /// Provides the outline for if a target or potential target is inside of the collision detection zone
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        //CollideWithNewCombatEntity(other);
        //Sets the flag in CombatLock for if there is an entity in this zone
        if(other.GetComponent<CombatEntityController>().isAlive)
            combatLock.combatEntityInLockedZone = true;

        if(!combatLock.Controls.isLockedOn && other.GetComponent<CombatEntityController>().isAlive && !combatLock.Controls.currentlyRetargetting)
        {
            closestCombatEntity = DetermineWhichCombatEntityIsClosest();
            //previousClosestCombatEntity = null;
        }

        //If the CombatLock says we need to lock onto something, and we havn't already locked onto anything (targetDesisiconMade) and the target we are checking for (other) is alive
        // - Then lock onto it
        if (combatLock.Controls.isLockedOn && other.GetComponent<CombatEntityController>().isAlive)
        {
            //Debug.Log("Stay");
            //Tells combatLock that the collisonDectector (this script) is to lock onto it
            //If its the current locked on dude

            if (!targetDescisionMade)
            {
                CancelInvoke("StopRetargetting");
                //Debug.Log("OnStay wants to lock onto: " + other.gameObject.name);
                ///RETARGET
                //If we have more than 1 potentiatl target we are colliding with, determine if we want to retarget
                if (previousClosestCombatEntity == closestCombatEntity)
                    DetermineIfWantToSwitchToOtherTargetAvaliable();


                //Saves this new target as the previous Closest Target
                previousClosestCombatEntity = closestCombatEntity;


                //Sets the flag that we have concluded targgeting
                targetDescisionMade = true;
                //Debug.Log("Descision made");

            }

            if (other.gameObject == closestCombatEntity)
            {
                //If not attacking

                //Testing for attacking then missing
                combatLock.Controls.CombatFollowTarget?.Invoke(other.gameObject.GetComponent<CombatEntityController>());

                //print("Following locked target caller called : " + other.gameObject.name);
            }
        }
        if (combatLock.Controls.isLockedOn && closestCombatEntity == null )
        {
            Debug.Log("Out of bounds");
            Delock();
        }

    }

    /// <summary>
    /// Provides the outline for the functionality of a target or potential target exiting the collision detection zone
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerExit(Collider other)
    {
        //print(combatLock.gameObject.name + " : Exit");
        //Checks if any dead enemies are still in the collidedWithCombatentities list, if they are remove them
        GuerenteeRemovalOfDeadEnemy();

        //Remove the target thats exiting the bounds
        collidedWithCombatEntities.Remove(other.gameObject);


        //Retargetting 
        bool switchingOffWhileTargetting = false;
        if (other.gameObject != previousClosestCombatEntity && other.gameObject != closestCombatEntity)
        {
            switchingOffWhileTargetting = true;
        }


        //Checks if the one exiting is the previous, or closest, if it is, we don't need to track or worry about them anymore because they arent able to be collied with, so remove them
        if (other.gameObject == previousClosestCombatEntity)
            previousClosestCombatEntity = null;

        if (other.gameObject == closestCombatEntity)
            closestCombatEntity = null;

        if(collidedWithCombatEntities.Count <= 0)
        {
            combatLock.combatEntityInLockedZone = false;
        }

        //Checks if this is the last enemy that has left the zone, and we are not switching off while targetting
        if (collidedWithCombatEntities.Count == 0 && !switchingOffWhileTargetting)
        {
            Delock();

            //Returns the collider detector's rotation to 0 (we are no longer locked onto something)
            transform.localEulerAngles = Vector3.zero;
        }
    }

    /// <summary>
    /// Method for when the potential target, or target goes out of bounds and when there are no more enemies left in the collider detector zone
    /// </summary>
    void Delock()
    {
        //print("Delocking");
        if (combatLock.Controls.isLockedOn)
        {
            //Debug.Log("Delock from Collider Detector");
            combatLock.ExitCombatCaller();
        }
    }

    /// <summary>
    /// Upon collision with a new potential target, determine if its the closest, and set the new locked target to whatever is closer
    /// </summary>
    /// <param name="other"></param>
    void CollideWithNewCombatEntity(Collider other)
    {
        //print("Collided with new combat entity");
    
        closestCombatEntity = DetermineWhichCombatEntityIsClosest();
        //Debug.Log("Making the closest combat entity: " + closestCombatEntity.gameObject.name);

        if (closestCombatEntity.GetComponent<CombatEntityController>() == null)
            Debug.LogError("Error: Combat entity controller script not given to a combat entity : " + other.name);
    }

    #region Targetting Order & Retarget

    /// <summary>
    /// Returns the target thats the closest to the player by distance
    /// </summary>
    /// <returns></returns>
    GameObject DetermineWhichCombatEntityIsClosest()
    {
        if (collidedWithCombatEntities.Count == 1)
            return collidedWithCombatEntities[0];

        GameObject closest = null;
        float closestDistance = Mathf.Infinity;
        Vector3 playerPos = transform.position;


        foreach (GameObject target in collidedWithCombatEntities)
        {
            //Debug.Log(target);
            Vector3 targetPos = target.transform.position;

            float dist = Vector3.Distance(playerPos, targetPos);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closest = target;
            }
        }

        return closest;
    }

    #endregion

    #region ReTarget

    /// <summary>
    /// Retargets this entity to the new target set by the parameter
    /// - Tells the CombatLock there is still a combat in the entity zone (already checked for because we are even relocking in the first place) (this is just to make sure)
    /// - Sets the new closest combat entity to this target (even though it may be false) (is this neccessary?)
    /// - Tells CombatLock the new locked target is this retargeted target
    /// - Tells Combat lock to Lock onto that target with a method call
    /// - Sets the previous closest combat entity to this retarget (setup for any additional retargets)
    /// - Sets the flag that we have made our targgeting descsion (perfomance)
    /// </summary>
    /// <param name="newTarget"></param>
    void RetargetTo(GameObject newTarget)
    {
        print("retargeting to new target: " + newTarget.name);
        CancelInvoke("StopRetargetting");

        combatLock.Controls.currentlyRetargetting = true;
        Invoke("StopRetargetting", retargetTime);
        
        combatLock.combatEntityInLockedZone = true;

        closestCombatEntity = newTarget;
        previousClosestCombatEntity = newTarget;

        //combatLock.Controls.CombatFollowTarget?.Invoke(newTarget.GetComponent<CombatEntityController>());

        targetDescisionMade = true;
    }


    /// <summary>
    /// Determine if we want to switch targets (called when we lock and then unlock and relock again)
    /// - Check if the closest target is the previous closest target (the one we are locked onto vs the previous lock on)
    /// - If it is, we are locking on to the same target twice (what we dont want) so lock onto a new target instead with RetargetLock
    /// </summary>
    void DetermineIfWantToSwitchToOtherTargetAvaliable()
    {
        //Debug.Log("determing if we want to switch targets");
        if (closestCombatEntity == previousClosestCombatEntity)
        {
           // Debug.Log("Yes, switch");

            GameObject newTargetLock = RetargetLock();
            Debug.Log(combatLock.gameObject.name + " : Retarget wants to lock onto:" + collidedWithCombatEntities[collidedWithCombatEntities.IndexOf(newTargetLock)].name);
            RetargetTo(newTargetLock);
        }

    }

    /// <summary>
    /// Retargets on the entity that currently isnt the target
    /// </summary>
    /// <returns></returns>
    GameObject RetargetLock()
    {
        int indexOfCurrentClosestCombatEntity = collidedWithCombatEntities.IndexOf(closestCombatEntity);
        //Debug.Log("Current closest's index:" + indexOfCurrentClosestCombatEntity);

        if (indexOfCurrentClosestCombatEntity >= collidedWithCombatEntities.Count - 1)
            return collidedWithCombatEntities[0];
        else
            return collidedWithCombatEntities[indexOfCurrentClosestCombatEntity + 1];

    }

    #endregion

    /// <summary>
    /// Sets the targetDescsion made flag, which ensures the collider detector isnt detecting stuff that doesnt matter (performance)
    /// </summary>
    public void UnLockFromCombatLock()
    {
        //print("collider detector: unlocking from combat");
        targetDescisionMade = false;
        //previousClosestCombatEntity = null;
    }

    /// <summary>
    /// Guarentees that any death's this Entity causes, causes them to be removed from the collidedWithCombatEntities list
    /// - Checks if the current capacity is greater than 0 (Is this neccessary?)
    /// - Loops through every target in collisiondetector, if they are null, remove them
    /// </summary>
    public void GuerenteeRemovalOfDeadEnemy()
    {
        //Debug.Log(collidedWithCombatEntities.Count);
        if (collidedWithCombatEntities.Capacity > 0)
        {
            for (int i = 0; i < collidedWithCombatEntities.Count - 1; i++)
            {
                //Debug.Log(collidedWithCombatEntities[0]);
                if (collidedWithCombatEntities[i] == null)
                    collidedWithCombatEntities.RemoveAt(i);
            }
        }
    }

    public IEnumerator ReturnToPreLockedUnlockedState()
    {
        yield return new WaitForSeconds(retargetTime + 0.5f);
        //print("Returning to prelocked unlocked state");
        if (!combatLock.Controls.isLockedOn && !combatLock.Controls.currentlyRetargetting)
        {
            previousClosestCombatEntity = null;
            //print("Returned");

        }
    }


    void StopRetargetting()
    {
        //print("retargetting period ended");
       combatLock.Controls.currentlyRetargetting = false;
    }
}
