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
        public int selectedPlayerId;
        public void ShowPlayer(int playerId)
        {
            selectedPlayerId = playerId;
            VRCPlayerApi player = VRCPlayerApi.GetPlayerById(playerId);
            if (output == null) return;
            if (!Utilities.IsValid(player)) { output.text = "Offline"; return; }
            int areaId = tracker.AreaForPlayer(playerId);
            int areaIndex = areas.IndexOf(areaId);
            string area = areaIndex < 0 ? "Unknown" : areas.names[areaIndex];
            int floorId = areaIndex < 0 ? -1 : areas.floorIds[areaIndex];
            string floor = floors.Valid(floorId) ? floors.floorNames[floorId] : "Unknown";
            int xp = PlayerData.GetInt(player, "bh2vsq.xp");
            int level = 1 + (int)UnityEngine.Mathf.Sqrt(xp / 100f);
            output.text = player.displayName + "\n" + registry.RankForPlayer(playerId) + "  Lv." + level +
                "\n" + floor + " / " + area + "\nOnline" + (areaId == BaseConstants.RadioAreaId ? "  Radio Duty" : "");
        }
    }
}
