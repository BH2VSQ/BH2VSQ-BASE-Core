namespace BH2VSQ.Base
{
    public static class BaseConstants
    {
        public const int MaxPlayers = 80;
        public const int InvalidId = int.MinValue;
        public const float XpInterval = 30f;
        public const float RequestTimeout = 15f;
    }

    public enum BaseRank { Visitor = 0, Member = 1, Admin = 2 }
    public enum FloorState { Open = 0, Reserved = 1, Maintenance = 2 }
    public enum AccessResult { Allowed = 0, InvalidPlayer = 1, InvalidFloor = 2, InvalidArea = 3, InsufficientRank = 4, FloorReserved = 5, FloorMaintenance = 6 }
    public enum BroadcastPriority { Normal = 0, Important = 1, Emergency = 2 }
}
