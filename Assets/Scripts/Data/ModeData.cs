using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mode", menuName = "ScriptableObjects/Mode")]
public class ModeData : ScriptableObject
{
    public string modeName;
    public AbilitySet abilitySet;
    public Ability currentAbility;
    public ModeGeneralFunctionality modeFunctionality;
    public bool solo;
    public bool initializedTriggers;
    public Texture UIIndicator;
    public string modeTextDesc;
}
