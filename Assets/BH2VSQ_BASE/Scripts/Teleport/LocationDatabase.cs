using UnityEngine;

namespace BH2VSQ.Base
{
    // Starter locations only. The runtime catalog is the TeleportPoint children in the scene.
    [CreateAssetMenu(menuName = "BH2VSQ BASE/默认地点数据")]
    public class LocationDatabase : ScriptableObject
    {
        public int[] ids = { 1000, 2000, 2001, 2002, 2003, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000, 10001 };
        public string[] names = { "B3 Secret", "B2", "B2 Monitoring", "B2 Server", "B2 Equipment Control", "B1 AV", "1F Living", "2F Entertainment", "3F Private", "4F Private", "5F Private", "6F Residence", "7F Radio", "Rooftop" };
        public string[] chineseNames = { "B3 秘密区", "B2", "B2 监控室", "B2 服务器室", "B2 设备控制室", "B1 影音室", "1F 生活区", "2F 娱乐区", "3F 私人区", "4F 私人区", "5F 私人区", "6F 住宅区", "7F 电台", "屋顶" };
        public int[] floorIds = { -3, -2, -2, -2, -2, -1, 1, 2, 3, 4, 5, 6, 7, 100 };
        public string[] floorNames = { "B3", "B2", "B2", "B2", "B2", "B1", "1F", "2F", "3F", "4F", "5F", "6F", "7F", "Rooftop" };
        public string[] chineseFloorNames = { "B3", "B2", "B2", "B2", "B2", "B1", "1F", "2F", "3F", "4F", "5F", "6F", "7F", "屋顶" };
        public BaseRank[] requiredRanks = { BaseRank.Admin, BaseRank.Member, BaseRank.Member, BaseRank.Member, BaseRank.Member, BaseRank.Visitor, BaseRank.Visitor, BaseRank.Visitor, BaseRank.Visitor, BaseRank.Visitor, BaseRank.Visitor, BaseRank.Member, BaseRank.Member, BaseRank.Admin };
        public bool[] tabVisible = { true, true, true, true, true, true, true, true, true, true, true, true, true, true };
        public float[] xpMultipliers = { 5f, 3f, 3f, 3f, 3f, 1f, .5f, 1.5f, 1f, 1f, 1f, 2f, 3f, 3f };
        public int safeFallbackId = 4000;
        public int radioDutyId = 10000;
    }
}
