using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(SpriteSortingSystem))]
public partial class SpriteSortingDebugSystem : SystemBase
{
    private bool debugEnabled = false;
    
    protected override void OnUpdate()
    {
        // Toggle debug with F1
        if (Keyboard.current != null && Keyboard.current.f1Key.wasPressedThisFrame)
        {
            debugEnabled = !debugEnabled;
            Debug.Log($"Sprite Sorting Debug: {(debugEnabled ? "Enabled" : "Disabled")}");
        }
        
        if (!debugEnabled)
            return;
            
        // Get sorted sprites
        if (!SystemAPI.TryGetSingleton<SortedSprites>(out var sortedSprites))
            return;
            
        if (!sortedSprites.IsCreated || sortedSprites.Entities.Length == 0)
            return;
            
        // Display info for first 10 sprites
        int count = math.min(10, sortedSprites.Entities.Length);
        Debug.Log($"=== Sprite Sorting Debug (showing first {count} of {sortedSprites.Entities.Length} sprites) ===");
        
        for (int i = 0; i < count; i++)
        {
            var entity = sortedSprites.Entities[i];
            
            if (!EntityManager.HasComponent<SpriteData>(entity))
                continue;
                
            var spriteData = EntityManager.GetComponentData<SpriteData>(entity);
            var hasLocalToWorld = EntityManager.HasComponent<LocalToWorld>(entity);
            var hasSortingLayer = EntityManager.HasComponent<SortingLayer>(entity);
            var hasOrderInLayer = EntityManager.HasComponent<OrderInLayer>(entity);
            
            string info = $"[{i}] Entity {entity.Index}:{entity.Version}";
            
            if (hasLocalToWorld)
            {
                var ltw = EntityManager.GetComponentData<LocalToWorld>(entity);
                info += $" Pos({ltw.Position.x:F2}, {ltw.Position.y:F2}, {ltw.Position.z:F2})";
            }
            
            if (hasSortingLayer)
            {
                var layer = EntityManager.GetComponentData<SortingLayer>(entity);
                info += $" Layer:{layer.Value}";
            }
            
            if (hasOrderInLayer)
            {
                var order = EntityManager.GetComponentData<OrderInLayer>(entity);
                info += $" Order:{order.Value}";
            }
            
            info += $" SortKey:0x{spriteData.SortingKey:X16}";
            
            Debug.Log(info);
        }
    }
}

