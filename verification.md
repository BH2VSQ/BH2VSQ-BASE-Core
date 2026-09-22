# 验证记录

## 本地已完成

| 检查 | 结果 |
| --- | --- |
| Unity 2022.3.22f1 批量构建 | 通过，进程退出码 0；UdonSharp 编译、核心与组件预制体、默认数据、示例场景和 Unity 资源包生成成功 |
| 配置验证器 | 通过；提示 TOTP 密钥为空，符合公开仓库默认配置 |
| C# 语法检查 | 43 个项目 C# 文件，0 个语法或类名错误 |
| 运行时代码类型检查 | 0 个警告、0 个错误；使用本地 Unity/VRChat/Udon API 桩，不替代 UdonSharp 编译 |
| TOTP 向量 | RFC 6238 的 6 个 SHA-1 向量、Base32 解码和成员登录通过 |
| 导出资源包清单 | 包含核心预制体、示例场景、TextMesh Pro 必需资源及已编译的 SerializedUdonPrograms |

构建命令：`tools/Build-Package.ps1`。产物：`Releases/BH2VSQ_BASE_Core.unitypackage`。该文件按仓库规则不提交到 Git，可在本机直接导入。构建日志 `unity-build.log` 中有 Unity SceneTemplate 在保存示例场景时的非致命 `ArgumentNullException`；构建继续并成功导出资源包，验证器通过。

## 尚未覆盖

未在 VRChat 客户端或 ClientSim 中执行交互验证，也未运行双客户端网络测试。楼层状态、共享玩家与区域数组、单槽传送请求、广播队列及所有权竞争必须在实际世界中验证。Udon 中的 TOTP 不能保护真实密钥或付费权限，详见[安全边界](Assets/BH2VSQ_BASE/Documentation/SECURITY.md)。