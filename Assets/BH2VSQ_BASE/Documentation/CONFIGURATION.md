# 配置说明

## 新增位置或楼层

将 `Prefabs/Teleport/BH2VSQ_TeleportPoint.prefab` 或现有点复制到核心的 `Teleport` 子节点。给新点设置唯一 `locationId`，填写 `locationName` / `chineseName`、`floorId`、`floorName` / `chineseFloorName`、`requiredRank` 和 `xpMultiplier`。点的 Transform 是传送目的地；按实际空间调整子物体 `AreaTrigger` 的碰撞体。

勾选 `tabVisible` 才在传送页显示。用 `radioDutyArea` 标记值守区。全场景只能有一个 `isSafeFallback` 安全返回点。新楼层只需使用新的 `floorId`；同楼层的所有点须使用一致的楼层名称。楼层状态同步到该楼层各点。传送页与管理员页分页显示，位置数量不固定。增删点后运行验证器。

`Data/Default/DefaultLocationDatabase.asset` 是首次生成种子，默认包含 B3、B2、B1、1F 至 7F 与屋顶共 14 个位置，不是运行时目录。它的修改不会覆盖已有场景实例。`Data/Localization/DefaultLocalization.asset` 包含中英文界面文本；玩家可切换语言并保存偏好。

## 核心组件

- `Authentication/TOTPAuthManager`：成员和管理员 Base32 密钥应不同；空值禁用对应等级。六位 HMAC-SHA1 TOTP，30 秒周期，允许前后各一个周期。
- `Teleport/TeleportRequestManager`：默认 15 秒超时。共享单个同步请求槽；类型 0 是请求者传向目标，类型 1 是目标传向请求者。
- `Admin/BroadcastManager`：普通、重要、紧急广播默认持续 8、20、120 秒。八槽同步环保存内容与过期时间，各客户端独立排队显示；紧急广播由本地用户关闭。
- `UI/NotificationLayer`：跟随本地视角；主菜单是独立的世界空间画布。

`PlayerDataManager` 等待 `OnPlayerRestored` 后读写经验、在线时长、语言和通知偏好。每 30 秒按当前点的经验倍率累计经验。登录等级是本地会话，重新加入后回到游客；同步等级及经验不构成可信权限。共享的玩家列表、区域、楼层、请求及广播在多人并发写入时可能互相覆盖，详见[安全边界](SECURITY.md)。