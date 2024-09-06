using System.Diagnostics;

using Test_ComputeSharp;

using Test_ILGPU;

using Test_ManagedCUDA;

using CppClassLibrary1;

namespace Test_App;

public static class Program
{
	public static void Main()
	{
		//const bool useCpu = false; //Debugger.IsAttached;
		//var header = new string('=', Console.WindowWidth);
		var divider = new string('-', Console.WindowWidth);

		//Console.Clear();

		const int N = (4_096 * 1_024) - 64;

		//Measure(() => ForLoopDemo.VectorAdd(N, useCpu));
		//Console.WriteLine(divider);
		//Measure(() => ParallelForLoopDemo.VectorAdd(N, useCpu));
		//Console.WriteLine(divider);
		//Measure(() => IntrinsicsDemo.VectorAdd(N, useCpu));
		//Console.WriteLine(divider);
		//Measure(() => ComputeSharpDemo.VectorAdd(N, useCpu));
		//Console.WriteLine(divider);
		//Measure(() => ILGpuDemo.VectorAdd(N, useCpu));
		//Console.WriteLine(divider);
		//Measure(() => ManagedCUDADemo.VectorAdd(N, useCpu));
		//Console.WriteLine(divider);

		var arrayX = Enumerable.Range(1, N).Select(x => (float) x).ToArray();
		var arrayY = Enumerable.Range(2, N).Select(y => (float) y).ToArray();
		const float A = 2.0f;
		const int expectedLength = 5;
		var expected = arrayX.Select((x, i) => (A * x) + arrayY[i]).Last(expectedLength);

		Measure(() => ForLoopDemo.Saxpy(N, A, arrayX, arrayY).Last(expectedLength).Check1D(expected).Print("ForLoop"));
		Console.WriteLine(divider);
		Measure(() => ParallelForLoopDemo.Saxpy(N, A, arrayX, arrayY).Last(expectedLength).Check1D(expected).Print("ParallelForLoop"));
		Console.WriteLine(divider);
		Measure(() => IntrinsicsDemo.SaxpyVector(N, A, arrayX, arrayY).Last(expectedLength).Check1D(expected).Print("Vector"));
		Console.WriteLine(divider);
		Measure(() => IntrinsicsDemo.SaxpyMultiplyAdd(N, A, arrayX, arrayY).Last(expectedLength).Check1D(expected).Print("MultiplyAdd"));
		Console.WriteLine(divider);
		Measure(() => IntrinsicsDemo.SaxpyMultiplyAddUsingPointers(N, A, arrayX.Select(x => (double)x).ToArray(), arrayY.Select(x => (double)x).ToArray()).Last(expectedLength).Check1D(expected.Select(x => (double)x).ToArray()).Print("MultiplyAddUsingPointers"));
		Console.WriteLine(divider);
		Measure(() => IntrinsicsDemo.Saxpy128(N, A, arrayX, arrayY).Last(expectedLength).Check1D(expected).Print("SIMD128"));
		Console.WriteLine(divider);
		Measure(() => IntrinsicsDemo.Saxpy256(N, A, arrayX, arrayY).Last(expectedLength).Check1D(expected).Print("SIMD256"));
		Console.WriteLine(divider);
		Measure(() => ComputeSharpDemo.Saxpy(N, A, arrayX, arrayY).Last(expectedLength).Check1D(expected).Print("ComputeSharp"));
		Console.WriteLine(divider);
		Measure(() => ILGpuDemo.Saxpy(N, A, arrayX, arrayY).Last(expectedLength).Check1D(expected).Print("ILGpu"));
		Console.WriteLine(divider);
		Measure(() => ManagedCUDADemo.Saxpy(N, A, arrayX, arrayY).Last(expectedLength).Check1D(expected).Print("ManagedCUDA"));
		Console.WriteLine(divider);
		Measure(() => ManagedCUDADemo.CuBlasSaxpy(N, A, arrayX, arrayY).Last(expectedLength).Check1D(expected).Print("ManagedCuBlas"));
		Console.WriteLine(divider);
		Measure(() => CppClass1.Saxpy(N, A, arrayX, arrayY).Last(expectedLength).Check1D(expected).Print("C++Saxpy"));
		Console.WriteLine(divider);

		//Console.WriteLine(header);
		//ComputeSharpDemo.MultiplyByTwoIntoSingleIntArray(useCpu);
		//Console.WriteLine(divider);
		//ComputeSharpDemo.Saxpy();
		//ComputeSharpDemo.Lenna(@"E:\Test.GPU\Test_App\lenna.jpg");
		//using var ilGpuPuzzle = ILGpuPuzzle.Create(useCpu);
		//Console.WriteLine(ilGpuPuzzle.Description);
		//Console.WriteLine(new string('-', ilGpuPuzzle.Description.Length));
		//Console.WriteLine($"Puzzle #1 : {ilGpuPuzzle.Puzzle1().Check1D([10,11,12,13])}");
		//Console.WriteLine($"Puzzle #2a : {ilGpuPuzzle.Puzzle2a().Check1D([0,2,4,6])}");
		//Console.WriteLine($"Puzzle #2b : {ilGpuPuzzle.Puzzle2b().Check1D([0,2,4,6])}");
		//Console.WriteLine($"Puzzle #2c : {ilGpuPuzzle.Puzzle2c().Check1D([0,2,4,6])}");
		//Console.WriteLine($"Puzzle #2d : {ilGpuPuzzle.Puzzle2d().Check1D([0,2,4,6])}");
		//Console.WriteLine($"Puzzle #3 : {ilGpuPuzzle.Puzzle3().Check1D([10, 11, 12, 13])}");
		//Console.WriteLine($"Puzzle #4 : {ilGpuPuzzle.Puzzle4().Check2D(new[,] {{ 10, 11 }, { 12, 13 }})}");
		//Console.WriteLine($"Puzzle #5 : {ilGpuPuzzle.Puzzle5().Check2D(new[,] {{ 0, 1 }, { 1, 2 }})}");
		//Console.WriteLine($"Puzzle DenseX : {ilGpuPuzzle.DenseX().Check2D(new[,] {{ 0, 1, 2 }, { 3, 4, 5 }})}");
		//Console.WriteLine($"Puzzle DenseY : {ilGpuPuzzle.DenseY().Check2D(new[,] {{ 0, 1, 2 }, { 3, 4, 5 }})}");
		//Console.WriteLine();

		//Console.WriteLine(header);
		//ILGpuDemo.MultiplyByTwoIntoSingleIntArray(useCpu);
		//Console.WriteLine(divider);
		//ILGpuDemo.Saxpy();
		//using var computeSharpPuzzle = ComputeSharpPuzzle.Create(useCpu);
		//Console.WriteLine(computeSharpPuzzle.Description);
		//Console.WriteLine(new string('-', computeSharpPuzzle.Description.Length));
		//Console.WriteLine($"Puzzle #1 : {computeSharpPuzzle.Puzzle1().Check1D([10, 11, 12, 13])}");
		//Console.WriteLine($"Puzzle #2 : {computeSharpPuzzle.Puzzle2().Check1D([0, 2, 4, 6])}");
		//Console.WriteLine($"Puzzle #3 : {computeSharpPuzzle.Puzzle3().Check1D([10, 11, 12, 13])}");
		//Console.WriteLine($"Puzzle #4 : {computeSharpPuzzle.Puzzle4().Check2D(new[,] {{ 10, 11 }, { 12, 13 }})}");
		//Console.WriteLine($"Puzzle #5 : {computeSharpPuzzle.Puzzle5().Check2D(new[,] { { 0, 1 }, { 1, 2 } })}");
		//Console.WriteLine();

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
		return $"{(result ? "✅" : "❌")} [{string.Join(",", inputArray)}] {(result ? "==" : "!=")} [{string.Join(",",expectedArray)}]";
	}

	//public static string Check2D<T>(this T[,] inputArray, T[,] expectedArray)
	//{
	//	static string arrayToString(T[,] array)
	//	{
	//		var rows = array.GetLength(0);
	//		var cols = array.GetLength(1);
	//		var result = new List<string>();

	//		for (int i = 0; i < rows; i++)
	//		{
	//			var row = new T[cols];
	//			for (int j = 0; j < cols; j++)
	//			{
	//				row[j] = array[i, j];
	//			}
	//			result.Add($"[{string.Join(",", row)}]");
	//		}

	//		return string.Join(",", result);
	//	}

	//	if (inputArray.GetLength(0) != expectedArray.GetLength(0) || inputArray.GetLength(1) != expectedArray.GetLength(1))
	//	{
	//		return $"❌ {arrayToString(inputArray)} != {arrayToString(expectedArray)}";
	//	}

	//	for (int i = 0; i < inputArray.GetLength(0); i++)
	//	{
	//		for (int j = 0; j < inputArray.GetLength(1); j++)
	//		{
	//			if (inputArray[i, j].Equals(expectedArray[i, j]))
	//			{
	//				return $"❌ {arrayToString(inputArray)} != {arrayToString(expectedArray)}";
	//			}
	//		}
	//	}

	//	return $"✅ {arrayToString(inputArray)} == {arrayToString(expectedArray)}";
	//}
}
