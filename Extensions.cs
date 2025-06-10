using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ConsoleAppFramework
{
	// Token: 0x02000004 RID: 4
	public static class Extensions
	{
		// Token: 0x0600000A RID: 10 RVA: 0x000024F4 File Offset: 0x000006F4
		public static byte[] ReadBytes(this Stream stream, int count)
		{
			byte[] array = new byte[count];
			stream.Read(array, 0, count);
			return array;
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002514 File Offset: 0x00000714
		public static string ReadString(this Stream stream, int size)
		{
			byte[] array = new byte[size];
			stream.Read(array, 0, size);
			return Encoding.UTF8.GetString(array);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002540 File Offset: 0x00000740
		public static string ReadCString(this Stream stream, int maxSize = 256)
		{
			byte[] array = new byte[maxSize];
			int num = stream.ReadByte();
			int num2 = 0;
			while (num2 < maxSize && num > 0)
			{
				array[num2] = (byte)num;
				num = stream.ReadByte();
				num2++;
			}
			return Encoding.UTF8.GetString(array, 0, num2);
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002584 File Offset: 0x00000784
		public static T[] ReadArray<T>(this Stream reader, int count) where T : struct
		{
			int num = Marshal.SizeOf<T>();
			T[] array = new T[count];
			if (count == 0)
			{
				return array;
			}
			byte[] array2 = new byte[count * num];
			reader.Read(array2, 0, array2.Length);
			GCHandle gchandle = GCHandle.Alloc(array2, GCHandleType.Pinned);
			IntPtr intPtr = gchandle.AddrOfPinnedObject();
			for (int i = 0; i < count; i++)
			{
				array[i] = Marshal.PtrToStructure<T>(intPtr + i * num);
			}
			gchandle.Free();
			return array;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000025FC File Offset: 0x000007FC
		public static T Read<T>(this Stream reader) where T : struct
		{
			byte[] array = new byte[Marshal.SizeOf<T>()];
			reader.Read(array, 0, array.Length);
			GCHandle gchandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			T t = Marshal.PtrToStructure<T>(gchandle.AddrOfPinnedObject());
			gchandle.Free();
			return t;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000263C File Offset: 0x0000083C
		public static void Write<T>(this Stream reader, T value) where T : struct
		{
			byte[] array = new byte[Marshal.SizeOf<T>()];
			GCHandle gchandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			Marshal.StructureToPtr<T>(value, gchandle.AddrOfPinnedObject(), true);
			gchandle.Free();
			reader.Write(array, 0, array.Length);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x0000267C File Offset: 0x0000087C
		public static bool TryDequeue<T>(this Queue<T> queue, out T result)
		{
			if (queue.Count > 0)
			{
				result = queue.Dequeue();
				return true;
			}
			result = default(T);
			return false;
		}
	}
}
