using UnityEngine;

namespace BH2VSQ.Base
{
    [CreateAssetMenu(menuName = "BH2VSQ BASE/本地化数据")]
    public class LocalizationDatabase : ScriptableObject
    {
        public string[] english = DefaultEnglish();
        public string[] chinese = DefaultChinese();

        public static string[] DefaultEnglish() { return new[] {
            "Personal", "Teleport", "Players", "Admin", "Login", "Send", "Close", "Accept", "Reject", "Refresh", "Broadcast", "Floor",
            "Loading player data...", "Visitor", "Member", "Administrator", "English", "Chinese", "Teleport confirm", "Notifications",
            "Select a destination", "Floors", "Areas", "Teleport?", "Confirm", "Cancel", "Online", "Radio", "On Duty", "No Duty",
            "Select a player", "Go to player", "Invite player", "Admin only", "Open", "Reserved", "Maintenance", "Apply state",
            "Player Management", "Population", "Normal", "Important", "Emergency", "Message (max 180 chars)", "NOTICE", "Teleport request",
            "6-digit TOTP", "Authenticated", "Invalid code", "Teleporting", "Access denied or point missing", "Offline", "Unknown",
            "Radio Duty", "XP", "Time", "Teleport confirmation", "On", "Off", " wants to teleport to you", " invited you", "Sent",
            "Admin only / empty message", "floor", "area", "s", "Lv.", "Count", "Page", "Locations", "Previous", "Next"
        }; }

        public static string[] DefaultChinese() { return new[] {
            "个人", "传送", "玩家", "管理", "登录", "发送", "关闭", "接受", "拒绝", "刷新", "广播", "楼层",
            "正在加载玩家数据…", "访客", "成员", "管理员", "英语", "中文", "传送确认", "通知",
            "请选择目的地", "楼层", "区域", "确认传送？", "确认", "取消", "在线", "电台", "值守中", "无人值守",
            "请选择玩家", "传送至玩家", "邀请玩家", "仅管理员可用", "开放", "包场", "维护", "应用状态",
            "玩家管理", "人数统计", "普通", "重要", "紧急", "消息（最多 180 字符）", "通知", "传送请求",
            "六位 TOTP 验证码", "认证成功", "验证码无效", "正在传送", "无权访问或传送点不存在", "离线", "未知",
            "电台值守", "经验", "时长", "传送确认", "开", "关", " 请求传送到你身边", " 邀请你传送过去", "已发送",
            "仅管理员可发送，或消息为空", "楼层", "区域", "秒", "等级", "人数", "页码", "地点", "上一页", "下一页"
        }; }
    }
}
