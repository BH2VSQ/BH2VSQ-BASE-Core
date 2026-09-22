using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace BH2VSQ.Base
{
    public class TeleportManager : UdonSharpBehaviour
    {
        public AccessManager access;
        public TeleportPoint[] points;
        public AccessResult lastResult;

        public bool ToFloor(int floorId)
        {
            lastResult = access.CheckFloorAccess(floorId);
            if (lastResult != AccessResult.Allowed) return false;
            for (int i = 0; i < points.Length; i++)
                if (points[i] != null && points[i].floorId == floorId) return Move(points[i]);
            return false;
        }

        public bool ToArea(int areaId)
        {
            lastResult = access.CheckAreaAccess(areaId);
            if (lastResult != AccessResult.Allowed) return false;
            for (int i = 0; i < points.Length; i++)
                if (points[i] != null && points[i].areaId == areaId) return Move(points[i]);
            return false;
        }

        public bool ToPlayer(int playerId)
        {
            VRCPlayerApi target = VRCPlayerApi.GetPlayerById(playerId);
            if (!Utilities.IsValid(target) || target.isLocal) return false;
            Vector3 position = target.GetPosition();
            Networking.LocalPlayer.TeleportTo(position + target.GetRotation() * Vector3.back, target.GetRotation());
            return true;
        }

        public void ToSafeFloor()
        {
            if (!ToFloor(3) && Utilities.IsValid(Networking.LocalPlayer)) Networking.LocalPlayer.Respawn();
        }

        private bool Move(TeleportPoint point)
        {
            if (!Utilities.IsValid(Networking.LocalPlayer)) return false;
            Networking.LocalPlayer.TeleportTo(point.Position(), point.Rotation());
            return true;
        }
    }
}
