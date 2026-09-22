# 预制组件用途与实际场景配置

本文以已导入 `BH2VSQ_BASE_Core.unitypackage` 的 VRChat Worlds 项目为前提。**实际世界通常只需放入一个 `Prefabs/Core/BH2VSQ_BASE_Core.prefab`。** 它已包含运行所需的管理器、菜单、通知、请求界面和初始传送点。不要再把下表的零件预制体逐个叠加到同一场景，否则可能出现重复管理器、重复同步对象与冲突的 UI。

## 应该放入哪些对象

| 场景用途 | 应放入的对象 | 说明 |
| --- | --- | --- |
| 正式世界 | 一个 `BH2VSQ_BASE_Core.prefab` 实例；VRChat 的场景描述符、出生点与实际建筑 | 核心实例是唯一的 BASE 系统入口。已有世界 UI 可保留，但场景只能有一个有效 `EventSystem`。 |
| 新增房间或楼层 | 在核心实例的 `Teleport` 子节点下增加 `BH2VSQ_TeleportPoint.prefab`，或复制现有点 | 每个点代表一个位置；无需添加 `FloorController` 或修改脚本。 |
| 仅调试 | 打开 `Scenes/BH2VSQ_BASE_Demo.unity`，使用其中的核心实例 | 初始点在直线上，地面只是占位。不要将该演示布局直接当成成品世界。 |
| 独立定制 UI/管理器 | 按需使用下面的零件预制体 | 零件在导出时会清除指向核心外部对象的引用；必须在 Inspector 中重新连接依赖。 |

## 正式世界的配置顺序

1. 导入资源包。若场景尚无 VRChat 世界描述符和出生点，先按 VRChat Worlds SDK 要求添加。打开原有场景，拖入 **一个** 核心预制体实例；不要把演示场景中的第二套核心也保留下来。核心已带 `EventSystem`，若原场景已有一个，请只保留一个有效实例。
2. 展开核心的 `UI/MainCanvas`。菜单默认隐藏；桌面模式按 **Tab** 打开或关闭。蓝色半透明画布打开时位于视野左侧。选中核心根节点，在 Inspector 的 `BaseWorldSystem.menuOpacity` 设置整体不透明度（0 到 1），下次运行时生效。要给 VR 玩家提供入口，可在世界里的交互按钮上调用核心 `BaseWorldSystem.ToggleMenu` 公共事件。主菜单、广播弹窗和传送请求画布均须保留 `Canvas`、`VRC_UIShape`、`BoxCollider`、`GraphicRaycaster`；画布使用 Default 图层。不要把其他实体碰撞体放在菜单与玩家视线之间。
3. 在核心的 `Teleport` 子节点下移动初始 `Point_*` 到建筑中对应房间。点物体的 Transform（或它的 `destination`）是传送落点；调整其子物体 `AreaTrigger` 的 BoxCollider，使它覆盖实际入口或区域。演示点位只是占位。
4. 如需新增位置，复制 `BH2VSQ_TeleportPoint.prefab` 到同一 `Teleport` 子节点。设置唯一 `locationId`、中英文位置名、`floorId`、中英文楼层名、`requiredRank`、`tabVisible`、`xpMultiplier` 和目的地。新楼层直接使用新的 `floorId`；同一楼层的点须使用一致名称。只保留一个 `isSafeFallback`，且它应为游客可到达的安全位置。需要电台值守统计时，将对应点标为 `radioDutyArea`。
5. 在 **场景实例** 的 `Authentication/TOTPAuthManager` 设置不同的成员和管理员 Base32 密钥；空值会禁用对应登录。不要将带真实密钥的场景或预制体提交到公共仓库。上传的 VRChat 世界仍会向客户端暴露世界内密钥，不能将它用于真实身份或付费权限。
6. 根据需要调整 `Teleport/TeleportRequestManager.timeoutSeconds`（默认 15 秒）以及 `Admin/BroadcastManager` 的广播时长（普通 8 秒、重要 20 秒、紧急 120 秒）。其余核心引用由生成器连接，通常无需手动改动。
7. 选中核心实例并运行 **BH2VSQ BASE → 验证配置**。先在 ClientSim 中检查 Tab、三页菜单、中文切换、按钮、传送和弹窗，再用至少两个 VRChat 客户端验证所有权、同步、权限及广播。

`Data/Default/DefaultLocationDatabase.asset` 仅是重新生成核心时使用的初始位置种子。修改它不会更新已经放入场景的核心实例。`Data/Localization/DefaultLocalization.asset` 是重新生成界面时使用的中英文文本种子。

## 核心层级与职责

| 核心子节点 | 主要职责 | 常用配置 |
| --- | --- | --- |
| `Core` | 玩家登记、持久化经验/偏好、当前位置、经验计时、本地化 | 检查引用；经验每 30 秒按当前位置倍率累计。 |
| `Authentication` | 本地 TOTP 会话与游客/成员/管理员等级 | 仅在场景实例设置世界专用密钥。 |
| `Floor` | 从位置点汇总楼层、检查通行权限、统计人数与电台值守 | 楼层 ID/名称与权限在 `TeleportPoint` 上配置；状态由管理员界面更改。 |
| `Teleport` | 位置点扫描、移动玩家、双向传送请求 | 移动点、设置区域触发器和请求超时。 |
| `Admin` | 管理员授权、楼层状态与广播 | 设置广播时长；运行时由管理员操作。 |
| `UI` | Tab 菜单、玩家列表、通知与传送请求 | 保留交互画布组件和 EventSystem；不需要复制其他 UI 零件。 |

## 每个预制体的用途

下面的“单独使用”说明适用于改造现有核心或开发自定义界面。零件不能代替完整核心；拖入后需手动连接其组件 Inspector 字段。

### Core 与 Demo

| 预制体 | 用途 | 使用方法 |
| --- | --- | --- |
| `Core/BH2VSQ_BASE_Core.prefab` | 完整系统，包含所有管理器、界面和初始点 | 正式场景放入一个实例；按上文配置点位、密钥、出生点及现有 EventSystem 冲突。 |
| `Demo/BH2VSQ_DemoBase.prefab` | 与生成时核心相同的演示副本 | 仅供检查布局/测试。不要与 Core 同时放入正式场景。 |

### Teleport 与 Floor

| 预制体 | 用途 | 单独使用时的连接与配置 |
| --- | --- | --- |
| `Teleport/BH2VSQ_TeleportPoint.prefab` | 一个可传送的位置，附可视标记与 `AreaTrigger` | 放在核心 `Teleport` 子节点下，设置唯一位置 ID、楼层 ID/名称、等级、目的地与触发范围。复制后取消多余的 `isSafeFallback`。运行时扫描子节点，无需改数据库。 |
| `Floor/BH2VSQ_AreaTrigger.prefab` | 玩家进入区域时更新当前位置，阻止无权进入 | 放在某个 `TeleportPoint` 下，保留 `BoxCollider.isTrigger`，调整位置与尺寸；`point` 指向父点，`tracker`、`access`、`teleport` 指向核心管理器。新点预制体已自带触发器，不要重复添加。 |
| `Floor/BH2VSQ_FloorController.prefab` | `FloorManager` 模板，按点汇总楼层并处理状态 | 核心已包含。自定义替换时连接 `teleport`；不要与原有 FloorManager 并存。 |
| `Floor/BH2VSQ_AreaController.prefab` | `AreaManager` 模板，按点提供位置名称和倍率查询 | 核心已包含。自定义替换时连接 `teleport`；不要与原有 AreaManager 并存。 |

### UI

| 预制体 | 用途 | 单独使用时的连接与配置 |
| --- | --- | --- |
| `UI/BH2VSQ_TabMenu.prefab` | 主菜单容器及全部页签/子面板 | 必须放进可交互的 World Space Canvas，连接 `permission`、各页对象和按钮的目标管理器。完整核心中已生成并连接；单独拖入不会自动组成系统。 |
| `UI/BH2VSQ_PersonalInfo.prefab` | 显示等级、经验、时长、位置及偏好 | 连接 `PlayerDataManager`、`PermissionManager`、`PlayerAreaTracker`、楼层/位置及本地化管理器。 |
| `UI/BH2VSQ_PermissionLogin.prefab` | 成员/管理员六位 TOTP 输入和反馈 | 连接 `TOTPAuthManager` 与主菜单；实际密钥在场景实例的认证组件上填写。 |
| `UI/BH2VSQ_Teleport.prefab` | 可分页的位置按钮与传送确认 | 连接 `TeleportManager`、玩家偏好和本地化；按钮从 `TeleportPoint.tabVisible` 自动生成。 |
| `UI/BH2VSQ_PlayerList.prefab` | 玩家列表、刷新与右侧详情区 | 连接登记、位置、电台、玩家数据、详情及本地化组件。 |
| `UI/BH2VSQ_PlayerListItem.prefab` | 玩家列表的一条按钮模板 | 由玩家列表复用；其 `UIButtonAction` 应连到详情组件并设置玩家 ID。不要单独放入场景。 |
| `UI/BH2VSQ_PlayerDetail.prefab` | 选中玩家的等级/位置详情与传送请求按钮 | 连接玩家登记、位置、楼层、本地化及 `TeleportRequestManager`。 |
| `UI/BH2VSQ_AdminPanel.prefab` | 管理员主页，含楼层选择、人数和广播入口 | 连接管理员、楼层状态、位置人数、电台、广播与本地化组件；仅管理员可使用。 |
| `UI/BH2VSQ_FloorAdmin.prefab` | 楼层状态选择按钮区 | 是 AdminPanel 的子面板；通过 `FloorAdminManager` 设置开放/包场/维护状态。 |
| `UI/BH2VSQ_BroadcastPanel.prefab` | 管理员编辑、选择优先级并发送广播 | 连接 `BroadcastManager` 与本地化；不要在场景中另放一套广播管理器。 |
| `UI/BH2VSQ_BroadcastNotificationManager.prefab` | 本地广播通知队列和传送请求弹窗容器 | 连接玩家偏好、本地化、广播/请求管理器与三种提示音；弹窗画布只在显示时开启，防止遮挡菜单点击。核心已包含。 |
| `UI/BH2VSQ_BroadcastNotification.prefab` | 单条广播的标题、消息、发送者和关闭按钮 | 是通知容器内的显示模板；需要由 `BroadcastNotificationManager` 控制，不应独立使用。 |
| `UI/BH2VSQ_TeleportRequest.prefab` | 传送请求提示、倒计时、接受和拒绝按钮 | 是通知容器内的模板；连接 `RequestPanel` 与 `TeleportRequestManager`。 |

## 交互故障排查

- **菜单偏到画面右下或被裁切**：使用新版生成的核心；确认 `MainCanvas` 的 RectTransform 轴心为中心，保留 `LocalCanvasFollower`，不要把旧版场景实例当成已更新资源。重新放置核心实例前先保存自己的点位和密钥。
- **Tab 无反应**：确认核心的 `BaseWorldSystem` 已启用，`menuCanvas` 与 `menuFollower` 引用不为空，并确保游戏窗口有焦点。菜单在场景编辑状态下默认隐藏，进入模拟模式后按 Tab 开关。
- **能看到按钮但点不动**：画布本体必须有 `VRC_UIShape`、`BoxCollider`、`GraphicRaycaster`，图层为 Default；场景有且只有一个有效 `EventSystem`。Button 的 Image 必须可被射线命中，`UIButtonAction.Click` 的 Udon 事件必须已连接。检查前方是否有其他碰撞体遮挡。
- **点开零件预制体不工作**：零件导出时清除核心外部引用。优先从完整核心开始修改；独立使用时按上表把引用逐项连回核心，并运行验证器。
- **中文仍显示英文**：先在个人页切换语言；确认本地化数据已随最新版核心重新生成。用户的语言偏好会持久化。
