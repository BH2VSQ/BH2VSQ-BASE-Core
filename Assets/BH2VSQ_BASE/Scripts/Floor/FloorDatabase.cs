using UnityEngine;

namespace BH2VSQ.Base
{
    [CreateAssetMenu(menuName = "BH2VSQ BASE/Floor Database")]
    public class FloorDatabase : ScriptableObject
    {
        public int[] ids = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        public string[] names = { "B3", "B2", "B1", "1F", "2F", "3F", "4F", "5F", "6F", "7F" };
        public int[] requiredRanks = { 2, 1, 0, 0, 0, 0, 0, 0, 1, 1 };
        public float[] xpMultipliers = { 5f, 3f, 1f, .5f, 1.5f, 1f, 1f, 1f, 2f, 3f };
    }

}
