using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;

[CreateAssetMenu(fileName = "NewModeFunctionality", menuName = "Mode/ModeFunctionality")]
public class ModeFunctionalityReferenceSO : ScriptableObject
{
    [SerializeField] private string modeName;
    [SerializeField] private MonoScript modeScript; // Reference to the script asset

    public string ModeName => modeName;

    // Get the Type, with validation
    public Type GetModeType()
    {
        if (modeScript == null)
        {
            Debug.LogError($"[ModeDataSO] No script assigned in {name}");
            return null;
        }

        Type type = modeScript.GetClass();
        if (type != null && typeof(ModeGeneralFunctionality).IsAssignableFrom(type) && type.IsSubclassOf(typeof(MonoBehaviour)))
        {
            return type;
        }

        Debug.LogError($"[ModeDataSO] Script {modeScript.name} is not a valid ModeGeneralFunctionality type in {name}");
        return null;
    }

    // Validate in Editor
    private void OnValidate()
    {
        if (modeScript != null)
        {
            Type type = modeScript.GetClass();
            if (type == null || !typeof(ModeGeneralFunctionality).IsAssignableFrom(type) || !type.IsSubclassOf(typeof(MonoBehaviour)))
            {
                Debug.LogError($"[ModeDataSO] Assigned script {modeScript.name} is not a valid ModeGeneralFunctionality type in {name}");
                modeScript = null;
            }
        }
    }
}