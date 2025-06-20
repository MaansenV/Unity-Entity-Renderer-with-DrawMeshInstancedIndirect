# GPU-Instanced Sprite Rendering System Setup

## Overview
This system provides efficient GPU-instanced rendering for thousands of 2D sprites using Unity ECS/DOTS.

## Setup Instructions

### 1. Create or Import Sprite Atlas
The material "Third.mat" requires a 4x4 sprite atlas texture (16 sprites total).

**Missing Texture GUID**: `b9b394a6684e48647b979e89500c6fee`

To fix this:
1. Create a 4x4 sprite atlas texture (recommended size: 1024x1024 or 512x512)
2. Import it into `Assets/Sprites/` folder
3. Select the Third.mat material in `Assets/Resources/`
4. Assign your sprite atlas to the "_MainTex" property

### 2. Create Entity References GameObject
1. Create an empty GameObject named "EntityReferences"
2. Add the `EntitiesReferenceAuthoring` component
3. This will hold references to all entity prefabs

### 3. Create Bullet Prefab
1. Create a GameObject named "BulletPrefab"
2. Add the `SpriteDataAuthoring` component
3. Configure:
   - Scale: 0.3 (default)
   - Color: White (1,1,1,1)
   - UVIndex: 0-15 (sprite index in atlas)
4. Save as prefab in `Assets/Prefabs/`
5. Assign to EntityReferences' bulletPrefab field

### 4. System Configuration
The system automatically:
- Spawns 10 sprites per frame (configurable in SpawnSystem)
- Supports up to 1 million sprites
- Uses a 4x4 UV grid (16 different sprites)
- Renders using GPU instancing

### 5. Performance Notes
- All systems use Burst compilation
- GPU instancing via DrawMeshInstancedIndirect
- Minimal CPU overhead with job system
- Adjust spawn rate in SpawnSystem.cs as needed

## Sprite Atlas UV Layout
The system expects a 4x4 grid layout:
```
[0] [1] [2] [3]
[4] [5] [6] [7]
[8] [9] [10][11]
[12][13][14][15]
```

Each cell should contain a different sprite. The UVIndex in SpriteData corresponds to these positions.