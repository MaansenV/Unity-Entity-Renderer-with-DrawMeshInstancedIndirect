using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using System;

[Serializable]
public struct AnimationClipDefinition
{
    [Tooltip("Name of the animation (e.g., 'Idle', 'Walk', 'Attack')")]
    public string Name;
    
    [Tooltip("First frame index in the sprite atlas")]
    public int StartFrame;
    
    [Tooltip("Number of frames in this animation")]
    public int FrameCount;
}

public class AnimationAuthoring : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("Frames per second for animation playback")]
    public float FrameRate = 12f;
    
    [Tooltip("Start playing automatically")]
    public bool PlayOnStart = true;
    
    [Tooltip("Loop the animation")]
    public bool Loop = true;
    
    [Tooltip("Initial animation to play (by index)")]
    public int InitialAnimation = 0;
    
    [Header("Animation Clips")]
    [Tooltip("Define animation clips for this sprite")]
    public AnimationClipDefinition[] AnimationClips;
    
    class Baker : Baker<AnimationAuthoring>
    {
        public override void Bake(AnimationAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            // Add animation data component
            AddComponent(entity, new AnimationData
            {
                CurrentFrame = 0,
                FrameTimer = 0f,
                FrameRate = authoring.FrameRate,
                AnimationID = authoring.InitialAnimation,
                IsPlaying = authoring.PlayOnStart,
                Loop = authoring.Loop
            });
            
            // Add animation clips buffer
            if (authoring.AnimationClips != null && authoring.AnimationClips.Length > 0)
            {
                var buffer = AddBuffer<AnimationClipBuffer>(entity);
                
                foreach (var clipDef in authoring.AnimationClips)
                {
                    buffer.Add(new AnimationClipBuffer
                    {
                        Clip = new AnimationClipData
                        {
                            StartFrame = clipDef.StartFrame,
                            FrameCount = math.max(1, clipDef.FrameCount),
                            Name = new FixedString32Bytes(clipDef.Name ?? "Unnamed")
                        }
                    });
                }
            }
        }
    }
}