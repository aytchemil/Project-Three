using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomPropertyDrawer(typeof(TypeVariable))]
public class TypeVariablePropertyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var container = new VisualElement();

        var objectField = new ObjectField(property.displayName)
        {
            objectType = typeof(TypeVariable)
        };
        objectField.BindProperty(property);

        var valueLabel = new Label();
        valueLabel.style.paddingLeft = 20;


        container.Add(objectField);
        container.Add(valueLabel);

        objectField.RegisterValueChangedCallback(
            evt =>
            {
                var variable = evt.newValue as TypeVariable;
                if (variable != null)
                {
                    valueLabel.text = $"Current Value: {variable.Value}";
                    variable.OnValueChanged += newValue => valueLabel.text = $"Current Value: {newValue}";
                }
                else
                    valueLabel.text = string.Empty;
            });

        var curretVariable = property.objectReferenceValue as TypeVariable;
        if(curretVariable != null)
        {
            valueLabel.text = $"Curretn Value: {curretVariable.Value}";
            curretVariable.OnValueChanged += newValue => valueLabel.text = $"Current Value: {newValue}";
        }

        return container;
    }
}
