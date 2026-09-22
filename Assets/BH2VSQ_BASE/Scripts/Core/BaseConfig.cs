using UnityEngine;

namespace BH2VSQ.Base
{
    [CreateAssetMenu(menuName = "BH2VSQ BASE/Base Config")]
    public class BaseConfig : ScriptableObject
    {
        public FloorDatabase floors;
        public AreaDatabase areas;
        [Min(1)] public int requestTimeoutSeconds = 15;
        [Min(1)] public int broadcastNormalSeconds = 8;
        [Min(1)] public int broadcastImportantSeconds = 20;
    }
}
