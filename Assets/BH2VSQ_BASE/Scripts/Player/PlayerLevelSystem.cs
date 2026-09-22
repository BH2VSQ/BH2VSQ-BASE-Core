using UdonSharp;
using UnityEngine;

namespace BH2VSQ.Base
{
    public class PlayerLevelSystem : UdonSharpBehaviour
    {
        public PlayerDataManager data;
        public PlayerAreaTracker tracker;
        public FloorManager floors;
        public AreaManager areas;
        private float fractionalXp;

        private void Start() { SendCustomEventDelayedSeconds("Tick", BaseConstants.XpInterval); }

        public void Tick()
        {
            if (data != null && data.restored)
            {
                float multiplier = floors != null && tracker != null && floors.Valid(tracker.localFloorId) ? floors.xpMultipliers[tracker.localFloorId] : 1f;
                int area = areas != null && tracker != null ? areas.IndexOf(tracker.localAreaId) : -1;
                if (area >= 0 && areas.xpMultipliers != null && area < areas.xpMultipliers.Length) multiplier *= areas.xpMultipliers[area];
                fractionalXp += .5f * multiplier;
                int whole = Mathf.FloorToInt(fractionalXp);
                if (whole > 0) { data.experience += whole; fractionalXp -= whole; }
                data.totalSeconds += 30;
                data.Save();
            }
            SendCustomEventDelayedSeconds("Tick", BaseConstants.XpInterval);
        }
    }
}
