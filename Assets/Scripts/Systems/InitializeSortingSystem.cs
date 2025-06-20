using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct InitializeSortingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // Create singleton entity for sorted sprites
        var entity = state.EntityManager.CreateEntity();
        state.EntityManager.AddComponentData(entity, new SortedSprites
        {
            Entities = default,
            IsCreated = false
        });
    }
}