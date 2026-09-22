using UnityEngine;

namespace BH2VSQ.Base
{
    [CreateAssetMenu(menuName = "BH2VSQ BASE/Area Database")]
    public class AreaDatabase : ScriptableObject
    {
        public int[] ids = { 1000, 2000, 2001, 2002, 2003, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000, 10001 };
        public string[] names = { "B3 Secret", "B2", "B2 Monitoring", "B2 Server", "B2 Equipment Control", "B1 AV", "1F", "2F", "3F", "4F", "5F", "6F", "7F Radio", "Rooftop" };
        public int[] floorIds = { 0, 1, 1, 1, 1, 2, 3, 4, 5, 6, 7, 8, 9, 9 };
        public int[] requiredRanks = { 2, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 2 };
        public float[] xpMultipliers = { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
    }
}
