public static class ShaCalcs
{
    public class Context
    {
        public byte[] Data = new byte[64];
        public uint DataLength;
        public ulong BitLength;
        public uint[] BlkState = new uint[8];
    }

    public static uint[] RoundConstTable =
    {
        0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5,
        0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
        0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3,
        0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
        0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc,
        0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
        0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7,
        0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
        0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13,
        0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
        0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3,
        0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
        0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5,
        0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
        0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208,
        0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2
    };

    public static void ComputeHash(Context Ctx, byte[] Data)
    {
        Ctx.DataLength = 0;
        Ctx.BitLength = 0;

        Ctx.BlkState[0] = 0x6a09e667;
        Ctx.BlkState[1] = 0xbb67ae85;
        Ctx.BlkState[2] = 0x3c6ef372;
        Ctx.BlkState[3] = 0xa54ff53a;
        Ctx.BlkState[4] = 0x510e527f;
        Ctx.BlkState[5] = 0x9b05688c;
        Ctx.BlkState[6] = 0x1f83d9ab;
        Ctx.BlkState[7] = 0x5be0cd19;

        for (uint i = 0; i < Data.Length; ++i)
        {
            Ctx.Data[Ctx.DataLength] = Data[i];
            Ctx.DataLength++;

            if (Ctx.DataLength == 64)
            {
                TransformBlock(Ctx, Ctx.Data);
                Ctx.BitLength += 512;
                Ctx.DataLength = 0;
            }
        }
    }

    public static void Finalise(Context Ctx, byte[] Hash)
    {
        uint i = Ctx.DataLength;

        if (Ctx.DataLength < 56)
        {
            Ctx.Data[i++] = 128;

            while (i < 56)
            {
                Ctx.Data[i++] = 0;
            }
        }
        else
        {
            Ctx.Data[i++] = 128;

            while (i < 64)
            {
                Ctx.Data[i++] = 0;
            }

            TransformBlock(Ctx, Ctx.Data);
        }

        Ctx.BitLength += Ctx.DataLength * 8;

        Ctx.Data[63] = (byte)Ctx.BitLength;
        Ctx.Data[62] = (byte)(Ctx.BitLength >> 8);
        Ctx.Data[61] = (byte)(Ctx.BitLength >> 16);
        Ctx.Data[60] = (byte)(Ctx.BitLength >> 24);
        Ctx.Data[59] = (byte)(Ctx.BitLength >> 32);
        Ctx.Data[58] = (byte)(Ctx.BitLength >> 40);
        Ctx.Data[57] = (byte)(Ctx.BitLength >> 48);
        Ctx.Data[56] = (byte)(Ctx.BitLength >> 56);

        TransformBlock(Ctx, Ctx.Data);

        for (i = 0; i < 4; ++i)
        {
            for (uint n = 0; n < 8; ++n)
            {
                Hash[i + n * 4] = (byte)((Ctx.BlkState[n] >> (int)(24 - i * 8)) & 255);
            }
        }
    }

    public static void TransformBlock(Context Ctx, byte[] Data)
    {
        uint S0;
        uint S1;
        uint S2;
        uint S3;
        uint S4;
        uint S5;
        uint S6;
        uint S7;
        uint S8;
        uint S9;

        uint[] Blk = new uint[64];

        for (S8 = 0, S9 = 0; S8 < 16; ++S8, S9 += 4)
        {
            Blk[S8] = (uint)((Data[S9] << 24) | (Data[S9 + 1] << 16) | (Data[S9 + 2] << 8) | (Data[S9 + 3]));
        }

        for (; S8 < 64; ++S8)
        {
            Blk[S8] = ((((Blk[S8 - 2]) >> 17) | ((Blk[S8 - 2]) << (32 - 17))) ^ (((Blk[S8 - 2]) >> 19) | ((Blk[S8 - 2]) << (32 - 19))) ^ ((Blk[S8 - 2]) >> 10)) + Blk[S8 - 7] + ((((Blk[S8 - 15]) >> 7) | ((Blk[S8 - 15]) << (32 - 7))) ^ (((Blk[S8 - 15]) >> 18) | ((Blk[S8 - 15]) << (32 - 18))) ^ ((Blk[S8 - 15]) >> 3)) + Blk[S8 - 16];
        }

        S0 = Ctx.BlkState[0];
        S1 = Ctx.BlkState[1];
        S2 = Ctx.BlkState[2];
        S3 = Ctx.BlkState[3];
        S4 = Ctx.BlkState[4];
        S5 = Ctx.BlkState[5];
        S6 = Ctx.BlkState[6];
        S7 = Ctx.BlkState[7];

        for (S8 = 0; S8 < 64; ++S8)
        {
            uint T1 = S7 + (((S4 >> (6)) | (S4 << (32 - 6))) ^ (((S4) >> (11)) | ((S4) << (32 - (11)))) ^ (((S4) >> (25)) | ((S4) << (32 - (25))))) + (((S4) & (S5)) ^ (~(S4) & (S6))) + RoundConstTable[S8] + Blk[S8];
            uint T2 = ((((S0) >> (2)) | ((S0) << (32 - (2)))) ^ (((S0) >> (13)) | ((S0) << (32 - (13)))) ^ (((S0) >> (22)) | ((S0) << (32 - (22))))) + (((S0) & (S1)) ^ ((S0) & (S2)) ^ ((S1) & (S2)));
            S7 = S6;
            S6 = S5;
            S5 = S4;
            S4 = S3 + T1;
            S3 = S2;
            S2 = S1;
            S1 = S0;
            S0 = T1 + T2;
        }

        Ctx.BlkState[0] += S0;
        Ctx.BlkState[1] += S1;
        Ctx.BlkState[2] += S2;
        Ctx.BlkState[3] += S3;
        Ctx.BlkState[4] += S4;
        Ctx.BlkState[5] += S5;
        Ctx.BlkState[6] += S6;
        Ctx.BlkState[7] += S7;
    }
}