using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability")]
public class Ability : ScriptableObject
{
    public enum Archetype
    {
        Swordsman = 0,
    }
    public Archetype archetype;
    public enum CollisionType
    {
        Box = 0,
        Overhead = 1,
        Pierce = 2,
        SideSlash = 3,
        MovementForward = 4,
        MovementRightOrLeft = 5,
    }
    public CollisionType collisionType;
    public GameObject attackTriggerCollider;
    public string attackName;
    public Texture icon;
    public float speed;
    public float initialAttackDelay = 0.3f;

    public float damage;
    public float movementAmount;

}
