using Throw;
using BenchmarkDotNet.Attributes;
using Test_Library;
using Test_ComputeSharp;
using Test_ILGPU;
using Test_ManagedCUDA;
//using CppClassLibrary1;

namespace SaxpyBenchmarks;

public class SaxpyTests
{
	const int N = (4_096 * 1_024) - 64;
	const float A = 2.0f;
	private readonly float[] arrayX;
	private readonly float[] arrayY;

	public SaxpyTests()
	{
		const string ptxFile = @"E:\ManagedSAXPY\CudaRuntime1\x64\Release\saxpy.ptx";
		File.Exists(ptxFile).Throw(_ => throw new FileNotFoundException(ptxFile)).IfFalse();
		File.Copy(ptxFile, Path.Combine(Environment.CurrentDirectory, "saxpy.ptx"), true);

		arrayX = Enumerable.Range(1, N).Select(x => (float)x).ToArray();
		arrayY = Enumerable.Range(2, N).Select(y => (float)y).ToArray();
	}

	[Benchmark] public float[] ForLoop_Saxpy() => ForLoop.Saxpy(N, A, arrayX, arrayY);
	[Benchmark] public float[] ParallelFor_Saxpy() => ParallelFor.Saxpy(N, A, arrayX, arrayY);
	[Benchmark] public float[] Linq_Saxpy() => Linq.Saxpy(N, A, arrayX, arrayY);
	[Benchmark] public float[] Intrinsics_SaxpyVector() => Intrinsics.SaxpyVector(N, A, arrayX, arrayY);
	[Benchmark] public float[] Intrinsics_SaxpyMultiplyAdd() => Intrinsics.SaxpyMultiplyAdd(N, A, arrayX, arrayY);
	[Benchmark] public float[] Intrinsics_Saxpy128() => Intrinsics.Saxpy128(N, A, arrayX, arrayY);
	[Benchmark] public float[] Intrinsics_Saxpy256() => Intrinsics.Saxpy256(N, A, arrayX, arrayY);
	[Benchmark] public float[] ComputeSharp_Saxpy1() => ComputeSharpDemo.Saxpy1(N, A, arrayX, arrayY);
	[Benchmark] public float[] ComputeSharp_Saxpy2() => ComputeSharpDemo.Saxpy2(N, A, arrayX, arrayY);
	[Benchmark] public float[] ILGpu_Saxpy() => ILGpuDemo.Saxpy(N, A, arrayX, arrayY);
	[Benchmark] public float[] ManagedCUDA_Saxpy() => ManagedCUDADemo.Saxpy(N, A, arrayX, arrayY);
	[Benchmark] public float[] ManagedCUDA_Fma() => ManagedCUDADemo.Fma(N, A, arrayX, arrayY);
	[Benchmark] public float[] ManagedCUDA_CuBlas_Saxpy() => ManagedCUDADemo.CuBlasSaxpy(N, A, arrayX, arrayY);

	//[Benchmark] public float[] CppClass1_Saxpy1() => CppClass1.Saxpy1(N, A, arrayX, arrayY);
	//[Benchmark] public float[] CppClass1_Saxpy2() => CppClass1.Saxpy2(N, A, arrayX, arrayY);
}
