using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace BH2VSQ.Base
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class FloorManager : UdonSharpBehaviour
    {
        public string[] floorNames = { "B3", "B2", "B1", "1F", "2F", "3F", "4F", "5F", "6F", "7F" };
        public int[] requiredRanks = { 2, 1, 0, 0, 0, 0, 0, 0, 1, 1 };
        public float[] xpMultipliers = { 5f, 3f, 1f, .5f, 1.5f, 1f, 1f, 1f, 2f, 3f };
        [UdonSynced] public int[] states = new int[10];

        public bool Valid(int floorId) { return floorId >= 0 && floorId < floorNames.Length; }
        public string GetFloor(int floorId) { return Valid(floorId) ? floorNames[floorId] : ""; }
        public bool IsFloorAvailable(int floorId) { return Valid(floorId) && GetState(floorId) == FloorState.Open; }
        public FloorState GetState(int floorId)
        {
            if (!Valid(floorId) || states == null || floorId >= states.Length) return FloorState.Maintenance;
            return (FloorState)states[floorId];
        }

        public bool SetState(int floorId, FloorState state, PermissionManager permission)
        {
            if (!Valid(floorId) || permission == null || !permission.IsAdmin()) return false;
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            states[floorId] = (int)state;
            RequestSerialization();
            return true;
        }
    }
}
