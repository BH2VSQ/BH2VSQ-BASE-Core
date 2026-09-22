using UdonSharp;
using UnityEngine;

namespace BH2VSQ.Base
{
    public class TeleportPoint : UdonSharpBehaviour
    {
        public int pointId;
        public int floorId = 3;
        public int areaId = 4000;
        public string pointName = "1F";
        public Transform destination;

        public Vector3 Position() { return destination != null ? destination.position : transform.position; }
        public Quaternion Rotation() { return destination != null ? destination.rotation : transform.rotation; }
    }
}
