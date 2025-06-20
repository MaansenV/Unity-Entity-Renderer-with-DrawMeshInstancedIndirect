using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpriteDataAuthoring : MonoBehaviour
{
    [Tooltip("The scale multiplier for the sprite. Will be multiplied with the GameObject's scale.")]
    public float Scale = 0.3f;
    
    [Tooltip("The tint color for the sprite. White (1,1,1,1) means no tint.")]
    public float4 Color = new float4(1.0f, 1.0f, 1.0f, 1.0f);
    
    [Tooltip("The index of the sprite in the texture atlas (0-15 for a 4x4 atlas)")]
    public int UVIndex;
    
    [Header("Sprite Flipping")]
    [Tooltip("Flip the sprite horizontally")]
    public bool FlipX = false;
    
    [Tooltip("Flip the sprite vertically")]
    public bool FlipY = false;
    
    [Header("Sorting")]
    [Tooltip("The rendering layer. Lower values render behind higher values.\n" +
             "Background: -1000, Floor: -500, Default: 0, Character: 100, Effects: 500, UI: 1000")]
    [SortingLayer]
    public int SortingLayer = 0;
    
    [Tooltip("The order within the same sorting layer and Y position. Higher values render on top.")]
    public int OrderInLayer = 0;
    
    [Header("Material")]
    [Tooltip("Material ID for batching sprites with the same material. 0 = default material")]
    public int MaterialID = 0;

    class Baker : Baker<SpriteDataAuthoring>
    {
        public override void Bake(SpriteDataAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var transform = authoring.transform;
            
            var flipFlags = SpriteFlipFlags.None;
            if (authoring.FlipX) flipFlags |= SpriteFlipFlags.FlipX;
            if (authoring.FlipY) flipFlags |= SpriteFlipFlags.FlipY;
            
            var data = new SpriteData
            {
                TranslationAndRotation = new float4(
                    transform.position.x,
                    transform.position.y,
                    transform.position.z,
                    transform.rotation.eulerAngles.z * Mathf.Deg2Rad
                ),
                Scale = authoring.Scale * transform.localScale.x,
                Color = authoring.Color,
                UVIndex = authoring.UVIndex,
                SortingKey = 0, // Will be calculated by SpriteSortingSystem
                FlipFlags = flipFlags,
                MaterialID = authoring.MaterialID
            };

            AddComponent(entity, data);
            AddComponent(entity, new SortingLayer { Value = authoring.SortingLayer });
            AddComponent(entity, new OrderInLayer { Value = authoring.OrderInLayer });
        }
    }
}