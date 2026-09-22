using UnityEngine;

namespace BH2VSQ.Base
{
    [CreateAssetMenu(menuName = "BH2VSQ BASE/基础配置")]
    public class BaseConfig : ScriptableObject
    {
        public LocationDatabase locations;
        [Min(1)] public int requestTimeoutSeconds = 30;
    }
}
