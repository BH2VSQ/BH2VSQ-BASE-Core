using UdonSharp;
using TMPro;
using UnityEngine;
using VRC.SDKBase;

namespace BH2VSQ.Base
{
    public class PersonalInfoPanel : UdonSharpBehaviour
    {
        public TMP_Text nameText;
        public TMP_Text rankText;
        public TMP_Text levelText;
        public TMP_Text xpText;
        public TMP_Text timeText;
        public RectTransform xpFill;
        public PlayerDataManager data;
        public PermissionManager permission;
        public PlayerAreaTracker tracker;
        public FloorManager floors;
        public AreaManager areas;
        public LocalizationManager localization;

        public void Refresh()
        {
            if (!Utilities.IsValid(Networking.LocalPlayer) || data == null) return;
            int level = data.Level();
            int currentThreshold = (level - 1) * (level - 1) * 100;
            int nextThreshold = level * level * 100;
            int seconds = data.totalSeconds;
            if (nameText != null) nameText.text = Networking.LocalPlayer.displayName;
            if (rankText != null && permission != null && localization != null) rankText.text = localization.RankName(permission.GetRank());
            if (levelText != null) levelText.text = "等级  " + level;
            if (xpText != null) xpText.text = "经验  " + data.experience + " / " + nextThreshold;
            if (timeText != null) timeText.text = "游戏时长  " + (seconds / 3600) + "小时 " + ((seconds % 3600) / 60) + "分";
            if (xpFill != null) xpFill.sizeDelta = new Vector2(485f * Mathf.Clamp01((float)(data.experience - currentThreshold) / (nextThreshold - currentThreshold)), 7f);
        }
    }
}
