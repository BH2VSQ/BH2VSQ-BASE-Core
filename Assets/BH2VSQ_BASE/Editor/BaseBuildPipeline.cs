using System;
using UnityEditor;
using UnityEngine;

namespace BH2VSQ.Base.Editor
{
    public static class BaseBuildPipeline
    {
        [MenuItem("BH2VSQ BASE/构建资源包")]
        public static void Run()
        {
            BaseSetupWizard.BuildAll();
            if (!BaseValidator.ValidateSelection(false)) throw new InvalidOperationException("BH2VSQ BASE 配置验证失败。");
            BaseSetupWizard.Export();
            Debug.Log("BH2VSQ BASE 构建完成。");
        }
    }
}
