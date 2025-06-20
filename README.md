# Unity ECS Sprite Rendering System

A high-performance Unity ECS project that renders **up to 1 million sprites** using GPU instancing with advanced sorting, animation, and visual effects.

![Unity Version](https://img.shields.io/badge/Unity-6000.2.0+-blue)
![ECS Version](https://img.shields.io/badge/Entities-1.3.14+-green)
![License](https://img.shields.io/badge/License-Educational%20%26%20Commercial-brightgreen)

## 🚀 Key Features

| Feature | Description |
|---------|-------------|
| **🎮 Million Sprites** | Efficiently render up to 1,000,000 sprites simultaneously |
| **⚡ GPU Instancing** | Uses `DrawMeshInstancedIndirect` for optimal performance |
| **📐 Smart Sorting** | Y-based depth sorting with multi-layer system |
| **🎬 Animation** | Frame-based sprite sheet animations |
| **✨ Visual Effects** | Hit flash, outlines, and color tinting |
| **🛠️ Debug Tools** | Real-time entity counter and sorting visualization |

## 📋 Requirements

- **Unity 6000.2.0+** (Unity 6)
- **Entities 1.3.14+**
- **Universal Render Pipeline**

## 🏗️ System Architecture

### Core Components
```csharp
SpriteData          // Main sprite data (position, color, UV)
SpriteEffectData    // Visual effects (flash, outline, tint)
AnimationData       // Animation state and timing
SortingLayer        // Rendering layer (-1000 to 1000)
OrderInLayer        // Fine-grained sorting control
```

### System Execution Order
1. `InitializeSortingSystem` - Creates sorting singleton
2. `SpriteTransformSyncSystem` - Syncs transform data
3. `SpriteAnimationSystem` - Handles animations
4. `SpriteSortingSystem` - Calculates depth sorting
5. `SpriteSystem` - GPU instanced rendering

## 🎨 Sorting Layers

| Layer | Value | Usage |
|-------|-------|-------|
| Background | -1000 | Sky, backgrounds |
| Floor | -500 | Ground tiles |
| Default | 0 | Standard objects |
| Character | 100 | Players, NPCs |
| Effects | 500 | Particles, overlays |
| UI | 1000 | Interface elements |

## 🔧 Quick Setup

### 1. Sprite Atlas
- Create a **4x4 sprite atlas** (16 sprites)
- Size: 1024x1024 or 512x512
- Place in `Assets/Sprites/`
- Assign to `Third.mat` material

### 2. Basic Sprite
```csharp
// Add SpriteDataAuthoring component to GameObject
Scale: 0.3          // Size multiplier
Color: (1,1,1,1)    // White = no tint
UVIndex: 0-15       // Atlas position
SortingLayer: 0     // Render layer
```

### 3. Animation
```csharp
// Add AnimationAuthoring component
FrameRate: 12       // FPS
StartFrame: 0       // First frame in atlas
FrameCount: 4       // Animation length
Loop: true          // Repeat animation
```

## 💡 Usage Examples

### Creating Sprites at Runtime
```csharp
var entity = entityManager.CreateEntity();
entityManager.AddComponentData(entity, new SpriteData 
{
    Scale = 0.5f,
    Color = new float4(1, 0, 0, 1), // Red tint
    UVIndex = 5
});
```

### Animation Control
```csharp
// Start animation
entityManager.SetComponentData(entity, new AnimationData 
{
    FrameRate = 8f,
    IsPlaying = true,
    Loop = true
});
```

### Layer Management
```csharp
// Move sprite to character layer
entityManager.SetComponentData(entity, new SortingLayer 
{ 
    Value = SortingLayer.Character 
});
```

## 🔍 Debug Features

- **F1 Key**: Toggle sorting debug info
- **Entity Counter**: Real-time sprite count display
- **Sorting Visualization**: See layer assignments and sort keys

## ⚡ Performance

- **Draw Calls**: Minimized through GPU instancing
- **CPU**: Burst-compiled systems with job parallelization
- **Memory**: Efficient data layout, minimal GC
- **Sorting**: 64-bit keys for single-pass sorting

## 🎮 Atlas Layout

The system expects sprites arranged in a 4x4 grid:

```
[ 0][ 1][ 2][ 3]
[ 4][ 5][ 6][ 7]
[ 8][ 9][10][11]
[12][13][14][15]
```

Set `UVIndex` to the corresponding number for each sprite.

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── Components/     # ECS components
│   ├── Systems/        # ECS systems
│   ├── Authoring/      # GameObject→Entity conversion
│   └── Editor/         # Custom inspectors
├── Resources/
│   └── Third.mat       # Main sprite material
└── Sprites/            # Sprite atlas textures
```

## 🙏 Credits

**Original Concept**: [Marnel Estrada](https://github.com/marnel-estrada) - "Sorting a Million Sprites"  
**Extended Implementation**: Enhanced with sorting, animation, and effects systems

## 📄 License

Available for educational and commercial use. See project files for implementation details.

---

⭐ **Star this repo** if you find it useful for your Unity ECS projects!
