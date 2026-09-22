using UdonSharp;
using VRC.SDKBase;

namespace BH2VSQ.Base
{
    public class AccessManager : UdonSharpBehaviour
    {
        public FloorManager floors;
        public TeleportManager teleport;
        public PermissionManager permission;

        public AccessResult CheckPointAccess(TeleportPoint point)
        {
            if (!Utilities.IsValid(Networking.LocalPlayer)) return AccessResult.InvalidPlayer;
            if (point == null) return AccessResult.InvalidArea;
            if (permission == null || permission.GetRank() < point.requiredRank) return AccessResult.InsufficientRank;
            if (permission.IsAdmin()) return AccessResult.Allowed;
            FloorState state = floors == null ? FloorState.Maintenance : floors.GetState(point.floorId);
            if (state == FloorState.Reserved) return AccessResult.FloorReserved;
            if (state == FloorState.Maintenance) return AccessResult.FloorMaintenance;
            return AccessResult.Allowed;
        }

        public AccessResult CheckFloorAccess(int floorId)
        {
            if (teleport == null || teleport.ByFloor(floorId) == null) return AccessResult.InvalidFloor;
            return CheckPointAccess(teleport.ByFloor(floorId));
        }

        public AccessResult CheckAreaAccess(int locationId)
        {
            return CheckPointAccess(teleport == null ? null : teleport.ById(locationId));
        }
    }
}
