using UnityEngine;

[CreateAssetMenu(fileName = "Ability Set", menuName = "ScriptableObjects/Ability Set")]
public class AbilitySet : ScriptableObject
{
    public ModeManager.Modes mode;
    public Ability right;
    public Ability left;
    public Ability up;
    public Ability down;
}
