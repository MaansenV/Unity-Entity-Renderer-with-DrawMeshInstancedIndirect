using Unity.Entities;
using Unity.Mathematics;

public struct SpriteEffectData : IComponentData
{
    public float4 TintColor; // Multiplicative tint for effects
    public float FlashIntensity; // 0-1 for hit flash effect
    public float FlashTimer; // Timer for flash duration
    public float OutlineWidth; // Outline thickness (0 = no outline)
    public float4 OutlineColor; // Outline color
}

// Tag component for sprites that should flash
public struct HitFlashTag : IComponentData
{
    public float Duration;
    public float4 FlashColor;
}