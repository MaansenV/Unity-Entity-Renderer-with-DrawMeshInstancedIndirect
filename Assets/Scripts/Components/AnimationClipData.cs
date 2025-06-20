using Unity.Entities;
using Unity.Collections;

public struct AnimationClipData : IComponentData
{
    public int StartFrame;
    public int FrameCount;
    public FixedString32Bytes Name;
    
    // Helper to get frame index for a given animation frame
    public int GetFrameIndex(int animationFrame)
    {
        return StartFrame + (animationFrame % FrameCount);
    }
}