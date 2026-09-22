using System;
using UnityEngine;

namespace BH2VSQ.Base.Editor
{
    public static class BaseBuildPipeline
    {
        public static void Run()
        {
            BaseSetupWizard.BuildAll();
            if (!BaseValidator.ValidateSelection(false)) throw new InvalidOperationException("BH2VSQ BASE validation failed.");
            BaseSetupWizard.Export();
            Debug.Log("BH2VSQ BASE build complete.");
        }
    }
}
