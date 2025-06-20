using Unity.Collections;
using Unity.Entities;

// Stores multiple animation clips for an entity
[InternalBufferCapacity(8)]
public struct AnimationClipBuffer : IBufferElementData
{
    public AnimationClipData Clip;
}