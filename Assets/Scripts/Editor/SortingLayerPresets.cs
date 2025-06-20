using UnityEditor;
using UnityEngine;

public static class SortingLayerPresets
{
    [MenuItem("GameObject/2D Roguelike/Sorting Layers/Set Background (-1000)", false, 0)]
    private static void SetBackgroundLayer()
    {
        SetSortingLayerOnSelection(SortingLayer.Background);
    }
    
    [MenuItem("GameObject/2D Roguelike/Sorting Layers/Set Floor (-500)", false, 0)]
    private static void SetFloorLayer()
    {
        SetSortingLayerOnSelection(SortingLayer.Floor);
    }
    
    [MenuItem("GameObject/2D Roguelike/Sorting Layers/Set Default (0)", false, 0)]
    private static void SetDefaultLayer()
    {
        SetSortingLayerOnSelection(SortingLayer.Default);
    }
    
    [MenuItem("GameObject/2D Roguelike/Sorting Layers/Set Character (100)", false, 0)]
    private static void SetCharacterLayer()
    {
        SetSortingLayerOnSelection(SortingLayer.Character);
    }
    
    [MenuItem("GameObject/2D Roguelike/Sorting Layers/Set Effects (500)", false, 0)]
    private static void SetEffectsLayer()
    {
        SetSortingLayerOnSelection(SortingLayer.Effects);
    }
    
    [MenuItem("GameObject/2D Roguelike/Sorting Layers/Set UI (1000)", false, 0)]
    private static void SetUILayer()
    {
        SetSortingLayerOnSelection(SortingLayer.UI);
    }
    
    private static void SetSortingLayerOnSelection(int layer)
    {
        foreach (GameObject go in Selection.gameObjects)
        {
            var authoring = go.GetComponent<SpriteDataAuthoring>();
            if (authoring != null)
            {
                Undo.RecordObject(authoring, "Set Sorting Layer");
                authoring.SortingLayer = layer;
                EditorUtility.SetDirty(authoring);
            }
        }
    }
    
    // Validate menu items only when SpriteDataAuthoring components are selected
    [MenuItem("GameObject/2D Roguelike/Sorting Layers/Set Background (-1000)", true)]
    [MenuItem("GameObject/2D Roguelike/Sorting Layers/Set Floor (-500)", true)]
    [MenuItem("GameObject/2D Roguelike/Sorting Layers/Set Default (0)", true)]
    [MenuItem("GameObject/2D Roguelike/Sorting Layers/Set Character (100)", true)]
    [MenuItem("GameObject/2D Roguelike/Sorting Layers/Set Effects (500)", true)]
    [MenuItem("GameObject/2D Roguelike/Sorting Layers/Set UI (1000)", true)]
    private static bool ValidateSortingLayerMenuItem()
    {
        foreach (GameObject go in Selection.gameObjects)
        {
            if (go.GetComponent<SpriteDataAuthoring>() != null)
                return true;
        }
        return false;
    }
}