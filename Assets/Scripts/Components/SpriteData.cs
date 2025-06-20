using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct SpriteData : IComponentData
{
    public float4 TranslationAndRotation;
    public float Scale;
    public float4 Color;
    public int UVIndex;
    public ulong SortingKey; // Used for depth sorting
    public SpriteFlipFlags FlipFlags;
    public int MaterialID; // For multi-material support
}

[System.Flags]
public enum SpriteFlipFlags : byte
{
    None = 0,
    FlipX = 1 << 0,
    FlipY = 1 << 1
}