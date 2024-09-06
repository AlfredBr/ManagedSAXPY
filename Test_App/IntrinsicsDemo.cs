using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Test_App
{
	public static partial class IntrinsicsDemo
	{
		public static void VectorAdd(int N = 100_000, bool _ = false)
		{
			Console.WriteLine("IntrinsicsDemo");
			// Ensure N is a multiple of 4 for simplicity
			if (N % 4 != 0)
			{
				throw new ArgumentException("N must be a multiple of 4");
			}

			// Initialize arrays
			var a = Enumerable.Range(1, N).ToArray();
			var b = Enumerable.Range(2, N).ToArray();
			var c = new int[N];

			// Perform SIMD addition
			for (int i = 0; i < N; i += 4)
			{
				var va = Vector128.Create(a[i], a[i + 1], a[i + 2], a[i + 3]);
				var vb = Vector128.Create(b[i], b[i + 1], b[i + 2], b[i + 3]);
				var vc = Sse2.Add(va, vb);

				// Store the result back into the array
				vc.CopyTo(c, i);
			}

			const int limit = 5;
			Console.WriteLine("BEFORE");
			Console.WriteLine($"  a[] = {string.Join(", ", a.Skip(Math.Max(0, a.Length - limit)).Take(limit).Select(f => f.ToString("D2")))}");
			Console.WriteLine($"  b[] = {string.Join(", ", b.Skip(Math.Max(0, b.Length - limit)).Take(limit).Select(f => f.ToString("D2")))}");
			Console.WriteLine("AFTER");
			Console.WriteLine($"  c[] = {string.Join(", ", c.Skip(Math.Max(0, c.Length - limit)).Take(limit).Select(f => f.ToString("D2")))}");
		}
	}
}
