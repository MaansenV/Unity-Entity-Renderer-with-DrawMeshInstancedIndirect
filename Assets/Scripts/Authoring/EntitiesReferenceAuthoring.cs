using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class EntitiesReferenceAuthoring : UnityEngine.MonoBehaviour
{
    [Tooltip("The prefab to spawn as bullets or projectiles. Must have a SpriteDataAuthoring component.")]
    public GameObject bulletPrefab;

    
    
    private class EntitiesReferenceAuthoringBaker : Unity.Entities.Baker<EntitiesReferenceAuthoring>
    {
        public override void Bake(EntitiesReferenceAuthoring authoring)
        {
            var e = GetEntity(TransformUsageFlags.Dynamic);
            var bulletPrefab = GetEntity(authoring.bulletPrefab,TransformUsageFlags.Dynamic);
            
            AddComponent(e, new EntitiesReferences
            {
                BulletPrefab = bulletPrefab,
            });
        }
    }
}