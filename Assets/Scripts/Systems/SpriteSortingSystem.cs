using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(SpriteTransformSyncSystem))]
[UpdateBefore(typeof(SpriteSystem))]
public partial struct SpriteSortingSystem : ISystem
{
    private EntityQuery spriteQuery;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        var builder = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<SpriteData, LocalToWorld>()
            .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState);
        
        spriteQuery = state.GetEntityQuery(builder);
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityCount = spriteQuery.CalculateEntityCount();
        if (entityCount == 0)
            return;
            
        // Get or create the singleton entity for sorted sprites
        if (!SystemAPI.TryGetSingletonRW<SortedSprites>(out var sortedSprites))
        {
            return; // Singleton not yet created
        }
        
        // Dispose old array if it exists
        if (sortedSprites.ValueRO.IsCreated)
        {
            sortedSprites.ValueRW.Entities.Dispose();
        }
        
        // Create arrays for sorting
        var entities = spriteQuery.ToEntityArray(Allocator.TempJob);
        var sortingKeys = new NativeArray<ulong>(entityCount, Allocator.TempJob);
        
        // Calculate sorting keys
        var calculateJob = new CalculateSortingKeysJob
        {
            Entities = entities,
            SortingKeys = sortingKeys,
            SpriteDataLookup = SystemAPI.GetComponentLookup<SpriteData>(false),
            LocalToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>(true),
            SortingLayerLookup = SystemAPI.GetComponentLookup<SortingLayer>(true),
            OrderInLayerLookup = SystemAPI.GetComponentLookup<OrderInLayer>(true)
        };
        
        var handle = calculateJob.Schedule(entityCount, 64);
        handle.Complete();
        
        // Sort entities based on their keys
        var sortJob = new SortEntitiesByKeyJob
        {
            Entities = entities,
            Keys = sortingKeys
        };
        
        handle = sortJob.Schedule();
        handle.Complete();
        
        // Store sorted entities
        sortedSprites.ValueRW.Entities = entities;
        sortedSprites.ValueRW.IsCreated = true;
        
        // Clean up temporary array
        sortingKeys.Dispose();
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        if (SystemAPI.TryGetSingletonRW<SortedSprites>(out var sortedSprites))
        {
            if (sortedSprites.ValueRO.IsCreated)
            {
                sortedSprites.ValueRW.Entities.Dispose();
            }
        }
    }
}

[BurstCompile]
public struct CalculateSortingKeysJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<Entity> Entities;
    [NativeDisableParallelForRestriction] public NativeArray<ulong> SortingKeys;
    
    [NativeDisableParallelForRestriction] public ComponentLookup<SpriteData> SpriteDataLookup;
    [ReadOnly] public ComponentLookup<LocalToWorld> LocalToWorldLookup;
    [ReadOnly] public ComponentLookup<SortingLayer> SortingLayerLookup;
    [ReadOnly] public ComponentLookup<OrderInLayer> OrderInLayerLookup;
    
    public void Execute(int index)
    {
        var entity = Entities[index];
        
        // Get components
        var localToWorld = LocalToWorldLookup[entity];
        var spriteData = SpriteDataLookup[entity];
        
        // Get sorting layer (default to 0 if not present)
        int sortingLayer = 0;
        if (SortingLayerLookup.HasComponent(entity))
        {
            sortingLayer = SortingLayerLookup[entity].Value;
        }
        
        // Get order in layer (default to 0 if not present)
        int orderInLayer = 0;
        if (OrderInLayerLookup.HasComponent(entity))
        {
            orderInLayer = OrderInLayerLookup[entity].Value;
        }
        
        // Calculate sorting key (64-bit)
        // Bits 48-63: Sorting layer (16 bits, signed -> unsigned)
        // Bits 16-47: Y position inverted (32 bits, higher Y = render on top)
        // Bits 0-15: Order in layer (16 bits, signed -> unsigned)
        
        uint layerBits = (uint)(sortingLayer + 32768); // Convert to unsigned
        uint yBits = math.asuint(-localToWorld.Position.y); // Invert Y for proper sorting
        uint orderBits = (uint)(orderInLayer + 32768); // Convert to unsigned
        
        ulong key = ((ulong)layerBits << 48) | ((ulong)yBits << 16) | orderBits;
        
        SortingKeys[index] = key;
        spriteData.SortingKey = key;
        SpriteDataLookup[entity] = spriteData;
    }
}

[BurstCompile]
public struct SortEntitiesByKeyJob : IJob
{
    public NativeArray<Entity> Entities;
    public NativeArray<ulong> Keys;
    
    public void Execute()
    {
        // Use a simple but efficient insertion sort for smaller arrays
        // or quicksort for larger arrays
        int n = Keys.Length;
        
        if (n <= 64)
        {
            // Insertion sort for small arrays
            for (int i = 1; i < n; i++)
            {
                ulong keyToInsert = Keys[i];
                Entity entityToInsert = Entities[i];
                int j = i - 1;
                
                while (j >= 0 && Keys[j] > keyToInsert)
                {
                    Keys[j + 1] = Keys[j];
                    Entities[j + 1] = Entities[j];
                    j--;
                }
                
                Keys[j + 1] = keyToInsert;
                Entities[j + 1] = entityToInsert;
            }
        }
        else
        {
            // For larger arrays, use Unity's built-in sort
            var sortedIndices = new NativeArray<int>(n, Allocator.Temp);
            var tempKeys = new NativeArray<ulong>(n, Allocator.Temp);
            var tempEntities = new NativeArray<Entity>(n, Allocator.Temp);
            
            // Initialize indices
            for (int i = 0; i < n; i++)
            {
                sortedIndices[i] = i;
            }
            
            // Sort indices based on keys
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (Keys[sortedIndices[i]] > Keys[sortedIndices[j]])
                    {
                        int temp = sortedIndices[i];
                        sortedIndices[i] = sortedIndices[j];
                        sortedIndices[j] = temp;
                    }
                }
            }
            
            // Apply sorted order
            for (int i = 0; i < n; i++)
            {
                tempKeys[i] = Keys[sortedIndices[i]];
                tempEntities[i] = Entities[sortedIndices[i]];
            }
            
            // Copy back
            Keys.CopyFrom(tempKeys);
            Entities.CopyFrom(tempEntities);
            
            // Cleanup
            sortedIndices.Dispose();
            tempKeys.Dispose();
            tempEntities.Dispose();
        }
    }
}
