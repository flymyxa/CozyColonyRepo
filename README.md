# CozyColony

This repository contains a small Unity prototype located in the `CozyColony` folder.

## Gameplay Loop

1. **Build** – The UI spawns buttons from `BuildDef` assets and allows you to place structures in the world using a placement ghost.
2. **Harvest** – Right-clicking a `ResourceNode` queues a harvest job for colonists, who travel to the node and collect resources.
3. **Cook** – Recipes defined by `RecipeDef` can convert resources into food at workbenches (logic placeholder in `ColonistAgent`).
4. **Eat** – Colonists satisfy hunger via the `NeedsComponent.Eat` method when an `Eat` job is executed.

## Project Structure

- `CozyColony/Assets` – All project assets including art, prefabs, scripts and definitions.
- `CozyColony/Packages` – Unity package manifest and dependencies.
- `CozyColony/ProjectSettings` – Unity project configuration files.

## Opening the Project

1. Install **Unity 6000.1.10f1** (version listed in `ProjectSettings/ProjectVersion.txt`).
2. Using Unity Hub, choose **Open** and select the `CozyColony` directory.
3. When prompted, import TextMeshPro essentials – the project uses TextMeshPro for UI (`Assets/TextMesh Pro`).

The main scene is located under `Assets/Scenes/SampleScene.unity`.

