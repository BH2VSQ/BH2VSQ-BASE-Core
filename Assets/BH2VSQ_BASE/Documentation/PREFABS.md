# 预制体说明

**BH2VSQ BASE → 构建资源包** 会生成完整核心、组件预制体与示例场景。

`Prefabs/Core/BH2VSQ_BASE_Core.prefab` 包含所有管理器、主菜单、本地通知与请求画布、14 个初始传送点和区域触发器。将一个实例放入世界场景，移动点、调整触发器，并在实例上配置 TOTP。此预制体不包含建筑模型。

`Prefabs/Demo/BH2VSQ_DemoBase.prefab` 与 `Scenes/BH2VSQ_BASE_Demo.unity` 用于演示；位置呈直线排列，仅是占位。示例场景在 SDK 示例预制体存在时会使用它。

可复用组件位于 `Prefabs/UI`、`Prefabs/Floor`、`Prefabs/Teleport`。单独抽出的组件不会保留对核心中其他对象的引用；独立使用时须在 Inspector 连接相应管理器。添加传送点时应放在核心的 `Teleport` 子节点下，让运行时自动扫描。完整核心预制体是推荐的安装入口。

重新构建会覆盖生成的预制体与示例场景。世界专属的点位、触发器和密钥请保存在独立场景实例中。修改后运行 **BH2VSQ BASE → 验证配置**。