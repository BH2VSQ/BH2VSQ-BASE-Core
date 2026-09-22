using UdonSharp;

namespace BH2VSQ.Base
{
    public class AuthenticationSession : UdonSharpBehaviour
    {
        public BaseRank rank;
        public long authenticatedTicks;
        public PlayerRegistry registry;

        public void ResetSession()
        {
            rank = BaseRank.Visitor;
            authenticatedTicks = 0;
            if (registry != null) registry.PublishRank(rank);
        }

        public void Authenticate(BaseRank newRank)
        {
            if (newRank <= rank) return;
            rank = newRank;
            authenticatedTicks = VRC.SDKBase.Networking.GetNetworkDateTime().Ticks;
            if (registry != null) registry.PublishRank(rank);
        }
    }
}
