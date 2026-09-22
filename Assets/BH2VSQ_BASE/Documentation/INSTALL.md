# Installation

## Prerequisites

- Unity Editor 2022.3.22f1 with an active license.
- A VRChat Creator Companion Worlds project with VRChat Worlds SDK 3.10.4 and TextMesh Pro 3.0.6.
- Windows PowerShell for the repository build scripts. A Unity Editor menu provides the same build entry on other platforms.

## Build from this repository

Run `tools/Bootstrap-VRChat.ps1` from the repository root. It downloads the pinned VRChat Base and Worlds packages from the official release, verifies their SHA-256 hashes, and extracts them under `Packages/`. These local package folders are ignored by Git. Open the project in Unity and select **BH2VSQ BASE → Build Package**. This creates UdonSharp program assets, default data, a dynamic TextMesh Pro font, 14 teleport points, UI and component prefabs, and `Assets/BH2VSQ_BASE/Scenes/BH2VSQ_BASE_Demo.unity`.

Select **BH2VSQ BASE → Export Unitypackage** or run `tools/Build-Package.ps1` to create `Releases/BH2VSQ_BASE_Core.unitypackage` after a successful compile and static validator pass.

## Install into a world

1. In a VRChat Worlds project, import the exported `.unitypackage`.
2. Drag `Assets/BH2VSQ_BASE/Prefabs/Core/BH2VSQ_BASE_Core.prefab` into the scene. The core prefab contains all managers, UI, notifications, floor data and default points. It does not include a building model.
3. Move every `Teleport/Point_*` object to its destination. Configure the 3D area trigger positions and sizes to match the actual building. The demo positions are placeholders in a straight line.
4. Configure member/admin TOTP Base32 secrets on the scene instance's `Authentication/TOTPAuthManager`. Never commit scene files containing real secrets to a public repository.
5. Set the world spawn at a public safe location such as 1F. The demo scene uses the SDK's VRCWorld sample prefab when available.
6. Select the scene instance and run **BH2VSQ BASE → Validate Setup**. Resolve errors and inspect warnings.
7. Test locally in ClientSim, then use VRChat Build & Test with two or more clients. Test visitor, member, admin, late join, consent requests, floor states, and broadcast priority/queue/expiry before upload.

## Regeneration

`Build Package` overwrites generated prefabs and the demo scene. Edit the authoring assets in `Data/Default` and `Data/Localization`, then regenerate. Move and configure a scene instance separately. Save modified scenes before running the wizard; it prompts in interactive Unity.
