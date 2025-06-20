using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(SpriteSortingSystem))]
public partial struct AddDefaultSortingComponentsSystem : ISystem
{
    private EntityQuery spritesWithoutSortingLayer;
    private EntityQuery spritesWithoutOrderInLayer;
    
    public void OnCreate(ref SystemState state)
    {
        // Query for sprites without sorting components
        var builder1 = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<SpriteData>()
            .WithNone<SortingLayer>();
        spritesWithoutSortingLayer = state.GetEntityQuery(builder1);
        
        var builder2 = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<SpriteData>()
            .WithNone<OrderInLayer>();
        spritesWithoutOrderInLayer = state.GetEntityQuery(builder2);
    }
    
    public void OnUpdate(ref SystemState state)
    {
        // Add default SortingLayer to sprites that don't have one
        var entitiesWithoutLayer = spritesWithoutSortingLayer.ToEntityArray(Allocator.Temp);
        for (int i = 0; i < entitiesWithoutLayer.Length; i++)
        {
            state.EntityManager.AddComponentData(entitiesWithoutLayer[i], new SortingLayer { Value = 0 });
        }
        entitiesWithoutLayer.Dispose();
        
        var entitiesWithoutOrder = spritesWithoutOrderInLayer.ToEntityArray(Allocator.Temp);
        for (int i = 0; i < entitiesWithoutOrder.Length; i++)
        {
            state.EntityManager.AddComponentData(entitiesWithoutOrder[i], new OrderInLayer { Value = 0 });
        }
        entitiesWithoutOrder.Dispose();
    }
}
