using UdonSharp;
using TMPro;
using VRC.SDKBase;
using VRC.SDK3.Persistence;

namespace BH2VSQ.Base
{
    public class PlayerListPanel : UdonSharpBehaviour
    {
        public PlayerRegistry registry;
        public PlayerAreaTracker tracker;
        public PlayerDataManager data;
        public RadioDutyManager radio;
        public FloorManager floors;
        public AreaManager areas;
        public TMP_Text output;
        public PlayerDetailPanel detail;
        public TMP_Text[] rowTexts;
        public UIButtonAction[] rowActions;
        public UnityEngine.GameObject[] rowObjects;
        public LocalizationManager localization;

        public void Refresh()
        {
            if (registry == null || output == null) return;
            registry.Refresh();
            output.text = localization.Get(BaseText.Online) + ": " + registry.count + "  /  " + localization.Get(BaseText.Radio) + ": " + localization.Get(radio != null && radio.OnDuty() ? BaseText.OnDuty : BaseText.NoDuty);
            if (rowTexts == null || rowActions == null || rowObjects == null) return;
            for (int i = 0; i < rowObjects.Length; i++)
            {
                bool visible = i < registry.count && Utilities.IsValid(registry.players[i]);
                rowObjects[i].SetActive(visible);
                if (!visible) continue;
                VRCPlayerApi player = registry.players[i];
                int areaId = tracker == null ? -1 : tracker.AreaForPlayer(player.playerId);
                int areaIndex = areas == null ? -1 : areas.IndexOf(areaId);
                string areaName = areaIndex < 0 ? localization.Get(BaseText.Unknown) : areas.DisplayName(areaIndex, localization.Language());
                int floorId = areaIndex < 0 ? BaseConstants.InvalidId : areas.FloorIdAt(areaIndex);
                string floorName = floors != null && floors.Valid(floorId) ? floors.GetFloor(floorId, localization.Language()) : localization.Get(BaseText.Unknown);
                int xp = PlayerData.GetInt(player, "bh2vsq.xp");
                int level = 1 + (int)UnityEngine.Mathf.Sqrt(xp / 100f);
                rowTexts[i].text = player.displayName + " | " + localization.RankName(registry.RankForPlayer(player.playerId)) + " | " + localization.Get(BaseText.Level) + level + "\n" + floorName + " / " + areaName + (areas != null && areas.IsRadioLocation(areaId) ? " | " + localization.Get(BaseText.OnDuty) : "");
                rowActions[i].value = player.playerId;
            }
        }
    }
}
