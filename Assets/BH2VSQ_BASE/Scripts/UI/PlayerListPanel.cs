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

        public void Refresh()
        {
            if (registry == null || output == null) return;
            registry.Refresh();
            output.text = "Online: " + registry.count + "  /  Radio: " + (radio != null && radio.OnDuty() ? "On Duty" : "No Duty");
            if (rowTexts == null || rowActions == null || rowObjects == null) return;
            for (int i = 0; i < rowObjects.Length; i++)
            {
                bool visible = i < registry.count && Utilities.IsValid(registry.players[i]);
                rowObjects[i].SetActive(visible);
                if (!visible) continue;
                VRCPlayerApi player = registry.players[i];
                int areaId = tracker == null ? -1 : tracker.AreaForPlayer(player.playerId);
                int areaIndex = areas == null ? -1 : areas.IndexOf(areaId);
                string areaName = areaIndex < 0 ? "?" : areas.names[areaIndex];
                int floorId = areaIndex < 0 ? -1 : areas.floorIds[areaIndex];
                string floorName = floors != null && floors.Valid(floorId) ? floors.floorNames[floorId] : "?";
                int xp = PlayerData.GetInt(player, "bh2vsq.xp");
                int level = 1 + (int)UnityEngine.Mathf.Sqrt(xp / 100f);
                rowTexts[i].text = player.displayName + " | " + registry.RankForPlayer(player.playerId) + " | Lv." + level + "\n" + floorName + " / " + areaName + (areaId == BaseConstants.RadioAreaId ? " | On Duty" : "");
                rowActions[i].value = player.playerId;
            }
        }
    }
}
