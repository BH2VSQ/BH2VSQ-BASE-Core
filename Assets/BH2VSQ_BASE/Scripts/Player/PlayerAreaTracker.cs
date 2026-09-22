using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace BH2VSQ.Base
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class PlayerAreaTracker : UdonSharpBehaviour
    {
        [UdonSynced] public int[] playerIds = new int[BaseConstants.MaxPlayers];
        [UdonSynced] public int[] areaIds = new int[BaseConstants.MaxPlayers];
        public AreaManager areas;
        public int localAreaId = BaseConstants.InvalidId;
        public int localFloorId = BaseConstants.InvalidId;

        public void EnterArea(int areaId)
        {
            if (areas == null || !Utilities.IsValid(Networking.LocalPlayer)) return;
            int area = areas.IndexOf(areaId);
            if (area < 0) return;
            localAreaId = areaId;
            localFloorId = areas.FloorIdAt(area);
            int id = Networking.LocalPlayer.playerId;
            int slot = -1;
            for (int i = 0; i < playerIds.Length; i++)
                if (playerIds[i] == id) { slot = i; break; }
                else if (slot < 0 && playerIds[i] == 0) slot = i;
            if (slot < 0) return;
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            playerIds[slot] = id;
            areaIds[slot] = areaId;
            RequestSerialization();
        }

        public int AreaForPlayer(int playerId)
        {
            for (int i = 0; i < playerIds.Length; i++) if (playerIds[i] == playerId) return areaIds[i];
            return BaseConstants.InvalidId;
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(Networking.LocalPlayer) || !Networking.IsOwner(gameObject)) return;
            for (int i = 0; i < playerIds.Length; i++) if (playerIds[i] == player.playerId)
            {
                playerIds[i] = 0;
                areaIds[i] = 0;
                RequestSerialization();
                return;
            }
        }
    }
}
