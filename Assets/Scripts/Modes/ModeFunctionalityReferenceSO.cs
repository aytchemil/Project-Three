using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewModeFunctionality", menuName = "Mode/ModeFunctionality")]
public class ModeFunctionalityReferenceSO : ScriptableObject
{
    [SerializeField] private string modeName;
    [SerializeField] private string modeTypeName; // Store the type name as a string for runtime

#if UNITY_EDITOR
    [SerializeField] private MonoScript modeScript; // Editor-only field for selecting the script
#endif

    public string ModeName => modeName;

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