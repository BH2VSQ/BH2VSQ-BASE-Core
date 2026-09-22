using UdonSharp;
using TMPro;
using VRC.SDKBase;
using VRC.SDK3.Persistence;

namespace BH2VSQ.Base
{
    public class PlayerDetailPanel : UdonSharpBehaviour
    {
        public TMP_Text output;
        public PlayerAreaTracker tracker;
        public PlayerRegistry registry;
        public FloorManager floors;
        public AreaManager areas;
        public LocalizationManager localization;
        public int selectedPlayerId;
        public void ShowPlayer(int playerId)
        {
            selectedPlayerId = playerId;
            VRCPlayerApi player = VRCPlayerApi.GetPlayerById(playerId);
            if (output == null) return;
            if (!Utilities.IsValid(player)) { output.text = localization.Get(BaseText.Offline); return; }
            int areaId = tracker.AreaForPlayer(playerId);
            int areaIndex = areas.IndexOf(areaId);
            string area = areaIndex < 0 ? localization.Get(BaseText.Unknown) : areas.DisplayName(areaIndex, localization.Language());
            int floorId = areaIndex < 0 ? BaseConstants.InvalidId : areas.FloorIdAt(areaIndex);
            string floor = floors.Valid(floorId) ? floors.GetFloor(floorId, localization.Language()) : localization.Get(BaseText.Unknown);
            int xp = PlayerData.GetInt(player, "bh2vsq.xp");
            int level = 1 + (int)UnityEngine.Mathf.Sqrt(xp / 100f);
            output.text = player.displayName + "\n" + localization.RankName(registry.RankForPlayer(playerId)) + "  " + localization.Get(BaseText.Level) + level +
                "\n" + floor + " / " + area + "\n" + localization.Get(BaseText.Online) + (areas != null && areas.IsRadioLocation(areaId) ? "  " + localization.Get(BaseText.RadioDuty) : "");
        }
    }
}
