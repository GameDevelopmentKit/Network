#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Math.EC.Rfc7748
{
    using System.Diagnostics;
    using BestHTTP.SecureProtocol.Org.BouncyCastle.Math.EC.Rfc8032;
    using BestHTTP.SecureProtocol.Org.BouncyCastle.Security;
    using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities;

    public abstract class X25519
    {
        public const int PointSize = 32;
        public const int ScalarSize = 32;

        private class F : X25519Field
        {
        }

        private const int C_A = 486662;
        private const int C_A24 = (C_A + 2)/4;

        //private static readonly int[] SqrtNeg486664 = { 0x03457E06, 0x03812ABF, 0x01A82CC6, 0x028A5BE8, 0x018B43A7,
        //    0x03FC4F7E, 0x02C23700, 0x006BBD27, 0x03A30500, 0x001E4DDB };

        public static bool CalculateAgreement(byte[] k, int kOff, byte[] u, int uOff, byte[] r, int rOff)
        {
            ScalarMult(k, kOff, u, uOff, r, rOff);
            return !Arrays.AreAllZeroes(r, rOff, PointSize);
        }

        private static uint Decode32(byte[] bs, int off)
        {
            uint n = bs[off];
            n |= (uint)bs[++off] << 8;
            n |= (uint)bs[++off] << 16;
            n |= (uint)bs[++off] << 24;
            return n;
        }

        private static void DecodeScalar(byte[] k, int kOff, uint[] n)
        {
            for (int i = 0; i < 8; ++i)
            {
                n[i] = Decode32(k, kOff + i * 4);
            }

            n[0] &= 0xFFFFFFF8U;
            n[7] &= 0x7FFFFFFFU;
            n[7] |= 0x40000000U;
        }

        public static void GeneratePrivateKey(SecureRandom random, byte[] k)
        {
            random.NextBytes(k);

            k[0] &= 0xF8;
            k[ScalarSize - 1] &= 0x7F;
            k[ScalarSize - 1] |= 0x40;
        }

        public static void GeneratePublicKey(byte[] k, int kOff, byte[] r, int rOff)
        {
            ScalarMultBase(k, kOff, r, rOff);
        }

        private static void PointDouble(int[] x, int[] z)
        {
            var a = X25519Field.Create();
            var b = X25519Field.Create();

            X25519Field.Apm(x, z, a, b);
            X25519Field.Sqr(a, a);
            X25519Field.Sqr(b, b);
            X25519Field.Mul(a, b, x);
            X25519Field.Sub(a, b, a);
            X25519Field.Mul(a, C_A24, z);
            X25519Field.Add(z, b, z);
            X25519Field.Mul(z, a, z);
        }

        public static void Precompute()
        {
            Ed25519.Precompute();
        }

        public static void ScalarMult(byte[] k, int kOff, byte[] u, int uOff, byte[] r, int rOff)
        {
            uint[] n = new uint[8];     DecodeScalar(k, kOff, n);

            var x1 = X25519Field.Create();
            X25519Field.Decode(u, uOff, x1);
            var x2 = X25519Field.Create();
            X25519Field.Copy(x1, 0, x2, 0);
            var z2 = X25519Field.Create();
            z2[0] = 1;
            var x3 = X25519Field.Create();
            x3[0] = 1;
            var z3 = X25519Field.Create();

            var t1 = X25519Field.Create();
            var t2 = X25519Field.Create();

            Debug.Assert(n[7] >> 30 == 1U);

            int bit = 254, swap = 1;
            do
            {
                X25519Field.Apm(x3, z3, t1, x3);
                X25519Field.Apm(x2, z2, z3, x2);
                X25519Field.Mul(t1, x2, t1);
                X25519Field.Mul(x3, z3, x3);
                X25519Field.Sqr(z3, z3);
                X25519Field.Sqr(x2, x2);

                X25519Field.Sub(z3, x2, t2);
                X25519Field.Mul(t2, C_A24, z2);
                X25519Field.Add(z2, x2, z2);
                X25519Field.Mul(z2, t2, z2);
                X25519Field.Mul(x2, z3, x2);

                X25519Field.Apm(t1, x3, x3, z3);
                X25519Field.Sqr(x3, x3);
                X25519Field.Sqr(z3, z3);
                X25519Field.Mul(z3, x1, z3);

                --bit;

                int word = bit >> 5, shift = bit & 0x1F;
                int kt = (int)(n[word] >> shift) & 1;
                swap ^= kt;
                X25519Field.CSwap(swap, x2, x3);
                X25519Field.CSwap(swap, z2, z3);
                swap = kt;
            }
            while (bit >= 3);

            Debug.Assert(swap == 0);

            for (int i = 0; i < 3; ++i)
            {
                PointDouble(x2, z2);
            }

            X25519Field.Inv(z2, z2);
            X25519Field.Mul(x2, z2, x2);

            X25519Field.Normalize(x2);
            X25519Field.Encode(x2, r, rOff);
        }

        public static void ScalarMultBase(byte[] k, int kOff, byte[] r, int rOff)
        {
            var y = X25519Field.Create();
            var z = X25519Field.Create();

            Ed25519.ScalarMultBaseYZ(k, kOff, y, z);

            X25519Field.Apm(z, y, y, z);

            X25519Field.Inv(z, z);
            X25519Field.Mul(y, z, y);

            X25519Field.Normalize(y);
            X25519Field.Encode(y, r, rOff);
        }
    }
}
#pragma warning restore
#endif
