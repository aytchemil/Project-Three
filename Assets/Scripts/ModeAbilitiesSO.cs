using UnityEngine;

[CreateAssetMenu(fileName = "Mode Preset", menuName = "ScriptableObjects/Mode Abilities")]
public class ModeAbilitiesSO : ScriptableObject
{
    public string modeName;
    public Ability right;
    public Ability left;
    public Ability up;
    public Ability down;
}
