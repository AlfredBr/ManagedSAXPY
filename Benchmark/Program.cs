using System;
using System.Security.Cryptography;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using CppClassLibrary1;

using Test_App;

using Test_ComputeSharp;

using Test_ILGPU;

using Test_Library;

using Test_ManagedCUDA;

namespace SaxpyBenchmarks;

public static class Program
{
	public static void Main(string[] args)
	{
		//BenchmarkRunner.Run<Md5VsSha256>();
		BenchmarkRunner.Run<SaxpyTests>();
	}
}

public class Md5VsSha256
{
	private const int N = 10000;
	private readonly byte[] data;

	private readonly SHA256 sha256 = SHA256.Create();
	private readonly MD5 md5 = MD5.Create();

	public Md5VsSha256()
	{
		data = new byte[N];
		new Random(42).NextBytes(data);
	}

	[Benchmark]
	public byte[] Sha256() => sha256.ComputeHash(data);

	[Benchmark]
	public byte[] Md5() => md5.ComputeHash(data);
}

public class SaxpyTests
{
	const int N = (4_096 * 1_024) - 64;
	const float A = 2.0f;
	private readonly float[] arrayX;
	private readonly float[] arrayY;

	public SaxpyTests()
	{
		arrayX = Enumerable.Range(1, N).Select(x => (float)x).ToArray();
		arrayY = Enumerable.Range(2, N).Select(y => (float)y).ToArray();
	}

	[Benchmark]	public float[] ForLoop() => ForLoopDemo.Saxpy(N, A, arrayX, arrayY);
	[Benchmark]	public float[] ParallelForLoop() => ParallelFor.Saxpy(N, A, arrayX, arrayY);
	[Benchmark]	public float[] Linq() => LinqDemo.Saxpy(N, A, arrayX, arrayY);
	[Benchmark]	public float[] IntrinsicsSaxpyVector() => Intrinsics.SaxpyVector(N, A, arrayX, arrayY);
	[Benchmark]	public float[] IntrinsicsSaxpyMultiplyAdd() => Intrinsics.SaxpyMultiplyAdd(N, A, arrayX, arrayY);
	[Benchmark]	public float[] IntrinsicsSaxpy128() => Intrinsics.Saxpy128(N, A, arrayX, arrayY);
	[Benchmark]	public float[] IntrinsicsSaxpy256() => Intrinsics.Saxpy256(N, A, arrayX, arrayY);
	[Benchmark]	public float[] ComputeSharpDemoSaxpy1() => ComputeSharpDemo.Saxpy1(N, A, arrayX, arrayY);
	[Benchmark]	public float[] ComputeSharpDemoSaxpy2() => ComputeSharpDemo.Saxpy2(N, A, arrayX, arrayY);
	[Benchmark]	public float[] ILGpuDemoSaxpy() => ILGpuDemo.Saxpy(N, A, arrayX, arrayY);
	[Benchmark]	public float[] ManagedCUDADemoSaxpy() => ManagedCUDADemo.Saxpy(N, A, arrayX, arrayY);
	[Benchmark]	public float[] ManagedCUDADemoCuBlasSaxpy() => ManagedCUDADemo.CuBlasSaxpy(N, A, arrayX, arrayY);
	[Benchmark]	public float[] CppClass1Saxpy1() => CppClass1.Saxpy1(N, A, arrayX, arrayY);
	[Benchmark]	public float[] CppClass1Saxpy2() => CppClass1.Saxpy2(N, A, arrayX, arrayY);
}
