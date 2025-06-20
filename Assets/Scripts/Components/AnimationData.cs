using Unity.Entities;

public struct AnimationData : IComponentData
{
    public int CurrentFrame;
    public float FrameTimer;
    public float FrameRate; // Frames per second
    public int AnimationID; // Which animation to play
    public bool IsPlaying;
    public bool Loop;
}