using UnityEngine;

namespace BH2VSQ.Base
{
    [CreateAssetMenu(menuName = "BH2VSQ BASE/中文文本")]
    public class LocalizationDatabase : ScriptableObject
    {
        public string[] chinese = DefaultChinese();

        public static string[] DefaultChinese() { return new[] {
            "访客", "成员", "管理员", "楼层", "在线", "电台", "值守中", "无人值守", "开放", "包场", "维护",
            "仅管理员可用", "离线", "未知", "电台值守", "等级", "页码", "无权访问或传送点不存在", "正在传送", "认证成功", "验证码无效"
        }; }
    }
}
