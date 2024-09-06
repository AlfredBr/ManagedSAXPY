namespace Test_App;

public static partial class ParallelForLoopDemo
{
	public static void VectorAdd(int N = 100_000, bool _ = false)
	{
		Console.WriteLine("ParallelForLoopDemo");
		var hostA = Enumerable.Range(1, N).ToArray();
		var hostB = Enumerable.Range(2, N).ToArray();
		var hostC = new int[N];

		Parallel.For(0, N, i => hostC[i] = hostA[i] + hostB[i]);

		const int limit = 5;
		Console.WriteLine("BEFORE");
		Console.WriteLine($"  a[] = {string.Join(", ", hostA.Skip(Math.Max(0, hostA.Length - limit)).Take(limit).Select(f => f.ToString("D2")))}");
		Console.WriteLine($"  b[] = {string.Join(", ", hostB.Skip(Math.Max(0, hostB.Length - limit)).Take(limit).Select(f => f.ToString("D2")))}");
		Console.WriteLine("AFTER");
		Console.WriteLine($"  c[] = {string.Join(", ", hostC.Skip(Math.Max(0, hostC.Length - limit)).Take(limit).Select(f => f.ToString("D2")))}");
	}
}