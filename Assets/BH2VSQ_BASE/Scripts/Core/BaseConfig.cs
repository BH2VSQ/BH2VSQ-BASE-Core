using UnityEngine;

namespace BH2VSQ.Base
{
    [CreateAssetMenu(menuName = "BH2VSQ BASE/基础配置")]
    public class BaseConfig : ScriptableObject
    {
        public LocationDatabase locations;
        [Min(1)] public int requestTimeoutSeconds = 15;
        [Min(1)] public int broadcastNormalSeconds = 8;
        [Min(1)] public int broadcastImportantSeconds = 20;
    }
}
