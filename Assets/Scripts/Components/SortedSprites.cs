using Unity.Collections;
using Unity.Entities;

public struct SortedSprites : IComponentData
{
    public NativeArray<Entity> Entities;
    public bool IsCreated;
}