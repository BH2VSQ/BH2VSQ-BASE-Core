namespace UdonSharp
{
    public class UdonSharpBehaviour { }
}

namespace UnityEngine
{
    public class SerializeField : System.Attribute { }
}

namespace VRC.SDKBase
{
    public static class Networking
    {
        public static System.DateTime Now;
        public static System.DateTime GetNetworkDateTime() => Now;
    }
}

namespace BH2VSQ.Base
{
    public enum BaseRank { Visitor, Member, Admin }
    public class AuthenticationSession
    {
        public BaseRank rank;
        public void Authenticate(BaseRank value) => rank = value;
    }
}
