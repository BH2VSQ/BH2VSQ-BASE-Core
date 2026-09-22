using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace BH2VSQ.Base
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class TeleportRequestManager : UdonSharpBehaviour
    {
        public TeleportManager teleport;
        public RequestPanel panel;
        public float timeoutSeconds = BaseConstants.RequestTimeout;
        [UdonSynced] private int requestId;
        [UdonSynced] private int requesterId;
        [UdonSynced] private int targetId;
        [UdonSynced] private int requestType; // 0: go to target, 1: invite target.
        [UdonSynced] private int requestState; // 1 pending, 2 accepted, 3 rejected.
        [UdonSynced] private long expiryTicks;
        private int handledId;

        private void Start() { SendCustomEventDelayedSeconds("Tick", 1f); }

        public void Tick()
        {
            if (requestState == 1 && Utilities.IsValid(Networking.LocalPlayer) && targetId == Networking.LocalPlayer.playerId)
            {
                long remaining = expiryTicks - Networking.GetNetworkDateTime().Ticks;
                if (remaining <= 0) { if (panel != null) panel.Hide(); }
                else if (panel != null) panel.SetSeconds((int)(remaining / 10000000L) + 1);
            }
            SendCustomEventDelayedSeconds("Tick", 1f);
        }

        public bool Send(int playerId, int type)
        {
            if (!Utilities.IsValid(Networking.LocalPlayer) || !Utilities.IsValid(VRCPlayerApi.GetPlayerById(playerId))) return false;
            if (requestState == 1 && Networking.GetNetworkDateTime().Ticks < expiryTicks) return false;
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            requestId++;
            requesterId = Networking.LocalPlayer.playerId;
            targetId = playerId;
            requestType = type;
            requestState = 1;
            expiryTicks = Networking.GetNetworkDateTime().Ticks + (long)(timeoutSeconds * 10000000L);
            handledId = requestId;
            RequestSerialization();
            return true;
        }

        public override void OnDeserialization() { Refresh(); }

        public void Refresh()
        {
            if (!Utilities.IsValid(Networking.LocalPlayer)) return;
            if (requestState == 1 && targetId == Networking.LocalPlayer.playerId && requestId != handledId && Networking.GetNetworkDateTime().Ticks < expiryTicks)
            {
                handledId = requestId;
                if (panel != null) panel.Show(VRCPlayerApi.GetPlayerById(requesterId), requestType);
            }
            else if (requestState == 2 && requesterId == Networking.LocalPlayer.playerId && requestId == handledId && requestType == 0)
            {
                if (teleport != null) teleport.ToPlayer(targetId);
                handledId = -requestId;
            }
            else if (requestState == 2 && targetId == Networking.LocalPlayer.playerId && requestId == handledId && requestType == 1)
            {
                if (teleport != null) teleport.ToPlayer(requesterId);
                handledId = -requestId;
            }
            if ((requestState != 1 || Networking.GetNetworkDateTime().Ticks >= expiryTicks) && panel != null) panel.Hide();
        }

        public void Accept() { Complete(2); }
        public void Reject() { Complete(3); }
        private void Complete(int state)
        {
            if (!Utilities.IsValid(Networking.LocalPlayer) || targetId != Networking.LocalPlayer.playerId || requestState != 1) return;
            if (Networking.GetNetworkDateTime().Ticks >= expiryTicks) return;
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            requestState = state;
            RequestSerialization();
            Refresh();
        }
    }
}
