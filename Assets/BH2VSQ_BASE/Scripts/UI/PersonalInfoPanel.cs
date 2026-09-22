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
        public LocalizationManager localization;

        private void Start() { SendCustomEventDelayedSeconds("Refresh", 1f); }
        public void Refresh()
        {
            if (output != null && Utilities.IsValid(Networking.LocalPlayer))
            {
                string floor = floors != null && floors.Valid(tracker.localFloorId) ? floors.GetFloor(tracker.localFloorId, localization.Language()) : localization.Get(BaseText.Unknown);
                int areaIndex = areas != null ? areas.IndexOf(tracker.localAreaId) : -1;
                string area = areaIndex >= 0 ? areas.DisplayName(areaIndex, localization.Language()) : localization.Get(BaseText.Unknown);
                output.text = Networking.LocalPlayer.displayName + "\n" + localization.RankName(permission.GetRank()) + "  " + localization.Get(BaseText.Level) + data.Level() +
                    "\n" + localization.Get(BaseText.Xp) + ": " + data.experience + "\n" + localization.Get(BaseText.Time) + ": " + (data.totalSeconds / 3600) + (localization.Language() == 1 ? "小时" : "h") + "\n" + floor + " / " + area +
                    "\n" + localization.Get(BaseText.TeleportConfirmation) + ": " + localization.Get(data.teleportConfirm ? BaseText.On : BaseText.Off) +
                    "\n" + localization.Get(BaseText.Notifications) + ": " + localization.Get(data.notificationsEnabled ? BaseText.On : BaseText.Off);
            }
            SendCustomEventDelayedSeconds("Refresh", 5f);
        }
    }
}
