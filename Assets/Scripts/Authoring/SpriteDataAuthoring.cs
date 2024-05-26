using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpriteDataAuthoring : MonoBehaviour
{
    public float4 TranslationAndRotation;
    public float Scale = 0.3f;
    public float4 Color = new float4(1.0f, 1.0f, 1.0f, 1.0f);
    public int UVIndex;

    class Baker : Baker<SpriteDataAuthoring>
    {
        public override void Bake(SpriteDataAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var data = new SpriteData
            {
                TranslationAndRotation = authoring.TranslationAndRotation,
                Scale = authoring.Scale,
                Color = authoring.Color,
                UVIndex = authoring.UVIndex
            };

            AddComponent(entity, data);
        }
    }
}