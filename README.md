# Operation Rust

Operation Rust is a 2D top-down stealth game developed in Unity.

## Overview

The player infiltrates a heavily guarded military outpost to complete a multi-stage mission while avoiding enemy detection. Guards patrol the environment using AI-driven behavior and vision-based detection systems.

The mission consists of:

1. Disabling the radar dish.
2. Accessing the VIP building and downloading intelligence.
3. Escaping through the helipad extraction zone.

Detection by enemy guards results in mission failure.

---

## Features

* Top-down stealth gameplay
* AI-driven guard behavior
* A* pathfinding system
* Finite State Machine enemy logic
* Dynamic patrol routes
* Vision cone detection
* Obstacle-aware line-of-sight checks
* Mission objective system
* Main Menu
* Victory Screen
* Game Over Screen
* Background audio integration

---

## Controls

| Action     | Key |
| ---------- | --- |
| Move Up    | W   |
| Move Down  | S   |
| Move Left  | A   |
| Move Right | D   |
| Interact   | E   |

---

## AI System

Enemy guards use:

* Patrol State
* Suspicious State
* Chase State
* Search State
* Return State

Navigation is handled using a custom A* pathfinding implementation built on a node-based grid system.

---

## Project Structure

```text
Assets
├── Scripts
├── Prefabs
├── Sprites
├── Audio
├── Materials
└── Scenes
```

---

## Scenes

1. Main Menu
2. Mission Scene

---

## Technologies Used

* Unity 6
* C#
* Visual Studio
* GitHub
* OBS Studio
* AI-assisted development tools (ChatGPT and Claude)

---

## Build

The playable Windows build is included in the Build folder.

Launch:

OperationRust.exe

---

## Author

Developed as part of a Game Development Internship Assignment.
