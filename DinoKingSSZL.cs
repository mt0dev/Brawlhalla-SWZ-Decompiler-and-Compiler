using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ConsoleAppFramework
{
	// Token: 0x02000003 RID: 3
	internal static class DinoKingSSZL
	{
		// Token: 0x06000008 RID: 8 RVA: 0x00002384 File Offset: 0x00000584
		public static void Extract(string filename)
		{
			using (FileStream fileStream = File.OpenRead(filename + ".fil"))
			{
				using (FileStream fileStream2 = File.OpenRead(filename + ".bin"))
				{
					while (fileStream.Position != fileStream.Length)
					{
						DinoKingSSZL.FileInfo fileInfo = fileStream.Read<DinoKingSSZL.FileInfo>();
						fileStream2.Position = (long)fileInfo.Offset;
						byte[] array;
						if (fileInfo.Flags == 1)
						{
							DinoKingSSZL.SSZL_Header sszl_Header = fileStream2.Read<DinoKingSSZL.SSZL_Header>();
							array = fileStream2.ReadBytes(fileInfo.Size - 16);
							array = DinoKingSSZL.Decompress(sszl_Header, array);
						}
						else
						{
							array = fileStream2.ReadBytes(fileInfo.Size);
						}
						File.WriteAllBytes("Dump\\" + fileInfo.Filename, array);
					}
				}
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002454 File Offset: 0x00000654
		private static byte[] Decompress(DinoKingSSZL.SSZL_Header header, byte[] input)
		{
			byte[] array = new byte[header.DecompressedSize];
			int num = 0;
			int i = 0;
			while (i < header.DecompressedSize)
			{
				byte b = input[num++];
				if ((int)b != header.Control)
				{
					array[i++] = b;
				}
				else
				{
					byte b2 = input[num++];
					if ((int)b2 == header.Control)
					{
						array[i++] = (byte)header.Control;
					}
					else
					{
						byte b3 = input[num++];
						if ((int)b2 > header.Control)
						{
							b2 -= 1;
						}
						int j = 0;
						while (j < (int)b3)
						{
							array[i] = array[i - (int)b2];
							j++;
							i++;
						}
					}
				}
			}
			return array;
		}

		// Token: 0x02000007 RID: 7
		private struct FileInfo
		{
			// Token: 0x04000003 RID: 3
			public int Offset;

			// Token: 0x04000004 RID: 4
			public int Size;

			// Token: 0x04000005 RID: 5
			public int Flags;

			// Token: 0x04000006 RID: 6
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string Filename;
		}

		// Token: 0x02000008 RID: 8
		private struct SSZL_Header
		{
			// Token: 0x04000007 RID: 7
			public uint Magic;

			// Token: 0x04000008 RID: 8
			public int DecompressedSize;

			// Token: 0x04000009 RID: 9
			public int Size;

			// Token: 0x0400000A RID: 10
			public int Control;
		}
	}
}
