﻿using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct SpriteData : IComponentData
{
    public float4 TranslationAndRotation;
    public float Scale;
    public float4 Color;
    public int UVIndex;
}