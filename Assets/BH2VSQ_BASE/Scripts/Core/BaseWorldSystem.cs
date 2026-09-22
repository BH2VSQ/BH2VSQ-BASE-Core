using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace BH2VSQ.Base
{
    public class BaseWorldSystem : UdonSharpBehaviour
    {
        public PlayerRegistry registry;
        public PlayerDataManager playerData;
        public AuthenticationSession session;
        public FloorManager floors;
        public AccessManager access;
        public TeleportManager teleport;
        public TabMenuController menu;
        public bool ready;

        private void Start()
        {
            ready = registry != null && playerData != null && session != null && floors != null && access != null && teleport != null;
            if (!ready) Debug.LogError("BH2VSQ BASE: missing core reference. Run Validator.");
            else
            {
                registry.Refresh();
                session.ResetSession();
                if (menu != null) menu.Refresh();
            }
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (registry != null) registry.Refresh();
            if (menu != null) menu.Refresh();
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            if (registry != null) registry.Refresh();
            if (menu != null) menu.Refresh();
        }
    }
}
