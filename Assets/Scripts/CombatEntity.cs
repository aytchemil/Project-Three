using UnityEngine;


public class CombatEntity : MonoBehaviour
{
    //Adjustable Variables
    public float health;
    public float maxHealth;

    //Component References
    public CombatEntity lockedTarget;
    public LayerMask combatEntityMask;
    public bool combatEntityInLockedZone;

    //Cache
    ColliderDetector myColliderDetector;

    //Asset References
    public GameObject colliderDetecterAsset;


    //Flags
    public bool isAlive;
    public bool isLockedOntoBySomething; //Later make this a list so multiple things can lock onto a Combat Entity
    public bool isLockedOntoSomething;


    protected virtual void Awake()
    {
        Respawn();
    }

    protected virtual void Respawn()
    {
        isAlive = true;
        health = maxHealth;
        InstantiateColliderDetector();
    }

    void InstantiateColliderDetector()
    {
        myColliderDetector = Instantiate(colliderDetecterAsset, transform, false).GetComponent<ColliderDetector>();
        myColliderDetector.myCombatEntity = this;
    }








}
