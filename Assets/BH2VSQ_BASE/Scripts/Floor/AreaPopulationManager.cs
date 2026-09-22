using UdonSharp;
using VRC.SDKBase;

namespace BH2VSQ.Base
{
    public class AreaPopulationManager : UdonSharpBehaviour
    {
        public PlayerAreaTracker tracker;
        public int Count(int areaId)
        {
            if (tracker == null) return 0;
            int count = 0;
            for (int i = 0; i < tracker.playerIds.Length; i++)
                if (tracker.playerIds[i] != 0 && tracker.areaIds[i] == areaId && Utilities.IsValid(VRCPlayerApi.GetPlayerById(tracker.playerIds[i]))) count++;
            return count;
        }
        public int CopyPlayerIds(int areaId, int[] destination)
        {
            if (tracker == null || destination == null) return 0;
            int count = 0;
            for (int i = 0; i < tracker.playerIds.Length && count < destination.Length; i++)
                if (tracker.playerIds[i] != 0 && tracker.areaIds[i] == areaId && Utilities.IsValid(VRCPlayerApi.GetPlayerById(tracker.playerIds[i])))
                    destination[count++] = tracker.playerIds[i];
            return count;
        }
    }
}
