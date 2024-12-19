using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability")]
public class Ability : ScriptableObject
{
    public enum AbilityType
    {
        Attack = 0,
    }
    public AbilityType type;

    public Texture icon;

    public float damage;


}
