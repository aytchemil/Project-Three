using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mode", menuName = "ScriptableObjects/Mode")]
public class ModeData : ScriptableObject
{
    public string name;
    public AbilitySet abilitySet;
    public ModeGeneralFunctionality modeFunctionality;
    public bool isStance;
    [ShowIf("isStance")]
    public bool abilityIndividualSelection;
    public bool initializedTriggers;
    public Texture UIIndicator;
    public string modeTextDesc;
    [SerializeField] private string modeTypeName; // Store the type name as a string for runtime
#if UNITY_EDITOR
    [SerializeField] private MonoScript modeScript; // Editor-only field for selecting the script
#endif

    // Get the Type at runtime
    public Type GetModeType()
    {
        if (string.IsNullOrEmpty(modeTypeName))
        {
            Debug.LogError($"[ModeDataSO] No type name assigned in {name}");
            return null;
        }

        Type type = Type.GetType(modeTypeName);
        if (type != null && typeof(ModeGeneralFunctionality).IsAssignableFrom(type) && type.IsSubclassOf(typeof(MonoBehaviour)))
        {
            return type;
        }

        Debug.LogError($"[ModeDataSO] Type {modeTypeName} is not a valid ModeGeneralFunctionality type in {name}");
        return null;
    }

#if UNITY_EDITOR
    // Validate in Editor and update modeTypeName
    private void OnValidate()
    {
        if (modeScript != null)
        {
            Type type = modeScript.GetClass();
            if (type == null || !typeof(ModeGeneralFunctionality).IsAssignableFrom(type) || !type.IsSubclassOf(typeof(MonoBehaviour)))
            {
                Debug.LogError($"[ModeDataSO] Assigned script {modeScript.name} is not a valid ModeGeneralFunctionality type in {name}");
                modeScript = null;
                modeTypeName = null;
            }
            else
            {
                // Store the fully qualified type name (namespace + class) for runtime
                modeTypeName = type.AssemblyQualifiedName;
            }
        }
        else
        {
            modeTypeName = null;
        }
    }
#endif
}
