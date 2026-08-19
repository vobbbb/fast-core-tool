# FastCoreTool (FCT)

**FastCoreTool (FCT)** is a lightweight core framework for Unity designed to help you start and structure your projects faster.
FCT provides a collection of simple, ready-to-use systems for common game development needs without forcing your project into a complex architecture.

## Features

- **State Machine (FSM)** — Lightweight state-based architecture for managing game logic.
- **Live FSM Debugger** — Monitor the current state directly from the Unity Editor while the game is running.
- **State Generator** — Automatically generate new `IState` implementations from the editor.
- **Object Pooling** — Reusable pooling system for frequently instantiated objects.
- **Input Management** — Centralized input configuration and management.
- **Core Game Manager** — A simple foundation for managing your game's core flow.
- **Core Wizard** — Set up the main FCT systems directly from the Unity Editor.
- **Organized Namespaces** — Clean `Vobb.FCT.*` namespace structure.
- **Assembly Definitions** — Includes `Vobb.FCT.asmdef` for better project organization and compilation.
- **Easy Integration** — Designed for both new and existing Unity projects.

## Getting Started
After importing FCT into your Unity project:

1. Open Unity.
2. Go to **FCT → Core Wizard**.
3. Use the setup wizard to configure the systems you need.
4. Configure your input settings through the generated `FCT_InputConfig` asset.
5. Start building your game.

## Core Wizard
The **Core Wizard** provides a central place for configuring FCT inside the Unity Editor.
It can help you set up your project, create the required configuration assets, initialize core systems, and generate example components.

### State Generator
Create new FSM states directly from the Core Wizard.
Enter your state name and Game Manager type and FCT will automatically generate a C# class implementing the required `IState` structure.

### Live FSM Debugger
When entering Play Mode, the Core Wizard can automatically detect an active `CoreGameManager` and display its current FSM state in real time.
This provides a quick way to inspect state transitions without adding additional debugging code.

## Philosophy
FCT is designed around three principles:

**Simple. Lightweight. Practical.**
It provides the common foundations many Unity projects need while staying out of the way of your game's architecture.
Use the systems you need, ignore the ones you don't.
