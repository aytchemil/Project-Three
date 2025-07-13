// RuntimeTypeReference.cs
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "SOAP/Variable Type")]
public class TypeVariable : ScriptableObject
{
    Type value;
    public Type Value
    {
        get => value;
        set
        {
            if (this.value == value) return;
            this.value = value;
            OnValueChanged.Invoke(this.value);
        }
    }

    public event UnityAction<Type> OnValueChanged = delegate { };

#if UNITY_EDITOR
    [SerializeField] private MonoScript script;

    private void OnValidate()
    {
        if (script == null) return;

        Type scriptType = script.GetClass();

        if (scriptType == null) return;
        if (scriptType == Value) return;

        Value = scriptType;

        //Debug.Log($"TypeVariable Set Type Value to {Value}");
    }
#endif

}