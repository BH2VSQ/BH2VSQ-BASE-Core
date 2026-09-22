# 安装说明

## 环境

- Unity Editor 2022.3.22f1，已激活许可证。
- VRChat Worlds SDK 3.10.4、TextMesh Pro 3.0.6。
- 从此仓库批量构建时需 Windows PowerShell 和 Python 3。

## 从源码构建

在仓库根目录运行 `tools/Build-Package.ps1`。脚本下载并校验锁定的 VRChat 包，从 Unity 本地包缓存准备 TextMesh Pro 必需资源，再执行 Unity 编译、生成、静态验证及导出。结果位于 `Releases/BH2VSQ_BASE_Core.unitypackage`，详细日志在 `unity-build.log`。

Unity 编辑器菜单 **BH2VSQ BASE → 构建资源包** 也可生成资源。若 TextMesh Pro 必需资源尚未导入，先用 **Window → TextMeshPro → Import TMP Essential Resources**。构建会生成 UdonSharp 程序资源、默认数据、字体、核心与组件预制体以及示例场景。

## 放入世界

1. 在目标 VRChat Worlds 项目中导入资源包，将**一个** `Prefabs/Core/BH2VSQ_BASE_Core.prefab` 拖入场景；不要再叠加 Demo 副本或同类零件管理器。核心已带 `EventSystem`，若场景原有一个，请只保留一个有效实例。
2. 将 `Teleport/Point_*` 移到建筑中的目的地，调整其 `AreaTrigger` 碰撞体，让所有需要识别的位置（包括出生区域）得到覆盖。界面会自动刷新位置和人数。
3. 在场景实例的 `Authentication/TOTPAuthManager` 设置成员与管理员 Base32 密钥。不要把真实密钥提交到公共仓库；上传的世界依然向客户端暴露密钥。
4. 将世界出生点放在公共安全位置。若要在玩家进入无权限区域时自动传回，在核心根节点启用 `returnUnauthorizedPlayersToSafePoint`，并指定唯一的 `isSafeFallback` 安全点；默认不开启。
5. 选中核心实例，运行 **BH2VSQ BASE → 验证配置**，修正错误。
6. 进入 ClientSim 后**按住 Tab** 显示菜单、松开隐藏，检查中文顶部资料卡、TOTP 输入、直接传送、人数和当前位置高亮、请求页同意/拒绝与结果提示。再用至少两个 VRChat 客户端测试游客、成员、管理员、迟到加入、区域自动更新、并发申请和楼层状态。各零件的用途和连接方法见 [预制组件说明](PREFABS.md)。

## 重新生成

构建会覆盖生成的预制体与示例场景，**不会自动替换已经放入其他场景的旧版实例**。升级旧版场景时，先记录点位、触发器和密钥，再用新版核心替换并重新配置。特定世界的配置应保存在场景实例中。修改 `Data/Default/DefaultLocationDatabase.asset` 只影响后续生成的初始点；已有实例可直接增删 `TeleportPoint`，无须修改代码。交互式构建前请保存已修改场景。
