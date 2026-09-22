using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace BH2VSQ.Base
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class BroadcastManager : UdonSharpBehaviour
    {
        public AdminManager admin;
        public BroadcastNotificationManager notification;
        public int normalSeconds = 8;
        public int importantSeconds = 20;
        public int emergencySeconds = 120;
        [UdonSynced] public int sequence;
        [UdonSynced] public int[] ids = new int[8];
        [UdonSynced] public int[] priorities = new int[8];
        [UdonSynced] public string[] messages = new string[8];
        [UdonSynced] public string[] senders = new string[8];
        [UdonSynced] public int[] senderIds = new int[8];
        [UdonSynced] public long[] sentTicks = new long[8];
        [UdonSynced] public long[] expires = new long[8];
        [UdonSynced] public bool[] active = new bool[8];
        private int lastSeen;
        private long joinedTicks;

        private void Start() { joinedTicks = Networking.GetNetworkDateTime().Ticks; }

        public bool SendBroadcast(string message, BroadcastPriority priority)
        {
            if (admin == null || !admin.CanManage() || string.IsNullOrEmpty(message)) return false;
            if (message.Length > 180) message = message.Substring(0, 180);
            VRCPlayerApi local = Networking.LocalPlayer;
            if (!Utilities.IsValid(local)) return false;
            Networking.SetOwner(local, gameObject);
            sequence++;
            int slot = sequence % ids.Length;
            ids[slot] = sequence;
            priorities[slot] = (int)priority;
            messages[slot] = message;
            senders[slot] = local.displayName;
            senderIds[slot] = local.playerId;
            sentTicks[slot] = Networking.GetNetworkDateTime().Ticks;
            int duration = priority == BroadcastPriority.Normal ? normalSeconds : priority == BroadcastPriority.Important ? importantSeconds : emergencySeconds;
            expires[slot] = sentTicks[slot] + (long)duration * 10000000L;
            active[slot] = true;
            RequestSerialization();
            Receive();
            return true;
        }

        public override void OnDeserialization() { Receive(); }

        public void Receive()
        {
            if (notification == null || ids == null) return;
            long now = Networking.GetNetworkDateTime().Ticks;
            if (joinedTicks == 0) joinedTicks = now;
            int first = lastSeen + 1;
            if (first < sequence - ids.Length + 1) first = sequence - ids.Length + 1;
            for (int next = first; next <= sequence; next++)
            {
                int slot = next % ids.Length;
                if (ids[slot] != next || !active[slot] || expires[slot] <= now) continue;
                if (priorities[slot] == (int)BroadcastPriority.Normal && sentTicks[slot] < joinedTicks) continue;
                notification.Enqueue(next, priorities[slot], messages[slot], senders[slot], expires[slot]);
            }
            if (sequence > lastSeen) lastSeen = sequence;
        }
    }
}
