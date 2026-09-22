using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace BH2VSQ.Base
{
    public class LocalCanvasFollower : UdonSharpBehaviour
    {
        public Transform target;
        public Vector3 offset = new Vector3(0f, -.08f, 1.15f);
        public bool followRotation = true;

        private void LateUpdate() { Place(); }

        public void Place()
        {
            VRCPlayerApi player = Networking.LocalPlayer;
            if (!Utilities.IsValid(player) || target == null) return;
            VRCPlayerApi.TrackingData head = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            target.position = head.position + head.rotation * offset;
            if (followRotation) target.rotation = head.rotation;
        }
    }
}
