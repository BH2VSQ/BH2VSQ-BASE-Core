using UdonSharp;
using VRC.SDKBase;

namespace BH2VSQ.Base
{
    public class PermissionManager : UdonSharpBehaviour
    {
        public AuthenticationSession session;
        public PlayerRegistry registry;

        public BaseRank GetRank() { return session == null ? BaseRank.Visitor : session.rank; }
        public BaseRank GetPlayerRank(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(player)) return BaseRank.Visitor;
            if (player.isLocal) return GetRank();
            return registry == null ? BaseRank.Visitor : registry.RankForPlayer(player.playerId);
        }
        public bool IsPlayerVisitor(VRCPlayerApi player) { return GetPlayerRank(player) == BaseRank.Visitor; }
        public bool IsPlayerMember(VRCPlayerApi player) { return GetPlayerRank(player) >= BaseRank.Member; }
        public bool IsPlayerAdmin(VRCPlayerApi player) { return GetPlayerRank(player) == BaseRank.Admin; }
        public bool IsMember() { return GetRank() >= BaseRank.Member; }
        public bool IsAdmin() { return GetRank() == BaseRank.Admin; }
    }
}
