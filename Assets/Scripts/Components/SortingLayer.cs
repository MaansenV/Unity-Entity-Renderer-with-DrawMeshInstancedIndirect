using Unity.Entities;

public struct SortingLayer : IComponentData
{
    public int Value;
    
    // Common layer presets
    public const int Background = -1000;
    public const int Floor = -500;
    public const int Default = 0;
    public const int Character = 100;
    public const int Effects = 500;
    public const int UI = 1000;
}