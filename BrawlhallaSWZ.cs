using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ionic.Zlib; 

namespace ConsoleAppFramework
{
    // Token: 0x02000002 RID: 2
    public static class BrawlhallaSWZ
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public static string[] Decrypt(Stream input, uint globalKey = 119026080U)
        {
            ReadUInt32BE(input); 
            WELL512 well = new WELL512(ReadUInt32BE(input) ^ globalKey); 
            uint num = 771006925U; 
            uint num2 = globalKey % 31U + 5U;
            int num3 = 0;
            while ((long)num3 < (long)((ulong)num2))
            {
                num ^= well.NextUInt(); 
                num3++;
            }
            List<string> list = new List<string>();
            while (input.Position != input.Length)
            {
                string text;
                if (ReadStringEntry(input, well, out text)) 
                {
                    list.Add(text);
                }
            }
            return list.ToArray();
        }

        // Token: 0x06000002 RID: 2 RVA: 0x000020CC File Offset: 0x000002CC
        public static byte[] Encrypt(uint seed, uint globalKey = 119026080U, params string[] stringEntries)
        {
            WELL512 well = new WELL512(seed ^ globalKey);
            uint num = 771006925U;
            uint num2 = globalKey % 31U + 5U;
            int num3 = 0;
            while ((long)num3 < (long)((ulong)num2))
            {
                num ^= well.NextUInt();
                num3++;
            }
            byte[] array;
            using (MemoryStream memoryStream = new MemoryStream(4096))
            {
                WriteUInt32BE(memoryStream, num); 
                WriteUInt32BE(memoryStream, seed); 
                foreach (string text in stringEntries)
                {
                    WriteStringEntry(Encoding.UTF8.GetBytes(text), well, memoryStream); 
                }
                array = memoryStream.ToArray();
            }
            return array;
        }

        // Token: 0x06000003 RID: 3 RVA: 0x0000217C File Offset: 0x0000037C
        private static bool ReadStringEntry(Stream input, WELL512 rand, out string result)
        {
            uint num = ReadUInt32BE(input) ^ rand.NextUInt(); 
            ReadUInt32BE(input); 
            rand.NextUInt();
            ReadUInt32BE(input); 
            if ((ulong)num + (ulong)input.Position > (ulong)input.Length)
            {
                result = null;
                return false;
            }
            byte[] array = new byte[num];
            input.Read(array, 0, array.Length);
            uint num2 = rand.NextUInt();
            int num3 = 0;
            while ((long)num3 < (long)((ulong)num))
            {
                int num4 = num3 & 15;
                byte[] array2 = array;
                int num5 = num3;
                array2[num5] ^= (byte)(((255U << num4) & rand.NextUInt()) >> num4);
                num2 = (uint)array[num3] ^ RotateRight(num2, num3 % 7 + 1);
                num3++;
            }
            byte[] array3 = ZlibStream.UncompressBuffer(array);
            result = Encoding.UTF8.GetString(array3);
            return true;
        }

        // Token: 0x06000004 RID: 4 RVA: 0x00002244 File Offset: 0x00000444
        private static void WriteStringEntry(byte[] input, WELL512 rand, Stream output)
        {
            byte[] array = ZlibStream.CompressBuffer(input);
            uint num = (uint)(array.Length ^ (int)rand.NextUInt());
            uint num2 = (uint)(input.Length ^ (int)rand.NextUInt());
            uint num3 = rand.NextUInt();
            for (int i = 0; i < array.Length; i++)
            {
                num3 = (uint)array[i] ^ RotateRight(num3, i % 7 + 1); 
                int num4 = i & 15;
                byte[] array2 = array;
                int num5 = i;
                array2[num5] ^= (byte)(((255U << num4) & rand.NextUInt()) >> num4);
            }
            WriteUInt32BE(output, num);   
            WriteUInt32BE(output, num2);  
            WriteUInt32BE(output, num3);  
            output.Write(array, 0, array.Length);
        }

        // Token: 0x06000005 RID: 5 RVA: 0x000022E6 File Offset: 0x000004E6
        private static uint RotateRight(uint v, int bits)
        {
            return (v >> bits) | (v << 32 - bits);
        }

        // Token: 0x06000006 RID: 6 RVA: 0x000022F8 File Offset: 0x000004F8
        public static uint ReadUInt32BE(Stream stream)
        {
            byte[] array = new byte[4];
            stream.Read(array, 0, 4);
            return (uint)((int)array[3] | ((int)array[2] << 8) | ((int)array[1] << 16) | ((int)array[0] << 24));
        }

        // Token: 0x06000007 RID: 7 RVA: 0x00002330 File Offset: 0x00000530
        public static void WriteUInt32BE(Stream stream, uint value)
        {
            byte[] array = new byte[]
            {
                (byte)((value >> 24) & 255U),
                (byte)((value >> 16) & 255U),
                (byte)((value >> 8) & 255U),
                (byte)(value & 255U)
            };
            stream.Write(array, 0, 4);
        }
    }
}