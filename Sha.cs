public static class ShaCalcs
{
    public class Context
    {
        public byte[] Data = new byte[64];
        public uint[] State = new uint[8];
        public uint DLen;
        public ulong BLen;
    }

    public static uint[] K =
    {
        0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
        0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3, 0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
        0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc, 0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
        0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7, 0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
        0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13, 0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
        0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3, 0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
        0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5, 0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
        0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208, 0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2
    };

    public static void Transform(Context Ctx, byte[] Data)
    {
        uint I1;
        uint I2;

        uint[] Blk = new uint[64];

        for (I1 = 0, I2 = 0; I1 < 16; ++I1, I2 += 4)
        {
            Blk[I1] = (uint)((Data[I2] << 24) | (Data[I2 + 1] << 16) | (Data[I2 + 2] << 8) | (Data[I2 + 3]));
        }

        for (; I1 < 64; ++I1)
        {
            Blk[I1] = (((Blk[I1 - 2] >> 17) | (Blk[I1 - 2] << 15)) ^ ((Blk[I1 - 2] >> 19) | (Blk[I1 - 2] << 13)) ^ (Blk[I1 - 2] >> 10)) + Blk[I1 - 7] + (((Blk[I1 - 15] >> 7) | (Blk[I1 - 15] << 25)) ^ ((Blk[I1 - 15] >> 18) | (Blk[I1 - 15] << 14)) ^ (Blk[I1 - 15] >> 3)) + Blk[I1 - 16];
        }

        uint[] S = new uint[8];

        for (int i = 0; i < 8; i++) S[i] = Ctx.State[i];

        for (I1 = 0; I1 < 64; ++I1)
        {
            uint T1 = S[7] + (((S[4] >> 6) | (S[4] << 26)) ^ ((S[4] >> 11) | (S[4] << 21)) ^ ((S[4] >> 25) | (S[4] << 7))) + ((S[4] & S[5]) ^ (~S[4] & S[6])) + K[I1] + Blk[I1];
            uint T2 = (((S[0] >> 2) | (S[0] << 30)) ^ ((S[0] >> 13) | (S[0] << 19)) ^ ((S[0] >> 22) | (S[0] << 10))) + ((S[0] & S[1]) ^ (S[0] & S[2]) ^ (S[1] & S[2]));
            S[7] = S[6];
            S[6] = S[5];
            S[5] = S[4];
            S[4] = S[3] + T1;
            S[3] = S[2];
            S[2] = S[1];
            S[1] = S[0];
            S[0] = T1 + T2;
        }

        for (int i = 0; i < 8; i++) Ctx.State[i] += S[i];
    }

    public static byte[] GenHash(Context Ctx, byte[] Data)
    {
        var i = Ctx.DLen;
        var Hash = new byte[0x20];

        Ctx.DLen = 0;
        Ctx.BLen = 0;

        void TB() { Transform(Ctx, Ctx.Data); }

        uint[] Init = { 0x6a09e667, 0xbb67ae85, 0x3c6ef372, 0xa54ff53a, 0x510e527f, 0x9b05688c, 0x1f83d9ab, 0x5be0cd19 };

        for (int c = 0; c < 8; c++) Ctx.State[c] = Init[c];

        for (int m = 0; m < Data.Length; ++m)
        {
            Ctx.Data[Ctx.DLen] = Data[m];
            Ctx.DLen++;
            if (Ctx.DLen == 64)
            {
                TB();
                Ctx.BLen += 512;
                Ctx.DLen = 0;
            }
        }

        if (Ctx.DLen < 56)
        {
            Ctx.Data[i++] = 128;
            while (i < 56) Ctx.Data[i++] = 0;
        }
        else
        {
            Ctx.Data[i++] = 128;
            while (i < 64) Ctx.Data[i++] = 0;
            TB();
        }

        Ctx.BLen += Ctx.DLen * 8;

        for (int d = 0, s = 63; d <= 56; d += 8, --s) Ctx.Data[s] = (byte)(Ctx.BLen >> d);

        TB();

        for (i = 0; i < 4; ++i) for (uint n = 0; n < 8; ++n) Hash[i + n * 4] = (byte)((Ctx.State[n] >> (int)(24 - i * 8)) & 255);

        return Hash;
    }
}