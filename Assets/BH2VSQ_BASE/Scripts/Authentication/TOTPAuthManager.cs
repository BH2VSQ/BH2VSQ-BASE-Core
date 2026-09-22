using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace BH2VSQ.Base
{
    // RFC 6238 / HMAC-SHA1.
    // Note: TOTP secrets embedded in an uploaded world are not private.
    public class TOTPAuthManager : UdonSharpBehaviour
    {
        [SerializeField] private string memberTotpSecret = "";
        [SerializeField] private string adminTotpSecret = "";

        public AuthenticationSession session;
        public int lastResult;

        public bool Authenticate(string code)
        {
            lastResult = 0;

            if (session == null)
                return false;

            if (code == null || code.Length != 6)
                return false;

            // Only allow a 6-digit numeric code.
            for (int i = 0; i < 6; i++)
            {
                if (code[i] < '0' || code[i] > '9')
                    return false;
            }

            // Already admin.
            if (session.rank == BaseRank.Admin)
            {
                lastResult = 3;
                return false;
            }

            // 30-second TOTP period.
            long step =
                (Networking.GetNetworkDateTime().Ticks - 621355968000000000L)
                / 300000000L;

            // Check admin first.
            if (Match(adminTotpSecret, code, step))
            {
                session.Authenticate(BaseRank.Admin);
                lastResult = 2;
                return true;
            }

            // Then check member.
            if (Match(memberTotpSecret, code, step))
            {
                session.Authenticate(BaseRank.Member);
                lastResult = 1;
                return true;
            }

            return false;
        }

        private bool Match(string secret, string code, long step)
        {
            if (string.IsNullOrEmpty(secret))
                return false;

            byte[] key = DecodeBase32(secret);

            if (key.Length == 0)
                return false;

            // Allow one 30-second step of clock skew in either direction.
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
                {
                    n = c - 'A';
                }
                else if (c >= '2' && c <= '7')
                {
                    n = c - '2' + 26;
                }
                else
                {
                    return new byte[0];
                }

                buffer = (buffer << 5) | n;
                bits += 5;

                if (bits >= 8)
                {
                    bits -= 8;

                    // Exactly 8 useful bits remain after the shift.
                    // Therefore this conversion is already in the byte range.
                    bytes[index++] = (byte)(buffer >> bits);

                    if (bits == 0)
                    {
                        buffer = 0;
                    }
                    else
                    {
                        buffer &= (1 << bits) - 1;
                    }
                }
            }

            return bytes;
        }

        private string Compute(byte[] key, long step)
        {
            byte[] inner = new byte[72];
            byte[] outer = new byte[84];

            // HMAC-SHA1 keys longer than the block size are hashed first.
            if (key.Length > 64)
                key = Sha1(key);

            // HMAC inner/outer pads.
            for (int i = 0; i < 64; i++)
            {
                byte k = i < key.Length ? key[i] : (byte)0;

                inner[i] = (byte)(k ^ 0x36);
                outer[i] = (byte)(k ^ 0x5C);
            }

            // TOTP/HOTP moving factor:
            // encode the 64-bit counter as big-endian bytes.
            //
            // IMPORTANT:
            // UdonSharp checks numeric narrowing conversions.
            // Masking to 8 bits before converting long -> byte
            // prevents the OverflowException seen in the original code.
            for (int i = 0; i < 8; i++)
            {
                inner[64 + i] =
                    LongLowByte(step >> (56 - i * 8));
            }

            // HMAC inner hash.
            byte[] digest = Sha1(inner);

            // Add inner hash to outer input.
            for (int i = 0; i < 20; i++)
                outer[64 + i] = digest[i];

            // HMAC outer hash.
            digest = Sha1(outer);

            // RFC 4226 dynamic truncation.
            int offset = digest[19] & 15;

            int number =
                ((digest[offset] & 127) << 24) |
                (digest[offset + 1] << 16) |
                (digest[offset + 2] << 8) |
                digest[offset + 3];

            // 6-digit TOTP.
            string digits = (number % 1000000).ToString();

            while (digits.Length < 6)
                digits = "0" + digits;

            return digits;
        }

        // UdonSharp performs checked numeric casts.
        // Masking first guarantees that the long value is 0..255.
        private byte LongLowByte(long value)
        {
            return (byte)(value & 255L);
        }

        // Same idea for uint -> byte.
        private byte UIntLowByte(uint value)
        {
            return (byte)(value & 255u);
        }

        private byte[] Sha1(byte[] data)
        {
            // SHA-1 padding:
            // original data + 0x80 + zero padding + 64-bit bit length.
            int padded =
                ((data.Length + 9 + 63) / 64) * 64;

            byte[] input = new byte[padded];

            for (int i = 0; i < data.Length; i++)
                input[i] = data[i];

            input[data.Length] = 0x80;

            long bitLength = (long)data.Length * 8L;

            // Write the 64-bit big-endian bit length.
            //
            // The original implementation did:
            // (byte)(bitLength >> ...)
            //
            // That is another potential Udon overflow.
            for (int i = 0; i < 8; i++)
            {
                input[padded - 8 + i] =
                    LongLowByte(bitLength >> (56 - i * 8));
            }

            // SHA-1 initial state.
            uint h0 = 0x67452301u;
            uint h1 = 0xEFCDAB89u;
            uint h2 = 0x98BADCFEu;
            uint h3 = 0x10325476u;
            uint h4 = 0xC3D2E1F0u;

            uint[] w = new uint[80];

            // Process every 512-bit block.
            for (int block = 0; block < padded; block += 64)
            {
                // First 16 words.
                for (int i = 0; i < 16; i++)
                {
                    int p = block + i * 4;

                    w[i] =
                        ((uint)input[p] << 24) |
                        ((uint)input[p + 1] << 16) |
                        ((uint)input[p + 2] << 8) |
                        input[p + 3];
                }

                // Extend to 80 words.
                for (int i = 16; i < 80; i++)
                {
                    uint x =
                        w[i - 3] ^
                        w[i - 8] ^
                        w[i - 14] ^
                        w[i - 16];

                    w[i] =
                        (x << 1) |
                        (x >> 31);
                }

                uint a = h0;
                uint b = h1;
                uint c = h2;
                uint d = h3;
                uint e = h4;

                // Main SHA-1 compression loop.
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

                // SHA-1 state accumulation.
                h0 += a;
                h1 += b;
                h2 += c;
                h3 += d;
                h4 += e;
            }

            // Store the final five 32-bit words.
            uint[] hashes = new uint[5];

            hashes[0] = h0;
            hashes[1] = h1;
            hashes[2] = h2;
            hashes[3] = h3;
            hashes[4] = h4;

            byte[] result = new byte[20];

            // Convert SHA-1 words to big-endian bytes.
            //
            // Original code:
            // (byte)(hashes[i] >> ...)
            //
            // This can overflow in Udon because the shifted uint
            // is not necessarily <= 255.
            //
            // Mask first so the final conversion is always safe.
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    result[i * 4 + j] =
                        UIntLowByte(
                            hashes[i] >> (24 - 8 * j)
                        );
                }
            }

            return result;
        }
    }
}