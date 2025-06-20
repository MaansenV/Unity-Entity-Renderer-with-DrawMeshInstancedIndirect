using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(SpriteTransformSyncSystem))]
public partial struct SpriteAnimationSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        
        // Update animation timers and frames
        foreach (var (animData, spriteData, animClips) in 
                 SystemAPI.Query<RefRW<AnimationData>, RefRW<SpriteData>, DynamicBuffer<AnimationClipBuffer>>())
        {
            if (!animData.ValueRO.IsPlaying)
                continue;
                
            // Update frame timer
            animData.ValueRW.FrameTimer += deltaTime;
            
            // Check if we need to advance to next frame
            float frameDuration = 1f / animData.ValueRO.FrameRate;
            if (animData.ValueRW.FrameTimer >= frameDuration)
            {
                animData.ValueRW.FrameTimer -= frameDuration;
                animData.ValueRW.CurrentFrame++;
                
                // Get current animation clip
                if (animData.ValueRO.AnimationID >= 0 && animData.ValueRO.AnimationID < animClips.Length)
                {
                    var clip = animClips[animData.ValueRO.AnimationID].Clip;
                    
                    // Check if animation finished
                    if (animData.ValueRW.CurrentFrame >= clip.FrameCount)
                    {
                        if (animData.ValueRO.Loop)
                        {
                            animData.ValueRW.CurrentFrame = 0;
                        }
                        else
                        {
                            animData.ValueRW.CurrentFrame = clip.FrameCount - 1;
                            animData.ValueRW.IsPlaying = false;
                        }
                    }
                    
                    // Update sprite UV index
                    spriteData.ValueRW.UVIndex = clip.GetFrameIndex(animData.ValueRO.CurrentFrame);
                }
            }
        }
        
        // Simple animation without clips (just AnimationData)
        foreach (var (animData, spriteData) in 
                 SystemAPI.Query<RefRW<AnimationData>, RefRW<SpriteData>>()
                          .WithNone<AnimationClipBuffer>())
        {
            if (!animData.ValueRO.IsPlaying)
                continue;
                
            animData.ValueRW.FrameTimer += deltaTime;
            
            float frameDuration = 1f / animData.ValueRO.FrameRate;
            if (animData.ValueRW.FrameTimer >= frameDuration)
            {
                animData.ValueRW.FrameTimer -= frameDuration;
                animData.ValueRW.CurrentFrame++;
                
                // Simple looping for sprites without animation clips
                if (animData.ValueRW.CurrentFrame >= 16) // Assuming 4x4 atlas
                {
                    if (animData.ValueRO.Loop)
                    {
                        animData.ValueRW.CurrentFrame = 0;
                    }
                    else
                    {
                        animData.ValueRW.CurrentFrame = 15;
                        animData.ValueRW.IsPlaying = false;
                    }
                }
                
                spriteData.ValueRW.UVIndex = animData.ValueRO.CurrentFrame;
            }
        }
    }
}