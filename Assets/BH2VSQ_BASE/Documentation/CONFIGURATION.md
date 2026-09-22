# Configuration and Inspector guide

## Authoring assets

`Data/Default/DefaultFloorDatabase.asset` is the source for the ten fixed floor indexes: B3=0, B2=1, B1=2, 1F=3, 2F=4, 3F=5, 4F=6, 5F=7, 6F=8, 7F=9. It sets names, minimum rank and XP multiplier. Default multipliers are 5, 3, 1, 0.5, 1.5, 1, 1, 1, 2, 3.

`DefaultAreaDatabase.asset` sets area IDs, names, parent floor, minimum rank and an extra XP multiplier. IDs are 1000; 2000-2003; 3000; 4000-9000; 10000 (7F Radio); 10001 (Rooftop). The Rooftop requires Admin. Area multipliers default to 1. `DefaultBaseConfig.asset` links the two databases and sets teleport request timeout and broadcast durations. Regenerating copies these values into serializable Udon fields; ScriptableObjects are editor authoring data, not Udon runtime data.

`Data/Localization/DefaultLocalization.asset` contains parallel English and Chinese UI label arrays. The first entries map to Personal, Teleport, Players, Admin, Login, Send, Close, Accept, Reject, Refresh, Broadcast and Floor. A generated dynamic Noto Sans SC TextMesh Pro font renders Chinese. Persistent language is 0=English, 1=Chinese. Some dynamic status text remains English in this version.

## Core scene instance

- `Authentication/TOTPAuthManager`: configure two different Base32 secrets. Blank values disable that rank. Codes are six digit HMAC-SHA1 TOTP with a 30 second period and ±1 period tolerance, checked against `Networking.GetNetworkDateTime()`.
- `Floor/FloorManager`: review `floorNames`, `requiredRanks`, `xpMultipliers`, and initial `states`. Admin changes to state are manually synchronized. Open=0, Reserved=1, Maintenance=2; only Admin bypasses reserved/maintenance.
- `Floor/AreaManager`: review each area ID, floor ID, rank and XP modifier. Keep the arrays aligned and IDs unique.
- `Teleport/Point_*`: edit point ID, floor ID, area ID, name and transform. The point transform is the teleport destination. `AreaTrigger` child has a trigger collider, area ID, tracker, access and teleport references. Resize it to fit the physical entrance/area. Unauthorized entrants return to 1F or respawn if 1F is unavailable.
- `Teleport/TeleportRequestManager`: `timeoutSeconds` defaults to 15. The single shared request slot serializes requester ID, target ID, type, state and expiry. Type 0 moves requester to target after acceptance; type 1 moves target to requester.
- `Admin/BroadcastManager`: `normalSeconds`=8, `importantSeconds`=20, `emergencySeconds`=120. The eight-slot synchronized ring carries ID, priority, message, sender, timestamp and expiry. A notification queue on each client displays messages one at a time; emergency stays until dismissed by that client. Normal broadcasts sent before a player's join are skipped.
- `UI/MainCanvas/TabMenu/AdminPanel`: Admin opens the shared player roster from **Player Management**, sees population and radio duty, changes floor state, and sends broadcasts. The roster is read-only; VRChat player moderation is outside this Udon plugin.
- `UI/NotificationLayer`: each client follows their own head; it is not a fixed room screen. `BroadcastNotificationManager` holds local notification state and three tone clips. The main menu uses a separate world-space canvas.

## Player data and runtime state

`PlayerDataManager` waits for `OnPlayerRestored` before reading or writing XP, total seconds, language, teleport confirmation, notification enablement and volume. The 30-second XP tick awards floor multiplier times area multiplier, storing fractional XP locally until whole points accrue. Rank is a local authentication session and resets to Visitor on each join. The synchronized registry publishes a display rank for the player list; public ranks and XP are not trusted authority.

`PlayerRegistry`, `PlayerAreaTracker`, `FloorManager`, `TeleportRequestManager`, and `BroadcastManager` use manual sync with ownership transfer and `RequestSerialization()`. Shared player rank/area arrays and the one-slot teleport request can race if multiple clients write nearly simultaneously. Verify with multiple VRChat clients and consider player-owned objects for high contention worlds. See `SECURITY.md` for authority limits.
