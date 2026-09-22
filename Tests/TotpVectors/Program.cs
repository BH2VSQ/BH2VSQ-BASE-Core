using System.Reflection;
using System.Text;
using BH2VSQ.Base;
using VRC.SDKBase;

var manager = new TOTPAuthManager();
var compute = typeof(TOTPAuthManager).GetMethod("Compute", BindingFlags.NonPublic | BindingFlags.Instance)!;
var decode = typeof(TOTPAuthManager).GetMethod("DecodeBase32", BindingFlags.NonPublic | BindingFlags.Instance)!;
byte[] secret = Encoding.ASCII.GetBytes("12345678901234567890");
var vectors = new (long Seconds, string Code)[]
{
    (59, "287082"),
    (1111111109, "081804"),
    (1111111111, "050471"),
    (1234567890, "005924"),
    (2000000000, "279037"),
    (20000000000, "353130"),
};
foreach (var vector in vectors)
{
    string actual = (string)compute.Invoke(manager, new object[] { secret, vector.Seconds / 30 })!;
    if (actual != vector.Code) throw new Exception($"TOTP vector at {vector.Seconds}: {actual} != {vector.Code}");
}
byte[] decoded = (byte[])decode.Invoke(manager, new object[] { "GEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQ" })!;
if (!decoded.SequenceEqual(secret)) throw new Exception("Base32 decode failed.");
typeof(TOTPAuthManager).GetField("memberTotpSecret", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(manager, "GEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQ");
manager.session = new AuthenticationSession();
Networking.Now = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(59);
if (!manager.Authenticate("287082") || manager.session.rank != BaseRank.Member) throw new Exception("Member authentication failed.");
Console.WriteLine("Six RFC 6238 SHA-1 vectors, Base32 decoding, and member login passed.");
