# BH2VSQ BASE Core

适用于 Unity 2022.3.22f1、VRChat Worlds SDK 3.10.4 的 UdonSharp 基础系统。以需求文档 V1.3 为基线，包含玩家资料、经验、TOTP 本地会话、位置权限、传送与同意请求、楼层状态、电台值守、管理员操作和实例广播。

## 快速开始

1. 激活 Unity 2022.3.22f1，在仓库根目录运行 `tools/Build-Package.ps1`。脚本准备 VRChat SDK 和 TextMesh Pro 必需资源，编译 UdonSharp，生成预制体与示例场景，验证引用并导出 `Releases/BH2VSQ_BASE_Core.unitypackage`。
2. 在 VRChat Worlds 项目中导入资源包，将 `Assets/BH2VSQ_BASE/Prefabs/Core/BH2VSQ_BASE_Core.prefab` 放入场景。
3. 将 `Teleport/Point_*` 移到实际目的地，调整触发器。在场景实例上配置仅供此世界使用的 TOTP 密钥，运行 **BH2VSQ BASE → 验证配置**。
4. 用 ClientSim 和至少两个 VRChat 客户端检查权限、同步、传送请求、楼层状态与广播。

每个 `TeleportPoint` 都保存位置和楼层 ID、名称、权限、传送页显示状态、目的地、经验倍率等信息。新增楼层或位置时，在核心的 `Teleport` 子节点下复制点，修改字段并移动到目的地；运行时会重新扫描，无须修改代码。`DefaultLocationDatabase.asset` 只提供初始示例点。

详见[安装](Assets/BH2VSQ_BASE/Documentation/INSTALL.md)、[配置](Assets/BH2VSQ_BASE/Documentation/CONFIGURATION.md)、[预制体](Assets/BH2VSQ_BASE/Documentation/PREFABS.md)、[安全](Assets/BH2VSQ_BASE/Documentation/SECURITY.md)和[验证记录](verification.md)。

