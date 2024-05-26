using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public partial class SpriteSystem : SystemBase
{
    private EntityQuery spriteQuery;
    private ComputeBuffer translationAndRotationBuffer;
    private ComputeBuffer scaleBuffer;
    private ComputeBuffer colorBuffer;
    private ComputeBuffer uvBuffer;
    private ComputeBuffer uvIndexBuffer;
    private uint[] args;
    private ComputeBuffer argsBuffer;
    private int spriteCount;
    private Material defaultMaterial;
    private Mesh quadMesh;

    private const int UV_X_ELEMENTS = 4;
    private const int UV_Y_ELEMENTS = 4;

    protected override void OnCreate()
    {
        spriteQuery = GetEntityQuery(ComponentType.ReadOnly<SpriteData>());
    }

    protected override void OnStartRunning()
    {
        spriteCount = spriteQuery.CalculateEntityCount() + 1000000;

        // Initialize UV buffer
        const int uvCount = UV_X_ELEMENTS * UV_Y_ELEMENTS;
        float4[] uvs = new float4[uvCount];
        for (int u = 0; u < UV_X_ELEMENTS; u++)
        {
            for (int v = 0; v < UV_Y_ELEMENTS; v++)
            {
                int index = v * UV_X_ELEMENTS + u;
                uvs[index] = new float4(0.25f, 0.25f, u * 0.25f, v * 0.25f);
            }
        }

        uvBuffer = new ComputeBuffer(uvs.Length, 16);
        uvBuffer.SetData(uvs);

        translationAndRotationBuffer = new ComputeBuffer(spriteCount, 16);
        scaleBuffer = new ComputeBuffer(spriteCount, sizeof(float));
        colorBuffer = new ComputeBuffer(spriteCount, 16);
        uvIndexBuffer = new ComputeBuffer(spriteCount, sizeof(int));

        args = new uint[] { 6, (uint)spriteCount, 0, 0, 0 };
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(args);

        defaultMaterial = Resources.Load<Material>("Third");

        if (defaultMaterial != null)
        {
            int uvBufferId = Shader.PropertyToID("uvBuffer");
            defaultMaterial.SetBuffer(uvBufferId, uvBuffer);

            int translationAndRotationBufferId = Shader.PropertyToID("translationAndRotationBuffer");
            defaultMaterial.SetBuffer(translationAndRotationBufferId, translationAndRotationBuffer);

            int scaleBufferId = Shader.PropertyToID("scaleBuffer");
            defaultMaterial.SetBuffer(scaleBufferId, scaleBuffer);

            int uvIndexBufferId = Shader.PropertyToID("uvIndexBuffer");
            defaultMaterial.SetBuffer(uvIndexBufferId, uvIndexBuffer);

            int colorsBufferId = Shader.PropertyToID("colorsBuffer");
            defaultMaterial.SetBuffer(colorsBufferId, colorBuffer);
        }
        else
        {
            Debug.LogError("Default material could not be loaded.");
        }
    }

    protected override void OnUpdate()
    {
        spriteCount = spriteQuery.CalculateEntityCount();
        if (spriteCount == 0)
            return;

        var translationAndRotations = new NativeArray<float4>(spriteCount, Allocator.TempJob);
        var scales = new NativeArray<float>(spriteCount, Allocator.TempJob);
        var colors = new NativeArray<float4>(spriteCount, Allocator.TempJob);
        var uvIndices = new NativeArray<int>(spriteCount, Allocator.TempJob);

        var spriteDataArray = spriteQuery.ToComponentDataArray<SpriteData>(Allocator.TempJob);

        var job = new CollectDataJob
        {
            SpriteDataArray = spriteDataArray,
            TranslationAndRotations = translationAndRotations,
            Scales = scales,
            Colors = colors,
            UVIndices = uvIndices
        };

        var handle = job.Schedule(spriteCount, 64, Dependency);
        handle.Complete();

        translationAndRotationBuffer.SetData(translationAndRotations);
        scaleBuffer.SetData(scales);
        colorBuffer.SetData(colors);
        uvIndexBuffer.SetData(uvIndices);

        translationAndRotations.Dispose();
        scales.Dispose();
        colors.Dispose();
        uvIndices.Dispose();
        spriteDataArray.Dispose();

        // Update args buffer
        args[1] = (uint)spriteCount;
        argsBuffer.SetData(args);

        if (defaultMaterial != null)
        {
            Graphics.DrawMeshInstancedIndirect(GetQuad(), 0, defaultMaterial, BOUNDS, argsBuffer);
        }
        else
        {
            Debug.LogError("Default material is null");
        }
    }

    protected override void OnDestroy()
    {
        translationAndRotationBuffer?.Release();
        scaleBuffer?.Release();
        colorBuffer?.Release();
        uvBuffer?.Release();
        uvIndexBuffer?.Release();
        argsBuffer?.Release();
    }

    private Mesh GetQuad()
    {
        if (quadMesh == null)
        {
            quadMesh = new Mesh();
            Vector3[] vertices = new Vector3[4];
            vertices[0] = new Vector3(0, 0, 0);
            vertices[1] = new Vector3(1, 0, 0);
            vertices[2] = new Vector3(0, 1, 0);
            vertices[3] = new Vector3(1, 1, 0);
            quadMesh.vertices = vertices;

            int[] tri = new int[6];
            tri[0] = 0;
            tri[1] = 2;
            tri[2] = 1;
            tri[3] = 2;
            tri[4] = 3;
            tri[5] = 1;
            quadMesh.triangles = tri;

            Vector3[] normals = new Vector3[4];
            normals[0] = -Vector3.forward;
            normals[1] = -Vector3.forward;
            normals[2] = -Vector3.forward;
            normals[3] = -Vector3.forward;
            quadMesh.normals = normals;

            Vector2[] uv = new Vector2[4];
            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(1, 0);
            uv[2] = new Vector2(0, 1);
            uv[3] = new Vector2(1, 1);
            quadMesh.uv = uv;
        }

        return quadMesh;
    }

    [BurstCompile]
    private struct CollectDataJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<SpriteData> SpriteDataArray;
        public NativeArray<float4> TranslationAndRotations;
        public NativeArray<float> Scales;
        public NativeArray<float4> Colors;
        public NativeArray<int> UVIndices;

        public void Execute(int index)
        {
            var spriteData = SpriteDataArray[index];
            TranslationAndRotations[index] = spriteData.TranslationAndRotation;
            Scales[index] = spriteData.Scale;
            Colors[index] = spriteData.Color;
            UVIndices[index] = spriteData.UVIndex;
        }
    }

    private static readonly Bounds BOUNDS = new Bounds(Vector2.zero, Vector3.one);
}