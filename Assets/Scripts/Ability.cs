using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability")]
public class Ability : ScriptableObject
{
    public enum AbilityType
    {
        Attack = 0,
    }
    public AbilityType type;

    public float damage;


}
