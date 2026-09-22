# Prefab inventory

The **BH2VSQ BASE → Build Package** editor command creates these assets after Unity imports and compiles the project. The source checkout contains the generator; generated prefabs are not available until that command runs successfully.

## Complete installation entry

`Prefabs/Core/BH2VSQ_BASE_Core.prefab` contains all runtime managers, the main menu, local notification and request canvases, 14 teleport points, and area triggers. Drag one instance into the world scene. Configure its TOTP secrets on the scene instance and move the points and triggers into the real building. The prefab does not contain a building model, finished room geometry, or a secure server.

`Prefabs/Demo/BH2VSQ_DemoBase.prefab` is a copy of the generated core for demonstrations. `Scenes/BH2VSQ_BASE_Demo.unity` adds a ground plane and a sample VRChat world prefab when that SDK sample is available. Its points sit in a straight line as placeholders.

## Reusable parts

| Folder | Generated prefabs |
| --- | --- |
| `Prefabs/UI` | `BH2VSQ_TabMenu`, `BH2VSQ_PersonalInfo`, `BH2VSQ_PermissionLogin`, `BH2VSQ_Teleport`, `BH2VSQ_PlayerList`, `BH2VSQ_PlayerListItem`, `BH2VSQ_PlayerDetail`, `BH2VSQ_TeleportRequest`, `BH2VSQ_AdminPanel`, `BH2VSQ_FloorAdmin`, `BH2VSQ_BroadcastPanel`, `BH2VSQ_BroadcastNotificationManager`, `BH2VSQ_BroadcastNotification` |
| `Prefabs/Floor` | `BH2VSQ_FloorController`, `BH2VSQ_AreaController`, `BH2VSQ_AreaTrigger` |
| `Prefabs/Teleport` | `BH2VSQ_TeleportPoint` |

These part prefabs are layout and component templates. The generator removes references to objects outside each extracted part. Add them to an existing core instance and wire their manager fields in the Inspector before using them alone. The complete core prefab is the supported drag and drop entry.

## Regeneration and edits

The generator replaces its prefab and demo scene outputs. Keep world-specific point placement, trigger dimensions, and secrets on a separate scene instance. Keep changes to reusable defaults in `Data/Default`, `Data/Localization`, and the generator. Run **BH2VSQ BASE → Validate Setup** on the selected scene instance after wiring or moving components.
