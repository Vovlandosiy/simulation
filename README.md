# 2D Battle Simulation Tool (Unity 6 / URP)

A high-impact 2D physical battle simulation tool developed with Unity 6. The project is specifically designed to simulate automated AI battles and generate vertical content (YouTube Shorts, TikTok) with vibrant neon aesthetics and fast-paced gameplay.

## Features

*   **Autonomous AI Combat:** Units search for targets, bounce off walls, use melee attacks with internal cooldowns, and dynamically equip weapons scattered across the arena.
*   **Data-Driven Design:** Unit stats and weapon configurations are managed via `ScriptableObjects`, making it easy to tweak balance and add new items without modifying the code.
*   **Dynamic Pooling UI:** A heart-based health tracking system that scales from 2 to 4 teams. It utilizes object pooling for high performance, preventing runtime memory allocation during chaotic battles.
*   **Neon Visuals & 2D Lights:** Built on Universal Render Pipeline (URP) utilizing global Post-Processing (Bloom effect) and custom HDR shaders for glowing arena walls and projectiles.
*   **Immersive VFX:** Procedural weapon spawn rings, trailing rockets, and scalable impact particle systems.

## Getting Started

### Prerequisites
*   **Unity Editor:** Unity 6 (6000.2.x or newer)
*   **Render Pipeline:** Universal Render Pipeline (URP 2D)
*   **Packages used:** New Input System, Unity 2D stack

### Installation
1.  Clone this repository to your local machine:
    ```bash
    git clone https://github.com/Vovlandosiy/simulation
    ```
2.  Open **Unity Hub**, click **Add** -> **Add project from disk**, and select the project folder.
3.  Ensure the editor version is set to Unity 6 and open the project.
4.  Navigate to `Assets/Project/Scenes/` and launch `FirstScene`.
