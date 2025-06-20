using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(SpriteSystem))]
public partial struct SpriteTransformSyncSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (spriteData, transform, localToWorld) in 
                 SystemAPI.Query<RefRW<SpriteData>, RefRO<LocalTransform>, RefRO<LocalToWorld>>())
        {
            // Extract rotation angle from the transform matrix
            float angle = math.atan2(localToWorld.ValueRO.Value.c0.y, localToWorld.ValueRO.Value.c0.x);
            
            // Sync position from transform to sprite data
            spriteData.ValueRW.TranslationAndRotation = new float4(
                localToWorld.ValueRO.Position.x,
                localToWorld.ValueRO.Position.y,
                localToWorld.ValueRO.Position.z,
                angle
            );
            
            // Sync scale
            spriteData.ValueRW.Scale = transform.ValueRO.Scale;
        }
    }
}