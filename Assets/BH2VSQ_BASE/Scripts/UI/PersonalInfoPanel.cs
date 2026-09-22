using UdonSharp;
using TMPro;
using VRC.SDKBase;

namespace BH2VSQ.Base
{
    public class PersonalInfoPanel : UdonSharpBehaviour
    {
        public TMP_Text output;
        public PlayerDataManager data;
        public PermissionManager permission;
        public PlayerAreaTracker tracker;
        public FloorManager floors;
        public AreaManager areas;

        private void Start() { SendCustomEventDelayedSeconds("Refresh", 1f); }
        public void Refresh()
        {
            if (output != null && Utilities.IsValid(Networking.LocalPlayer))
            {
                string floor = floors != null && floors.Valid(tracker.localFloorId) ? floors.floorNames[tracker.localFloorId] : "Unknown";
                int areaIndex = areas != null ? areas.IndexOf(tracker.localAreaId) : -1;
                string area = areaIndex >= 0 ? areas.names[areaIndex] : "Unknown";
                output.text = Networking.LocalPlayer.displayName + "\n" + permission.GetRank() + "  Lv." + data.Level() +
                    "\nXP: " + data.experience + "\nTime: " + (data.totalSeconds / 3600) + "h\n" + floor + " / " + area +
                    "\nTeleport confirmation: " + (data.teleportConfirm ? "On" : "Off") + "\nNotifications: " + (data.notificationsEnabled ? "On" : "Off");
            }
            SendCustomEventDelayedSeconds("Refresh", 5f);
        }
    }
}
