using System;

namespace ConsoleAppFramework
{
	// Token: 0x02000006 RID: 6
	public class WELL512
	{
		// Token: 0x06000013 RID: 19 RVA: 0x00002768 File Offset: 0x00000968
		public WELL512(uint seed)
		{
			this.Index = 0;
			this.State = new uint[16];
			this.SetSeed(seed);
		}

		// Token: 0x06000014 RID: 20 RVA: 0x0000278C File Offset: 0x0000098C
		private void SetSeed(uint seed)
		{
			this.Index = 0;
			this.State[0] = seed;
			for (uint num = 1U; num < 16U; num += 1U)
			{
				this.State[(int)num] = num + 1812433253U * (this.State[(int)(num - 1U)] ^ (this.State[(int)(num - 1U)] >> 30));
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000027E0 File Offset: 0x000009E0
		public uint NextUInt()
		{
			uint num = this.State[this.Index];
			uint num2 = this.State[(this.Index + 13) & 15];
			uint num3 = num ^ num2 ^ (num << 16) ^ (num2 << 15);
			num2 = this.State[(this.Index + 9) & 15];
			num2 ^= num2 >> 11;
			num = (this.State[this.Index] = num3 ^ num2);
			uint num4 = num ^ ((num << 5) & 3661901092U);
			this.Index = (this.Index + 15) & 15;
			num = this.State[this.Index];
			this.State[this.Index] = num ^ num3 ^ num4 ^ (num << 2) ^ (num3 << 18) ^ (num2 << 28);
			return this.State[this.Index];
		}

		// Token: 0x04000001 RID: 1
		private readonly uint[] State;

		// Token: 0x04000002 RID: 2
		private int Index;
	}
}
