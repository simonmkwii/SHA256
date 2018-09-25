using System;
using System.IO;
using static ShaCalcs;

namespace SHA256
{
    class Test
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                var In = File.ReadAllBytes(args[0]);
                byte[] Buffer = new byte[0x20];
                var Ctx = new Context();
                ComputeHash(Ctx, In);
                Finalise(Ctx, Buffer);
                Console.WriteLine(BitConverter.ToString(Buffer).Replace("-", "").ToLower());
            }
        }
    }
}