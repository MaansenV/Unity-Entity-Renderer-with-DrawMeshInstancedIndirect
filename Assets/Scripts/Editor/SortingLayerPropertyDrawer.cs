using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SortingLayerAttribute))]
public class SortingLayerPropertyDrawer : PropertyDrawer
{
    private readonly string[] layerNames = new string[]
    {
        "Background (-1000)",
        "Floor (-500)",
        "Default (0)",
        "Character (100)",
        "Effects (500)",
        "UI (1000)",
        "Custom..."
    };
    
    private readonly int[] layerValues = new int[]
    {
        SortingLayer.Background,
        SortingLayer.Floor,
        SortingLayer.Default,
        SortingLayer.Character,
        SortingLayer.Effects,
        SortingLayer.UI,
        0 // Custom
    };

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.Integer)
        {
            EditorGUI.PropertyField(position, property, label);
            return;
        }

        EditorGUI.BeginProperty(position, label, property);
        
        // Find current selection
        int currentValue = property.intValue;
        int selectedIndex = System.Array.IndexOf(layerValues, currentValue);
        
        // If not found, it's a custom value
        if (selectedIndex == -1)
        {
            selectedIndex = layerValues.Length - 1; // Custom
        }
        
        // Split rect for dropdown and value field
        Rect dropdownRect = new Rect(position.x, position.y, position.width * 0.6f, position.height);
        Rect valueRect = new Rect(position.x + position.width * 0.65f, position.y, position.width * 0.35f, position.height);
        
        // Draw label
        EditorGUI.PrefixLabel(dropdownRect, GUIUtility.GetControlID(FocusType.Passive), label);
        
        // Adjust dropdown rect
        dropdownRect.x += EditorGUIUtility.labelWidth;
        dropdownRect.width -= EditorGUIUtility.labelWidth;
        
        // Draw dropdown
        EditorGUI.BeginChangeCheck();
        int newIndex = EditorGUI.Popup(dropdownRect, selectedIndex, layerNames);
        
        if (EditorGUI.EndChangeCheck() && newIndex != layerValues.Length - 1)
        {
            property.intValue = layerValues[newIndex];
        }
        
        // Draw value field
        EditorGUI.BeginChangeCheck();
        int newValue = EditorGUI.IntField(valueRect, property.intValue);
        if (EditorGUI.EndChangeCheck())
        {
            property.intValue = newValue;
        }
        
        EditorGUI.EndProperty();
    }
}