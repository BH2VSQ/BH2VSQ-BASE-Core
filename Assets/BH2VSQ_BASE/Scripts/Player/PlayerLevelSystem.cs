using UdonSharp;
using UnityEngine;

namespace BH2VSQ.Base
{
    public class PlayerLevelSystem : UdonSharpBehaviour
    {
        public PlayerDataManager data;
        public PlayerAreaTracker tracker;
        public AreaManager areas;
        private float fractionalXp;

        private void Start() { SendCustomEventDelayedSeconds("Tick", BaseConstants.XpInterval); }

        public void Tick()
        {
            if (data != null && data.restored)
            {
                int area = areas != null && tracker != null ? areas.IndexOf(tracker.localAreaId) : -1;
                float multiplier = area >= 0 ? areas.XpMultiplierAt(area) : 1f;
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
