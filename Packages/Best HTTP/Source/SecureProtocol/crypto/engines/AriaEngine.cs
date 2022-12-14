#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    using System;
    using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;
    using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities;
    using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Encoders;

    /**
     * RFC 5794.
     * 
     * ARIA is a 128-bit block cipher with 128-, 192-, and 256-bit keys.
     */
    public class AriaEngine
        : IBlockCipher
    {
        private static readonly byte[][] C =
        {
            Hex.DecodeStrict("517cc1b727220a94fe13abe8fa9a6ee0"),
            Hex.DecodeStrict("6db14acc9e21c820ff28b1d5ef5de2b0"), Hex.DecodeStrict("db92371d2126e9700324977504e8c90e")
        };

        private static readonly byte[] SB1_sbox =
        {
            0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b,
            0x6f, 0xc5, 0x30, 0x01, 0x67, 0x2b, 0xfe, 0xd7, 0xab,
            0x76, 0xca, 0x82, 0xc9, 0x7d, 0xfa, 0x59, 0x47, 0xf0,
            0xad, 0xd4, 0xa2, 0xaf, 0x9c, 0xa4, 0x72, 0xc0, 0xb7,
            0xfd, 0x93, 0x26, 0x36, 0x3f, 0xf7, 0xcc, 0x34, 0xa5,
            0xe5, 0xf1, 0x71, 0xd8, 0x31, 0x15, 0x04, 0xc7, 0x23,
            0xc3, 0x18, 0x96, 0x05, 0x9a, 0x07, 0x12, 0x80, 0xe2,
            0xeb, 0x27, 0xb2, 0x75, 0x09, 0x83, 0x2c, 0x1a, 0x1b,
            0x6e, 0x5a, 0xa0, 0x52, 0x3b, 0xd6, 0xb3, 0x29, 0xe3,
            0x2f, 0x84, 0x53, 0xd1, 0x00, 0xed, 0x20, 0xfc, 0xb1,
            0x5b, 0x6a, 0xcb, 0xbe, 0x39, 0x4a, 0x4c, 0x58, 0xcf,
            0xd0, 0xef, 0xaa, 0xfb, 0x43, 0x4d, 0x33, 0x85, 0x45,
            0xf9, 0x02, 0x7f, 0x50, 0x3c, 0x9f, 0xa8, 0x51, 0xa3,
            0x40, 0x8f, 0x92, 0x9d, 0x38, 0xf5, 0xbc, 0xb6, 0xda,
            0x21, 0x10, 0xff, 0xf3, 0xd2, 0xcd, 0x0c, 0x13, 0xec,
            0x5f, 0x97, 0x44, 0x17, 0xc4, 0xa7, 0x7e, 0x3d, 0x64,
            0x5d, 0x19, 0x73, 0x60, 0x81, 0x4f, 0xdc, 0x22, 0x2a,
            0x90, 0x88, 0x46, 0xee, 0xb8, 0x14, 0xde, 0x5e, 0x0b,
            0xdb, 0xe0, 0x32, 0x3a, 0x0a, 0x49, 0x06, 0x24, 0x5c,
            0xc2, 0xd3, 0xac, 0x62, 0x91, 0x95, 0xe4, 0x79, 0xe7,
            0xc8, 0x37, 0x6d, 0x8d, 0xd5, 0x4e, 0xa9, 0x6c, 0x56,
            0xf4, 0xea, 0x65, 0x7a, 0xae, 0x08, 0xba, 0x78, 0x25,
            0x2e, 0x1c, 0xa6, 0xb4, 0xc6, 0xe8, 0xdd, 0x74, 0x1f,
            0x4b, 0xbd, 0x8b, 0x8a, 0x70, 0x3e, 0xb5, 0x66, 0x48,
            0x03, 0xf6, 0x0e, 0x61, 0x35, 0x57, 0xb9, 0x86, 0xc1,
            0x1d, 0x9e, 0xe1, 0xf8, 0x98, 0x11, 0x69, 0xd9, 0x8e,
            0x94, 0x9b, 0x1e, 0x87, 0xe9, 0xce, 0x55, 0x28, 0xdf,
            0x8c, 0xa1, 0x89, 0x0d, 0xbf, 0xe6, 0x42, 0x68, 0x41,
            0x99, 0x2d, 0x0f, 0xb0, 0x54, 0xbb, 0x16
        };

        private static readonly byte[] SB2_sbox =
        {
            0xe2, 0x4e, 0x54, 0xfc, 0x94, 0xc2,
            0x4a, 0xcc, 0x62, 0x0d, 0x6a, 0x46, 0x3c, 0x4d, 0x8b,
            0xd1, 0x5e, 0xfa, 0x64, 0xcb, 0xb4, 0x97, 0xbe, 0x2b,
            0xbc, 0x77, 0x2e, 0x03, 0xd3, 0x19, 0x59, 0xc1, 0x1d,
            0x06, 0x41, 0x6b, 0x55, 0xf0, 0x99, 0x69, 0xea, 0x9c,
            0x18, 0xae, 0x63, 0xdf, 0xe7, 0xbb, 0x00, 0x73, 0x66,
            0xfb, 0x96, 0x4c, 0x85, 0xe4, 0x3a, 0x09, 0x45, 0xaa,
            0x0f, 0xee, 0x10, 0xeb, 0x2d, 0x7f, 0xf4, 0x29, 0xac,
            0xcf, 0xad, 0x91, 0x8d, 0x78, 0xc8, 0x95, 0xf9, 0x2f,
            0xce, 0xcd, 0x08, 0x7a, 0x88, 0x38, 0x5c, 0x83, 0x2a,
            0x28, 0x47, 0xdb, 0xb8, 0xc7, 0x93, 0xa4, 0x12, 0x53,
            0xff, 0x87, 0x0e, 0x31, 0x36, 0x21, 0x58, 0x48, 0x01,
            0x8e, 0x37, 0x74, 0x32, 0xca, 0xe9, 0xb1, 0xb7, 0xab,
            0x0c, 0xd7, 0xc4, 0x56, 0x42, 0x26, 0x07, 0x98, 0x60,
            0xd9, 0xb6, 0xb9, 0x11, 0x40, 0xec, 0x20, 0x8c, 0xbd,
            0xa0, 0xc9, 0x84, 0x04, 0x49, 0x23, 0xf1, 0x4f, 0x50,
            0x1f, 0x13, 0xdc, 0xd8, 0xc0, 0x9e, 0x57, 0xe3, 0xc3,
            0x7b, 0x65, 0x3b, 0x02, 0x8f, 0x3e, 0xe8, 0x25, 0x92,
            0xe5, 0x15, 0xdd, 0xfd, 0x17, 0xa9, 0xbf, 0xd4, 0x9a,
            0x7e, 0xc5, 0x39, 0x67, 0xfe, 0x76, 0x9d, 0x43, 0xa7,
            0xe1, 0xd0, 0xf5, 0x68, 0xf2, 0x1b, 0x34, 0x70, 0x05,
            0xa3, 0x8a, 0xd5, 0x79, 0x86, 0xa8, 0x30, 0xc6, 0x51,
            0x4b, 0x1e, 0xa6, 0x27, 0xf6, 0x35, 0xd2, 0x6e, 0x24,
            0x16, 0x82, 0x5f, 0xda, 0xe6, 0x75, 0xa2, 0xef, 0x2c,
            0xb2, 0x1c, 0x9f, 0x5d, 0x6f, 0x80, 0x0a, 0x72, 0x44,
            0x9b, 0x6c, 0x90, 0x0b, 0x5b, 0x33, 0x7d, 0x5a, 0x52,
            0xf3, 0x61, 0xa1, 0xf7, 0xb0, 0xd6, 0x3f, 0x7c, 0x6d,
            0xed, 0x14, 0xe0, 0xa5, 0x3d, 0x22, 0xb3, 0xf8, 0x89,
            0xde, 0x71, 0x1a, 0xaf, 0xba, 0xb5, 0x81
        };

        private static readonly byte[] SB3_sbox =
        {
            0x52, 0x09, 0x6a, 0xd5, 0x30, 0x36,
            0xa5, 0x38, 0xbf, 0x40, 0xa3, 0x9e, 0x81, 0xf3, 0xd7,
            0xfb, 0x7c, 0xe3, 0x39, 0x82, 0x9b, 0x2f, 0xff, 0x87,
            0x34, 0x8e, 0x43, 0x44, 0xc4, 0xde, 0xe9, 0xcb, 0x54,
            0x7b, 0x94, 0x32, 0xa6, 0xc2, 0x23, 0x3d, 0xee, 0x4c,
            0x95, 0x0b, 0x42, 0xfa, 0xc3, 0x4e, 0x08, 0x2e, 0xa1,
            0x66, 0x28, 0xd9, 0x24, 0xb2, 0x76, 0x5b, 0xa2, 0x49,
            0x6d, 0x8b, 0xd1, 0x25, 0x72, 0xf8, 0xf6, 0x64, 0x86,
            0x68, 0x98, 0x16, 0xd4, 0xa4, 0x5c, 0xcc, 0x5d, 0x65,
            0xb6, 0x92, 0x6c, 0x70, 0x48, 0x50, 0xfd, 0xed, 0xb9,
            0xda, 0x5e, 0x15, 0x46, 0x57, 0xa7, 0x8d, 0x9d, 0x84,
            0x90, 0xd8, 0xab, 0x00, 0x8c, 0xbc, 0xd3, 0x0a, 0xf7,
            0xe4, 0x58, 0x05, 0xb8, 0xb3, 0x45, 0x06, 0xd0, 0x2c,
            0x1e, 0x8f, 0xca, 0x3f, 0x0f, 0x02, 0xc1, 0xaf, 0xbd,
            0x03, 0x01, 0x13, 0x8a, 0x6b, 0x3a, 0x91, 0x11, 0x41,
            0x4f, 0x67, 0xdc, 0xea, 0x97, 0xf2, 0xcf, 0xce, 0xf0,
            0xb4, 0xe6, 0x73, 0x96, 0xac, 0x74, 0x22, 0xe7, 0xad,
            0x35, 0x85, 0xe2, 0xf9, 0x37, 0xe8, 0x1c, 0x75, 0xdf,
            0x6e, 0x47, 0xf1, 0x1a, 0x71, 0x1d, 0x29, 0xc5, 0x89,
            0x6f, 0xb7, 0x62, 0x0e, 0xaa, 0x18, 0xbe, 0x1b, 0xfc,
            0x56, 0x3e, 0x4b, 0xc6, 0xd2, 0x79, 0x20, 0x9a, 0xdb,
            0xc0, 0xfe, 0x78, 0xcd, 0x5a, 0xf4, 0x1f, 0xdd, 0xa8,
            0x33, 0x88, 0x07, 0xc7, 0x31, 0xb1, 0x12, 0x10, 0x59,
            0x27, 0x80, 0xec, 0x5f, 0x60, 0x51, 0x7f, 0xa9, 0x19,
            0xb5, 0x4a, 0x0d, 0x2d, 0xe5, 0x7a, 0x9f, 0x93, 0xc9,
            0x9c, 0xef, 0xa0, 0xe0, 0x3b, 0x4d, 0xae, 0x2a, 0xf5,
            0xb0, 0xc8, 0xeb, 0xbb, 0x3c, 0x83, 0x53, 0x99, 0x61,
            0x17, 0x2b, 0x04, 0x7e, 0xba, 0x77, 0xd6, 0x26, 0xe1,
            0x69, 0x14, 0x63, 0x55, 0x21, 0x0c, 0x7d
        };

        private static readonly byte[] SB4_sbox =
        {
            0x30, 0x68, 0x99, 0x1b, 0x87, 0xb9,
            0x21, 0x78, 0x50, 0x39, 0xdb, 0xe1, 0x72, 0x9, 0x62,
            0x3c, 0x3e, 0x7e, 0x5e, 0x8e, 0xf1, 0xa0, 0xcc, 0xa3,
            0x2a, 0x1d, 0xfb, 0xb6, 0xd6, 0x20, 0xc4, 0x8d, 0x81,
            0x65, 0xf5, 0x89, 0xcb, 0x9d, 0x77, 0xc6, 0x57, 0x43,
            0x56, 0x17, 0xd4, 0x40, 0x1a, 0x4d, 0xc0, 0x63, 0x6c,
            0xe3, 0xb7, 0xc8, 0x64, 0x6a, 0x53, 0xaa, 0x38, 0x98,
            0x0c, 0xf4, 0x9b, 0xed, 0x7f, 0x22, 0x76, 0xaf, 0xdd,
            0x3a, 0x0b, 0x58, 0x67, 0x88, 0x06, 0xc3, 0x35, 0x0d,
            0x01, 0x8b, 0x8c, 0xc2, 0xe6, 0x5f, 0x02, 0x24, 0x75,
            0x93, 0x66, 0x1e, 0xe5, 0xe2, 0x54, 0xd8, 0x10, 0xce,
            0x7a, 0xe8, 0x08, 0x2c, 0x12, 0x97, 0x32, 0xab, 0xb4,
            0x27, 0x0a, 0x23, 0xdf, 0xef, 0xca, 0xd9, 0xb8, 0xfa,
            0xdc, 0x31, 0x6b, 0xd1, 0xad, 0x19, 0x49, 0xbd, 0x51,
            0x96, 0xee, 0xe4, 0xa8, 0x41, 0xda, 0xff, 0xcd, 0x55,
            0x86, 0x36, 0xbe, 0x61, 0x52, 0xf8, 0xbb, 0x0e, 0x82,
            0x48, 0x69, 0x9a, 0xe0, 0x47, 0x9e, 0x5c, 0x04, 0x4b,
            0x34, 0x15, 0x79, 0x26, 0xa7, 0xde, 0x29, 0xae, 0x92,
            0xd7, 0x84, 0xe9, 0xd2, 0xba, 0x5d, 0xf3, 0xc5, 0xb0,
            0xbf, 0xa4, 0x3b, 0x71, 0x44, 0x46, 0x2b, 0xfc, 0xeb,
            0x6f, 0xd5, 0xf6, 0x14, 0xfe, 0x7c, 0x70, 0x5a, 0x7d,
            0xfd, 0x2f, 0x18, 0x83, 0x16, 0xa5, 0x91, 0x1f, 0x05,
            0x95, 0x74, 0xa9, 0xc1, 0x5b, 0x4a, 0x85, 0x6d, 0x13,
            0x07, 0x4f, 0x4e, 0x45, 0xb2, 0x0f, 0xc9, 0x1c, 0xa6,
            0xbc, 0xec, 0x73, 0x90, 0x7b, 0xcf, 0x59, 0x8f, 0xa1,
            0xf9, 0x2d, 0xf2, 0xb1, 0x00, 0x94, 0x37, 0x9f, 0xd0,
            0x2e, 0x9c, 0x6e, 0x28, 0x3f, 0x80, 0xf0, 0x3d, 0xd3,
            0x25, 0x8a, 0xb5, 0xe7, 0x42, 0xb3, 0xc7, 0xea, 0xf7,
            0x4c, 0x11, 0x33, 0x03, 0xa2, 0xac, 0x60
        };

        protected const int BlockSize = 16;

        private byte[][] m_roundKeys;

        public virtual void Init(bool forEncryption, ICipherParameters parameters)
        {
            var keyParameter = parameters as KeyParameter;

            if (keyParameter == null)
                throw new ArgumentException("invalid parameter passed to ARIA init - "
                                            + Platform.GetTypeName(parameters));

            this.m_roundKeys = KeySchedule(forEncryption, keyParameter.GetKey());
        }

        public virtual string AlgorithmName => "ARIA";

        public virtual bool IsPartialBlockOkay => false;

        public virtual int GetBlockSize() { return BlockSize; }

        public virtual int ProcessBlock(byte[] input, int inOff, byte[] output, int outOff)
        {
            if (this.m_roundKeys == null)
                throw new InvalidOperationException("ARIA engine not initialised");

            Check.DataLength(input, inOff, BlockSize, "input buffer too short");
            Check.OutputLength(output, outOff, BlockSize, "output buffer too short");

            var z = new byte[BlockSize];
            Array.Copy(input, inOff, z, 0, BlockSize);

            int i = 0, rounds = this.m_roundKeys.Length - 3;
            while (i < rounds)
            {
                FO(z, this.m_roundKeys[i++]);
                FE(z, this.m_roundKeys[i++]);
            }

            FO(z, this.m_roundKeys[i++]);
            Xor(z, this.m_roundKeys[i++]);
            SL2(z);
            Xor(z, this.m_roundKeys[i]);

            Array.Copy(z, 0, output, outOff, BlockSize);

            return BlockSize;
        }

        public virtual void Reset()
        {
            // Empty
        }

        protected static void A(byte[] z)
        {
            byte x0 = z[0],
                x1  = z[1],
                x2  = z[2],
                x3  = z[3],
                x4  = z[4],
                x5  = z[5],
                x6  = z[6],
                x7  = z[7],
                x8  = z[8],
                x9  = z[9],
                x10 = z[10],
                x11 = z[11],
                x12 = z[12],
                x13 = z[13],
                x14 = z[14],
                x15 = z[15];

            z[0]  = (byte)(x3 ^ x4 ^ x6 ^ x8 ^ x9 ^ x13 ^ x14);
            z[1]  = (byte)(x2 ^ x5 ^ x7 ^ x8 ^ x9 ^ x12 ^ x15);
            z[2]  = (byte)(x1 ^ x4 ^ x6 ^ x10 ^ x11 ^ x12 ^ x15);
            z[3]  = (byte)(x0 ^ x5 ^ x7 ^ x10 ^ x11 ^ x13 ^ x14);
            z[4]  = (byte)(x0 ^ x2 ^ x5 ^ x8 ^ x11 ^ x14 ^ x15);
            z[5]  = (byte)(x1 ^ x3 ^ x4 ^ x9 ^ x10 ^ x14 ^ x15);
            z[6]  = (byte)(x0 ^ x2 ^ x7 ^ x9 ^ x10 ^ x12 ^ x13);
            z[7]  = (byte)(x1 ^ x3 ^ x6 ^ x8 ^ x11 ^ x12 ^ x13);
            z[8]  = (byte)(x0 ^ x1 ^ x4 ^ x7 ^ x10 ^ x13 ^ x15);
            z[9]  = (byte)(x0 ^ x1 ^ x5 ^ x6 ^ x11 ^ x12 ^ x14);
            z[10] = (byte)(x2 ^ x3 ^ x5 ^ x6 ^ x8 ^ x13 ^ x15);
            z[11] = (byte)(x2 ^ x3 ^ x4 ^ x7 ^ x9 ^ x12 ^ x14);
            z[12] = (byte)(x1 ^ x2 ^ x6 ^ x7 ^ x9 ^ x11 ^ x12);
            z[13] = (byte)(x0 ^ x3 ^ x6 ^ x7 ^ x8 ^ x10 ^ x13);
            z[14] = (byte)(x0 ^ x3 ^ x4 ^ x5 ^ x9 ^ x11 ^ x14);
            z[15] = (byte)(x1 ^ x2 ^ x4 ^ x5 ^ x8 ^ x10 ^ x15);
        }

        protected static void FE(byte[] D, byte[] RK)
        {
            Xor(D, RK);
            SL2(D);
            A(D);
        }

        protected static void FO(byte[] D, byte[] RK)
        {
            Xor(D, RK);
            SL1(D);
            A(D);
        }

        protected static byte[][] KeySchedule(bool forEncryption, byte[] K)
        {
            var keyLen = K.Length;
            if (keyLen < 16 || keyLen > 32 || (keyLen & 7) != 0)
                throw new ArgumentException("Key length not 128/192/256 bits.");

            var keyLenIdx = (keyLen >> 3) - 2;

            var CK1 = C[keyLenIdx];
            var CK2 = C[(keyLenIdx + 1) % 3];
            var CK3 = C[(keyLenIdx + 2) % 3];

            byte[] KL = new byte[16], KR = new byte[16];
            Array.Copy(K, 0, KL, 0, 16);
            Array.Copy(K, 16, KR, 0, keyLen - 16);

            var W0 = new byte[16];
            var W1 = new byte[16];
            var W2 = new byte[16];
            var W3 = new byte[16];

            Array.Copy(KL, 0, W0, 0, 16);

            Array.Copy(W0, 0, W1, 0, 16);
            FO(W1, CK1);
            Xor(W1, KR);

            Array.Copy(W1, 0, W2, 0, 16);
            FE(W2, CK2);
            Xor(W2, W0);

            Array.Copy(W2, 0, W3, 0, 16);
            FO(W3, CK3);
            Xor(W3, W1);

            var numRounds = 12 + keyLenIdx * 2;
            var rks       = new byte[numRounds + 1][];

            rks[0] = KeyScheduleRound(W0, W1, 19);
            rks[1] = KeyScheduleRound(W1, W2, 19);
            rks[2] = KeyScheduleRound(W2, W3, 19);
            rks[3] = KeyScheduleRound(W3, W0, 19);

            rks[4] = KeyScheduleRound(W0, W1, 31);
            rks[5] = KeyScheduleRound(W1, W2, 31);
            rks[6] = KeyScheduleRound(W2, W3, 31);
            rks[7] = KeyScheduleRound(W3, W0, 31);

            rks[8]  = KeyScheduleRound(W0, W1, 67);
            rks[9]  = KeyScheduleRound(W1, W2, 67);
            rks[10] = KeyScheduleRound(W2, W3, 67);
            rks[11] = KeyScheduleRound(W3, W0, 67);

            rks[12] = KeyScheduleRound(W0, W1, 97);
            if (numRounds > 12)
            {
                rks[13] = KeyScheduleRound(W1, W2, 97);
                rks[14] = KeyScheduleRound(W2, W3, 97);
                if (numRounds > 14)
                {
                    rks[15] = KeyScheduleRound(W3, W0, 97);

                    rks[16] = KeyScheduleRound(W0, W1, 109);
                }
            }

            if (!forEncryption)
            {
                ReverseKeys(rks);

                for (var i = 1; i < numRounds; ++i) A(rks[i]);
            }

            return rks;
        }

        protected static byte[] KeyScheduleRound(byte[] w, byte[] wr, int n)
        {
            var rk = new byte[16];

            int off = n >> 3, right = n & 7, left = 8 - right;

            var hi = wr[15 - off] & 0xFF;

            for (var to = 0; to < 16; ++to)
            {
                var lo = wr[(to - off) & 0xF] & 0xFF;

                var b = (hi << left) | (lo >> right);
                b ^= w[to] & 0xFF;

                rk[to] = (byte)b;

                hi = lo;
            }

            return rk;
        }

        protected static void ReverseKeys(byte[][] keys)
        {
            int length = keys.Length, limit = length / 2, last = length - 1;
            for (var i = 0; i < limit; ++i)
            {
                var t = keys[i];
                keys[i]        = keys[last - i];
                keys[last - i] = t;
            }
        }

        protected static byte SB1(byte x) { return SB1_sbox[x & 0xFF]; }

        protected static byte SB2(byte x) { return SB2_sbox[x & 0xFF]; }

        protected static byte SB3(byte x) { return SB3_sbox[x & 0xFF]; }

        protected static byte SB4(byte x) { return SB4_sbox[x & 0xFF]; }

        protected static void SL1(byte[] z)
        {
            z[0]  = SB1(z[0]);
            z[1]  = SB2(z[1]);
            z[2]  = SB3(z[2]);
            z[3]  = SB4(z[3]);
            z[4]  = SB1(z[4]);
            z[5]  = SB2(z[5]);
            z[6]  = SB3(z[6]);
            z[7]  = SB4(z[7]);
            z[8]  = SB1(z[8]);
            z[9]  = SB2(z[9]);
            z[10] = SB3(z[10]);
            z[11] = SB4(z[11]);
            z[12] = SB1(z[12]);
            z[13] = SB2(z[13]);
            z[14] = SB3(z[14]);
            z[15] = SB4(z[15]);
        }

        protected static void SL2(byte[] z)
        {
            z[0]  = SB3(z[0]);
            z[1]  = SB4(z[1]);
            z[2]  = SB1(z[2]);
            z[3]  = SB2(z[3]);
            z[4]  = SB3(z[4]);
            z[5]  = SB4(z[5]);
            z[6]  = SB1(z[6]);
            z[7]  = SB2(z[7]);
            z[8]  = SB3(z[8]);
            z[9]  = SB4(z[9]);
            z[10] = SB1(z[10]);
            z[11] = SB2(z[11]);
            z[12] = SB3(z[12]);
            z[13] = SB4(z[13]);
            z[14] = SB1(z[14]);
            z[15] = SB2(z[15]);
        }

        protected static void Xor(byte[] z, byte[] x)
        {
            for (var i = 0; i < 16; ++i) z[i] ^= x[i];
        }
    }
}
#pragma warning restore
#endif