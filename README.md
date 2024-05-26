# Unity ECS Rendering Project

This Unity project implements an Entity Component System (ECS) to render a million sprites, based on the system developed by Marnel Estrada. The goal is to efficiently manage and render a large number of sprites in Unity using the ECS architecture.

## Features
- **Sorting of One Million Sprites:** Efficiently renders a large number of sprites.
- **Optimized Performance:** Utilizes Unity's ECS to maximize performance and minimize overhead.

## Getting Started

### Prerequisites
- **Unity Version:** Ensure you have Unity 2022.3 or higher installed.
- Entities 1.2

Components:

    SpriteData: Holds data related to the sprite, such as position and sorting order.

System:

    SpriteSystem: Manages the buffer setup and updates for sprite data, and performs the actual rendering.

Credits:
    
    Marnel Estrada: Original system inspiration. Sorting a million sprites
