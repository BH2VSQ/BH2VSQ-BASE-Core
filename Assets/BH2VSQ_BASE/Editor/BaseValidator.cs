using System.Collections.Generic;
using System.Text;
using BH2VSQ.Base;
using UnityEditor;
using UnityEngine;

namespace BH2VSQ.Base.Editor
{
    public static class BaseValidator
    {
        [MenuItem("BH2VSQ BASE/Validate Setup")]
        public static void ValidateMenu() { ValidateSelection(true); }

        public static bool ValidateSelection(bool showDialog)
        {
            GameObject root = Selection.activeGameObject;
            BaseWorldSystem selected = root == null ? null : root.GetComponentInParent<BaseWorldSystem>();
            if (selected != null) root = selected.gameObject;
            if (root == null || root.GetComponentInChildren<BaseWorldSystem>(true) == null)
                root = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/BH2VSQ_BASE/Prefabs/Core/BH2VSQ_BASE_Core.prefab");
            List<string> errors = new List<string>();
            List<string> warnings = new List<string>();
            if (root == null) errors.Add("Core prefab missing. Run Build Package.");
            else Validate(root, errors, warnings);
            StringBuilder result = new StringBuilder();
            result.AppendLine(errors.Count == 0 ? "Configuration valid for static references." : "Configuration invalid:");
            foreach (string error in errors) result.AppendLine("ERROR: " + error);
            foreach (string warning in warnings) result.AppendLine("WARN: " + warning);
            result.AppendLine("Network behaviour still requires a multi-client VRChat test.");
            if (errors.Count > 0) Debug.LogError(result.ToString()); else Debug.Log(result.ToString());
            if (showDialog) EditorUtility.DisplayDialog("BH2VSQ BASE Validator", result.ToString(), "OK");
            return errors.Count == 0;
        }

        private static void Validate(GameObject root, List<string> errors, List<string> warnings)
        {
            BaseWorldSystem world = root.GetComponentInChildren<BaseWorldSystem>(true);
            FloorManager floors = root.GetComponentInChildren<FloorManager>(true);
            AreaManager areas = root.GetComponentInChildren<AreaManager>(true);
            TOTPAuthManager auth = root.GetComponentInChildren<TOTPAuthManager>(true);
            PermissionManager permission = root.GetComponentInChildren<PermissionManager>(true);
            TeleportManager teleport = root.GetComponentInChildren<TeleportManager>(true);
            BroadcastManager broadcast = root.GetComponentInChildren<BroadcastManager>(true);
            TabMenuController menu = root.GetComponentInChildren<TabMenuController>(true);
            BroadcastNotificationManager notification = root.GetComponentInChildren<BroadcastNotificationManager>(true);
            if (world == null || world.registry == null || world.playerData == null || world.session == null || world.access == null) errors.Add("Core reference missing.");
            if (floors == null || floors.floorNames == null || floors.requiredRanks == null || floors.xpMultipliers == null || floors.states == null || floors.floorNames.Length != 10 || floors.requiredRanks.Length != 10 || floors.xpMultipliers.Length != 10 || floors.states.Length != 10) errors.Add("Floor database must have 10 aligned entries.");
            bool validAreas = areas != null && areas.ids != null && areas.names != null && areas.floorIds != null && areas.requiredRanks != null && areas.xpMultipliers != null && areas.ids.Length == areas.names.Length && areas.ids.Length == areas.floorIds.Length && areas.ids.Length == areas.requiredRanks.Length && areas.ids.Length == areas.xpMultipliers.Length;
            if (!validAreas) errors.Add("Area database arrays have different lengths.");
            if (validAreas)
            {
                HashSet<int> ids = new HashSet<int>();
                for (int i = 0; i < areas.ids.Length; i++)
                {
                    if (!ids.Add(areas.ids[i])) errors.Add("Duplicate area ID: " + areas.ids[i]);
                    if (floors == null || !floors.Valid(areas.floorIds[i])) errors.Add("Invalid floor for area " + areas.ids[i]);
                    if (areas.requiredRanks[i] < 0 || areas.requiredRanks[i] > 2) errors.Add("Invalid rank for area " + areas.ids[i]);
                    if (areas.xpMultipliers[i] <= 0) errors.Add("Invalid XP multiplier for area " + areas.ids[i]);
                }
            }
            if (floors != null && floors.requiredRanks != null && floors.xpMultipliers != null)
                for (int i = 0; i < floors.requiredRanks.Length && i < floors.xpMultipliers.Length; i++)
                {
                    if (floors.requiredRanks[i] < 0 || floors.requiredRanks[i] > 2) errors.Add("Invalid rank for floor " + i);
                    if (floors.xpMultipliers[i] <= 0) errors.Add("Invalid XP multiplier for floor " + i);
                }
            if (permission == null || permission.session == null || auth == null || auth.session == null) errors.Add("Authentication references missing.");
            if (auth != null)
            {
                SerializedObject serialized = new SerializedObject(auth);
                string member = serialized.FindProperty("memberTotpSecret").stringValue;
                string admin = serialized.FindProperty("adminTotpSecret").stringValue;
                if (string.IsNullOrWhiteSpace(member) || string.IsNullOrWhiteSpace(admin)) warnings.Add("TOTP secrets are blank. Configure on scene instance, keep out of public Git.");
            }
            if (teleport == null || teleport.access == null || teleport.points == null || teleport.points.Length == 0) errors.Add("Teleport manager or points missing.");
            else
            {
                HashSet<int> pointIds = new HashSet<int>();
                foreach (TeleportPoint point in teleport.points)
                {
                    if (point == null) { errors.Add("Null teleport point."); continue; }
                    if (!pointIds.Add(point.pointId)) errors.Add("Duplicate point ID: " + point.pointId);
                    if (floors == null || !floors.Valid(point.floorId)) errors.Add("Invalid point floor: " + point.name);
                    if (!validAreas || areas.IndexOf(point.areaId) < 0) errors.Add("Invalid point area: " + point.name);
                }
                if (validAreas)
                    foreach (int id in areas.ids)
                    {
                        bool found = false;
                        foreach (TeleportPoint point in teleport.points) if (point != null && point.areaId == id) found = true;
                        if (!found) errors.Add("No teleport point for area " + id);
                    }
            }
            if (menu == null || menu.personal == null || menu.teleport == null || menu.players == null || menu.admin == null) errors.Add("Tab UI incomplete.");
            if (broadcast == null || broadcast.admin == null || broadcast.notification == null || notification == null || notification.root == null) errors.Add("Broadcast references missing.");
            if (root.GetComponentInChildren<TeleportRequestManager>(true) == null || root.GetComponentInChildren<RequestPanel>(true) == null) errors.Add("Request UI or manager missing.");
            if (root.GetComponentInChildren<LocalCanvasFollower>(true) == null) errors.Add("Local UI follower missing.");
        }
    }
}
