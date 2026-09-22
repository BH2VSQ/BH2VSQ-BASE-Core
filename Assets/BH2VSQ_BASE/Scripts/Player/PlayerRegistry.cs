using UdonSharp;
using VRC.SDKBase;

namespace BH2VSQ.Base
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class PlayerRegistry : UdonSharpBehaviour
    {
        public VRCPlayerApi[] players = new VRCPlayerApi[BaseConstants.MaxPlayers];
        public int count;
        [UdonSynced] public int[] rankPlayerIds = new int[BaseConstants.MaxPlayers];
        [UdonSynced] public int[] ranks = new int[BaseConstants.MaxPlayers];

        public void Refresh()
        {
            VRCPlayerApi.GetPlayers(players);
            count = VRCPlayerApi.GetPlayerCount();
            if (count > players.Length) count = players.Length;
        }

        public VRCPlayerApi FindById(int id)
        {
            return VRCPlayerApi.GetPlayerById(id);
        }

        public int FindPlayerIndex(int id)
        {
            for (int i = 0; i < count; i++) if (Utilities.IsValid(players[i]) && players[i].playerId == id) return i;
            return -1;
        }
        public VRCPlayerApi GetPlayer(int index)
        {
            return index >= 0 && index < count ? players[index] : null;
        }
        public int GetPlayerCount() { return count; }

        public void PublishRank(BaseRank rank)
        {
            if (!Utilities.IsValid(Networking.LocalPlayer)) return;
            int id = Networking.LocalPlayer.playerId;
            int slot = -1;
            for (int i = 0; i < rankPlayerIds.Length; i++)
                if (rankPlayerIds[i] == id) { slot = i; break; }
                else if (slot < 0 && rankPlayerIds[i] == 0) slot = i;
            if (slot < 0) return;
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            rankPlayerIds[slot] = id;
            ranks[slot] = (int)rank;
            RequestSerialization();
        }

        public BaseRank RankForPlayer(int id)
        {
            for (int i = 0; i < rankPlayerIds.Length; i++)
                if (rankPlayerIds[i] == id) return (BaseRank)ranks[i];
            return BaseRank.Visitor;
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(Networking.LocalPlayer) || !Networking.IsOwner(gameObject)) return;
            for (int i = 0; i < rankPlayerIds.Length; i++) if (rankPlayerIds[i] == player.playerId)
            {
                rankPlayerIds[i] = 0; ranks[i] = 0; RequestSerialization(); return;
            }
        }
    }
}
