using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    public partial struct SpawnSystem : ISystem
    {
        private Random _random;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _random = new Random(555);
            state.RequireForUpdate<EntitiesReferences>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var references = SystemAPI.GetSingleton<EntitiesReferences>();

            for (int i = 0; i < 10; i++)
            {
                var entity = state.EntityManager.Instantiate(references.BulletPrefab);
                var data = state.EntityManager.GetComponentData<SpriteData>(entity);
                data.TranslationAndRotation =
                    new float4(_random.NextFloat(-8.0f, 8.0f), _random.NextFloat(-8.0f, 8.0f), 0, 0);
                data.Scale = _random.NextFloat(0.2f, 0.6f);
                data.Color = new float4(1.0f, 1.0f, 1.0f, 1.0f);
                data.UVIndex = _random.NextInt(0, 5);
                state.EntityManager.SetComponentData(entity, data);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}