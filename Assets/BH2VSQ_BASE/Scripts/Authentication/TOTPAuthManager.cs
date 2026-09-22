using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace BH2VSQ.Base
{
    // RFC 6238 / HMAC-SHA1.
    // Secrets embedded in an uploaded VRChat world are not private.
    public class TOTPAuthManager : UdonSharpBehaviour
    {
        [SerializeField] private string memberTotpSecret = "";
        [SerializeField] private string adminTotpSecret = "";

        public AuthenticationSession session;
        public int lastResult;

        // 0 = failed, 1 = member, 2 = admin, 3 = already admin.
        public bool Authenticate(string code)
        {
            lastResult = 0;

            if (session == null)
            {
                Debug.LogError("[TOTP] AuthenticationSession reference is missing.");
                return false;
            }

            if (code == null || code.Length != 6)
                return false;

            for (int i = 0; i < 6; i++)
            {
                if (code[i] < '0' || code[i] > '9')
                    return false;
            }

            BaseRank currentRank = session.rank;
            Debug.Log("[TOTP] Current rank before authentication: " + currentRank);

            if (currentRank == BaseRank.Admin)
            {
                lastResult = 3;
                Debug.Log("[TOTP] Player is already Admin.");
                return false;
            }

            long step =
                (Networking.GetNetworkDateTime().Ticks - 621355968000000000L)
                / 300000000L;

            // Admin has priority over Member.
            if (Match(adminTotpSecret, code, step))
            {
                Debug.Log("[TOTP] Admin TOTP matched. Applying Admin rank...");

                // Apply the rank directly in this same UdonBehaviour execution path.
                // This avoids relying on a cross-Udon method call for the critical
                // local rank mutation.
                session.rank = BaseRank.Admin;
                session.authenticatedTicks =
                    Networking.GetNetworkDateTime().Ticks;

                if (session.registry != null)
                    session.registry.PublishRank(BaseRank.Admin);

                // Do not report success until the shared session actually changed.
                if (session.rank != BaseRank.Admin)
                {
                    Debug.LogError(
                        "[TOTP] TOTP matched, but AuthenticationSession.rank did not become Admin. " +
                        "Check that TOTPAuthManager.session is the live AuthenticationSession used by the permission system."
                    );
                    lastResult = 0;
                    return false;
                }

                lastResult = 2;
                Debug.Log("[TOTP] Admin authentication complete. Current rank: " + session.rank);
                return true;
            }

            if (Match(memberTotpSecret, code, step))
            {
                Debug.Log("[TOTP] Member TOTP matched. Applying Member rank...");

                // Apply Member rank directly for the same reason as Admin.
                session.rank = BaseRank.Member;
                session.authenticatedTicks =
                    Networking.GetNetworkDateTime().Ticks;

                if (session.registry != null)
                    session.registry.PublishRank(BaseRank.Member);

                if (session.rank != BaseRank.Member)
                {
                    Debug.LogError(
                        "[TOTP] TOTP matched, but AuthenticationSession.rank did not become Member. " +
                        "Check that TOTPAuthManager.session is the live AuthenticationSession used by the permission system."
                    );
                    lastResult = 0;
                    return false;
                }

                lastResult = 1;
                Debug.Log("[TOTP] Member authentication complete. Current rank: " + session.rank);
                return true;
            }

            Debug.Log("[TOTP] TOTP code did not match.");
            return false;
        }

        private bool Match(string secret, string code, long step)
        {
            if (string.IsNullOrEmpty(secret))
                return false;

            byte[] key = DecodeBase32(secret);

            if (key.Length == 0)
                return false;

            for (long offset = -1L; offset <= 1L; offset++)
            {
                if (Compute(key, step + offset) == code)
                    return true;
            }

            return false;
        }

        private byte[] DecodeBase32(string value)
        {
            string clean = value.ToUpper().Replace(" ", "").Replace("=", "");

            if (clean.Length == 0)
                return new byte[0];

            byte[] bytes = new byte[clean.Length * 5 / 8];

            int buffer = 0;
            int bits = 0;
            int index = 0;

            for (int i = 0; i < clean.Length; i++)
            {
                char c = clean[i];
                int n;

                if (c >= 'A' && c <= 'Z')
                    n = c - 'A';
                else if (c >= '2' && c <= '7')
                    n = c - '2' + 26;
                else
                    return new byte[0];

                buffer = (buffer << 5) | n;
                bits += 5;

                if (bits >= 8)
                {
                    bits -= 8;
                    bytes[index++] = (byte)(buffer >> bits);

                    if (bits == 0)
                        buffer = 0;
                    else
                        buffer &= (1 << bits) - 1;
                }
            }

            return bytes;
        }

        private string Compute(byte[] key, long step)
        {
            byte[] inner = new byte[72];
            byte[] outer = new byte[84];

            if (key.Length > 64)
                key = Sha1(key);

            for (int i = 0; i < 64; i++)
            {
                byte k = i < key.Length ? key[i] : (byte)0;
                inner[i] = (byte)(k ^ 0x36);
                outer[i] = (byte)(k ^ 0x5C);
            }

            // Encode the 64-bit TOTP counter as big-endian bytes.
            // Mask to the low 8 bits before converting to byte because Udon
            // checks numeric narrowing conversions for overflow.
            for (int i = 0; i < 8; i++)
            {
                inner[64 + i] = LongLowByte(step >> (56 - i * 8));
            }

            byte[] digest = Sha1(inner);

            for (int i = 0; i < 20; i++)
                outer[64 + i] = digest[i];

            digest = Sha1(outer);

            int offset = digest[19] & 15;

            int number =
                ((digest[offset] & 127) << 24) |
                (digest[offset + 1] << 16) |
                (digest[offset + 2] << 8) |
                digest[offset + 3];

            string digits = (number % 1000000).ToString();

            while (digits.Length < 6)
                digits = "0" + digits;

            return digits;
        }

        private byte LongLowByte(long value)
        {
            return (byte)(value & 255L);
        }

        private byte UIntLowByte(uint value)
        {
            return (byte)(value & 255u);
        }

        private byte[] Sha1(byte[] data)
        {
            int padded = ((data.Length + 9 + 63) / 64) * 64;
            byte[] input = new byte[padded];

            for (int i = 0; i < data.Length; i++)
                input[i] = data[i];

            input[data.Length] = 0x80;

            long bitLength = (long)data.Length * 8L;

            for (int i = 0; i < 8; i++)
            {
                input[padded - 8 + i] =
                    LongLowByte(bitLength >> (56 - i * 8));
            }

            uint h0 = 0x67452301u;
            uint h1 = 0xEFCDAB89u;
            uint h2 = 0x98BADCFEu;
            uint h3 = 0x10325476u;
            uint h4 = 0xC3D2E1F0u;

            uint[] w = new uint[80];

            for (int block = 0; block < padded; block += 64)
            {
                for (int i = 0; i < 16; i++)
                {
                    int p = block + i * 4;

                    w[i] =
                        ((uint)input[p] << 24) |
                        ((uint)input[p + 1] << 16) |
                        ((uint)input[p + 2] << 8) |
                        input[p + 3];
                }

                for (int i = 16; i < 80; i++)
                {
                    uint x =
                        w[i - 3] ^
                        w[i - 8] ^
                        w[i - 14] ^
                        w[i - 16];

                    w[i] = (x << 1) | (x >> 31);
                }

                uint a = h0;
                uint b = h1;
                uint c = h2;
                uint d = h3;
                uint e = h4;

                for (int i = 0; i < 80; i++)
                {
                    uint f;
                    uint k;

                    if (i < 20)
                    {
                        f = (b & c) | (~b & d);
                        k = 0x5A827999u;
                    }
                    else if (i < 40)
                    {
                        f = b ^ c ^ d;
                        k = 0x6ED9EBA1u;
                    }
                    else if (i < 60)
                    {
                        f = (b & c) | (b & d) | (c & d);
                        k = 0x8F1BBCDCu;
                    }
                    else
                    {
                        f = b ^ c ^ d;
                        k = 0xCA62C1D6u;
                    }

                    uint next =
                        ((a << 5) | (a >> 27)) +
                        f +
                        e +
                        k +
                        w[i];

                    e = d;
                    d = c;
                    c = (b << 30) | (b >> 2);
                    b = a;
                    a = next;
                }

                h0 += a;
                h1 += b;
                h2 += c;
                h3 += d;
                h4 += e;
            }

            uint[] hashes = new uint[5];
            hashes[0] = h0;
            hashes[1] = h1;
            hashes[2] = h2;
            hashes[3] = h3;
            hashes[4] = h4;

            byte[] result = new byte[20];

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    result[i * 4 + j] =
                        UIntLowByte(hashes[i] >> (24 - 8 * j));
                }
            }

            return result;
        }
    }
}
