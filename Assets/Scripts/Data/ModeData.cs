using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mode", menuName = "ScriptableObjects/Mode")]
public class ModeTemplate : ScriptableObject
{
    public string modeName;
    public AbilitySet abilitySet;
    public ModeGeneralFunctionality modeFunctionality;
    public bool isAbility;
    [ShowIf("isAbility")]
    public bool abilityIndividualSelection;
    public bool initializedTriggers;
    public Texture UIIndicator;
    public string modeTextDesc;
}
