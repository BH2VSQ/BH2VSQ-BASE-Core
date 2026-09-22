using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace BH2VSQ.Base
{
    // RFC 6238 / HMAC-SHA1. Secrets in uploaded worlds are not private; see SECURITY.md.
    public class TOTPAuthManager : UdonSharpBehaviour
    {
        [SerializeField] private string memberTotpSecret = "";
        [SerializeField] private string adminTotpSecret = "";
        public AuthenticationSession session;
        public int lastResult;

        public bool Authenticate(string code)
        {
            lastResult = 0;
            if (session == null || code == null || code.Length != 6) return false;
            if (session.rank == BaseRank.Admin) { lastResult = 3; return false; }
            long step = (Networking.GetNetworkDateTime().Ticks - 621355968000000000L) / 300000000L;
            if (Match(adminTotpSecret, code, step)) { session.Authenticate(BaseRank.Admin); lastResult = 2; return true; }
            if (Match(memberTotpSecret, code, step)) { session.Authenticate(BaseRank.Member); lastResult = 1; return true; }
            return false;
        }

        private bool Match(string secret, string code, long step)
        {
            if (string.IsNullOrEmpty(secret)) return false;
            byte[] key = DecodeBase32(secret);
            if (key.Length == 0) return false;
            for (long offset = -1; offset <= 1; offset++)
                if (Compute(key, step + offset) == code) return true;
            return false;
        }

        private byte[] DecodeBase32(string value)
        {
            string clean = value.ToUpper().Replace(" ", "").Replace("=", "");
            byte[] bytes = new byte[clean.Length * 5 / 8];
            int buffer = 0, bits = 0, index = 0;
            for (int i = 0; i < clean.Length; i++)
            {
                char c = clean[i];
                int n = c >= 'A' && c <= 'Z' ? c - 'A' : c >= '2' && c <= '7' ? c - '2' + 26 : -1;
                if (n < 0) return new byte[0];
                buffer = (buffer << 5) | n;
                bits += 5;
                if (bits >= 8)
                {
                    bits -= 8;
                    bytes[index++] = (byte)(buffer >> bits);
                    buffer &= (1 << bits) - 1;
                }
            }
            return bytes;
        }

        private string Compute(byte[] key, long step)
        {
            byte[] inner = new byte[72];
            byte[] outer = new byte[84];
            if (key.Length > 64) key = Sha1(key);
            for (int i = 0; i < 64; i++)
            {
                byte k = i < key.Length ? key[i] : (byte)0;
                inner[i] = (byte)(k ^ 0x36);
                outer[i] = (byte)(k ^ 0x5c);
            }
            for (int i = 0; i < 8; i++) inner[64 + i] = (byte)(step >> (56 - i * 8));
            byte[] digest = Sha1(inner);
            for (int i = 0; i < 20; i++) outer[64 + i] = digest[i];
            digest = Sha1(outer);
            int offset = digest[19] & 15;
            int number = ((digest[offset] & 127) << 24) | (digest[offset + 1] << 16) | (digest[offset + 2] << 8) | digest[offset + 3];
            string digits = (number % 1000000).ToString();
            while (digits.Length < 6) digits = "0" + digits;
            return digits;
        }

        private byte[] Sha1(byte[] data)
        {
            int padded = ((data.Length + 9 + 63) / 64) * 64;
            byte[] input = new byte[padded];
            for (int i = 0; i < data.Length; i++) input[i] = data[i];
            input[data.Length] = 0x80;
            long bitLength = (long)data.Length * 8;
            for (int i = 0; i < 8; i++) input[padded - 8 + i] = (byte)(bitLength >> (56 - i * 8));
            uint h0 = 0x67452301, h1 = 0xEFCDAB89, h2 = 0x98BADCFE, h3 = 0x10325476, h4 = 0xC3D2E1F0;
            uint[] w = new uint[80];
            for (int block = 0; block < padded; block += 64)
            {
                for (int i = 0; i < 16; i++)
                {
                    int p = block + i * 4;
                    w[i] = ((uint)input[p] << 24) | ((uint)input[p + 1] << 16) | ((uint)input[p + 2] << 8) | input[p + 3];
                }
                for (int i = 16; i < 80; i++)
                {
                    uint x = w[i - 3] ^ w[i - 8] ^ w[i - 14] ^ w[i - 16];
                    w[i] = (x << 1) | (x >> 31);
                }
                uint a = h0, b = h1, c = h2, d = h3, e = h4;
                for (int i = 0; i < 80; i++)
                {
                    uint f, k;
                    if (i < 20) { f = (b & c) | (~b & d); k = 0x5A827999; }
                    else if (i < 40) { f = b ^ c ^ d; k = 0x6ED9EBA1; }
                    else if (i < 60) { f = (b & c) | (b & d) | (c & d); k = 0x8F1BBCDC; }
                    else { f = b ^ c ^ d; k = 0xCA62C1D6; }
                    uint next = ((a << 5) | (a >> 27)) + f + e + k + w[i];
                    e = d; d = c; c = (b << 30) | (b >> 2); b = a; a = next;
                }
                h0 += a; h1 += b; h2 += c; h3 += d; h4 += e;
            }
            uint[] hashes = { h0, h1, h2, h3, h4 };
            byte[] result = new byte[20];
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 4; j++) result[i * 4 + j] = (byte)(hashes[i] >> (24 - 8 * j));
            return result;
        }
    }
}
