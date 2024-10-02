using System.Diagnostics;

using Test_ComputeSharp;

using Test_ILGPU;
using Test_Library;

using Test_ManagedCUDA;

using CppClassLibrary1;

namespace Test_App;

public static class Program
{
	public static void Main()
	{
		var divider = new string('-', Console.WindowWidth);

		//Console.Clear();

		const int N = (4_096 * 1_024) - 64;

		var arrayX = Enumerable.Range(1, N).Select(x => (float) x).ToArray();
		var arrayY = Enumerable.Range(2, N).Select(y => (float) y).ToArray();
		const float A = 2.0f;
		const int expectedLength = 5;
		var expected = arrayX.Select((x, i) => (A * x) + arrayY[i]).Last(expectedLength);

		Measure(() => LinqDemo.Saxpy(N, A, arrayX, arrayY).Last(expectedLength).Check1D(expected).Print("LinqDemo"));
		Console.WriteLine(divider);
		Measure(() => ForLoopDemo.Saxpy(N, A, arrayX, arrayY).Last(expectedLength).Check1D(expected).Print("ForLoop"));
		Console.WriteLine(divider);
		Measure(() => ParallelFor.Saxpy(N, A, arrayX, arrayY).Last(expectedLength).Check1D(expected).Print("ParallelForLoop"));
		Console.WriteLine(divider);
		Measure(() => Intrinsics.SaxpyVector(N, A, arrayX, arrayY).Last(expectedLength).Check1D(expected).Print("Vector"));
		Console.WriteLine(divider);
		Measure(() => Intrinsics.SaxpyMultiplyAdd(N, A, arrayX, arrayY).Last(expectedLength).Check1D(expected).Print("MultiplyAdd"));
		Console.WriteLine(divider);
		Measure(() => Intrinsics.SaxpyMultiplyAddUsingPointers(N, A, arrayX.Select(x => (double)x).ToArray(), arrayY.Select(x => (double)x).ToArray()).Last(expectedLength).Check1D(expected.Select(x => (double)x).ToArray()).Print("MultiplyAddUsingPointers"));
		Console.WriteLine(divider);
		Measure(() => Intrinsics.Saxpy128(N, A, arrayX, arrayY).Last(expectedLength).Check1D(expected).Print("SIMD128"));
		Console.WriteLine(divider);
		Measure(() => Intrinsics.Saxpy256(N, A, arrayX, arrayY).Last(expectedLength).Check1D(expected).Print("SIMD256"));
		Console.WriteLine(divider);
		Measure(() => ComputeSharpDemo.Saxpy1(N, A, arrayX, arrayY).Last(expectedLength).Check1D(expected).Print("ComputeSharp1"));
		Console.WriteLine(divider);
		Measure(() => ComputeSharpDemo.Saxpy2(N, A, arrayX, arrayY).Last(expectedLength).Check1D(expected).Print("ComputeSharp2"));
		Console.WriteLine(divider);
		Measure(() => ILGpuDemo.Saxpy(N, A, arrayX, arrayY).Last(expectedLength).Check1D(expected).Print("ILGpu"));
		Console.WriteLine(divider);
		Measure(() => ManagedCUDADemo.Saxpy(N, A, arrayX, arrayY).Last(expectedLength).Check1D(expected).Print("ManagedCUDA"));
		Console.WriteLine(divider);
		Measure(() => ManagedCUDADemo.CuBlasSaxpy(N, A, arrayX, arrayY).Last(expectedLength).Check1D(expected).Print("ManagedCuBlas"));
		Console.WriteLine(divider);
		Measure(() => CppClass1.Saxpy1(N, A, arrayX, arrayY).Last(expectedLength).Check1D(expected).Print("C++Saxpy1"));
		Console.WriteLine(divider);
		arrayY = Enumerable.Range(2, N).Select(y => (float)y).ToArray();
		Measure(() => CppClass1.Saxpy2(N, A, arrayX, arrayY).Last(expectedLength).Check1D(expected).Print("C++Saxpy2"));
		Console.WriteLine(divider);

		//Console.WriteLine();
	}

	private static void Measure(Action action)
	{
		var sw = Stopwatch.StartNew();
		action();
		sw.Stop();
		Console.WriteLine($"Elapsed: {sw.ElapsedMilliseconds} ms");
	}
}

public static class Extensions
{
	public static void Print(this string text, string label = "")
	{
		Console.WriteLine($"{label} {text}".Trim());
	}
	public static IEnumerable<T> Last<T>(this IEnumerable<T> source, int count)
	{
		var buffer = new Queue<T>(count);
		foreach (var item in source)
		{
			if (buffer.Count == count)
			{
				buffer.Dequeue();
			}
			buffer.Enqueue(item);
		}
		return buffer;
	}

	public static string Check1D<T>(this IEnumerable<T> inputArray, IEnumerable<T> expectedArray)
	{
		var result = inputArray.SequenceEqual(expectedArray);
		return $"{(result ? "✅" : "❌")} [{string.Join(",", inputArray)}] {(result ? "==" : "!=")} [{string.Join(",", expectedArray)}]";
	}
}