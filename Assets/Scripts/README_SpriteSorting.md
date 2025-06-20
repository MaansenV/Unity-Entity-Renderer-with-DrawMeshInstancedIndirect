# Sprite Sorting System

## Overview
The sprite sorting system provides Y-based depth sorting and layer management for the GPU-instanced sprite renderer.

## Features

### 1. Y-Based Sorting
- Sprites with higher Y positions render on top (typical for 2D top-down games)
- Automatic depth sorting based on world position
- Works seamlessly with transform movement

### 2. Layer System
- **Sorting Layers**: Primary sorting control (-1000 to 1000)
  - Background: -1000
  - Floor: -500
  - Default: 0
  - Character: 100
  - Effects: 500
  - UI: 1000
- **Order in Layer**: Fine-grained control within same layer

### 3. Sorting Algorithm
64-bit sorting key composition:
- Bits 48-63: Sorting layer (primary sort)
- Bits 16-47: Inverted Y position (secondary sort)
- Bits 0-15: Order in layer (tertiary sort)

## System Execution Order
1. `InitializeSortingSystem` - Creates singleton
2. `SpriteTransformSyncSystem` - Syncs transform data
3. `AddDefaultSortingComponentsSystem` - Adds default components
4. `SpriteSortingSystem` - Calculates sorting
5. `SpriteSystem` - Renders in sorted order

## Usage

### In Unity Editor
1. Add `SpriteDataAuthoring` to GameObjects
2. Set `Sorting Layer` and `Order in Layer` values
3. Use GameObject menu for quick presets:
   - GameObject → 2D Roguelike → Sorting Layers → [Layer]

### Via Code
```csharp
// Add sorting components to entity
entityManager.AddComponentData(entity, new SortingLayer { Value = 100 });
entityManager.AddComponentData(entity, new OrderInLayer { Value = 0 });
```

### Debug Mode
Press **F1** in Play mode to toggle sorting debug output.
Shows first 10 sprites with position, layer, and sorting key.

## Performance
- Burst-compiled sorting with NativeSortExtension
- Efficient bit-packing for sort keys
- Minimal overhead for thousands of sprites
- GPU instancing preserved

## Tips
- Use layer presets for consistency
- Characters typically use layer 100
- Effects should be on higher layers (500+)
- UI elements use layer 1000
- Adjust OrderInLayer for fine control