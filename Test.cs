using System;
using System.IO;
using static ShaCalcs;

namespace SHA256
{
    class Test
    {
        static void Main(string[] args)
        {
            if (args.Length > 0) Console.WriteLine(BitConverter.ToString(GenHash(new Context(), File.ReadAllBytes(args[0]))).Replace("-", "").ToLower());
        }
    }
}