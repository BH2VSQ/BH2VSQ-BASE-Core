using UdonSharp;
using VRC.SDKBase;

namespace BH2VSQ.Base
{
    public class AreaTrigger : UdonSharpBehaviour
    {
        public PlayerAreaTracker tracker;
        public AccessManager access;
        public TeleportManager teleport;
        public int areaId = 4000;
        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (!player.isLocal) return;
            if (access != null && access.CheckAreaAccess(areaId) != AccessResult.Allowed)
            {
                if (teleport != null) teleport.ToSafeFloor();
                return;
            }
            if (tracker != null) tracker.EnterArea(areaId);
        }
    }
}
