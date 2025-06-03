using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "AttackAbility", menuName = "ScriptableObjects/Abilities/Attack Ability")]
public class AbilityAttack : Ability
{
    public float damage;
    public float flinchAmount = 1f;

    public GameObject triggerCollider;
    public enum AnimationConnection
    {
        Idle = 0,
        Attack_Flat_Left = 1,
        Attack_Flat_Right = 2,
        Attack_Up_Right = 3,
        Attack_Overhead_Center = 4,
        Attack_Diagnal_Right = 5,
        Attack_Diagnal_Left = 6
    }
    public AnimationConnection anim_name;
    public override GameObject prefab
    {
        get => triggerCollider;
        set => triggerCollider = value;
    }

}
