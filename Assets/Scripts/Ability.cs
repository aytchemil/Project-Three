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
    }
    public CollisionType collisionType;
    public GameObject attackTriggerCollider;
    public string attackName;
    public Texture icon;
    public float speed;

    public float damage;


}
