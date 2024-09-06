using ComputeSharp;
using System.Diagnostics;

using TerraFX.Interop.Windows;

namespace Test_ComputeSharp;

public static partial class ComputeSharpDemo
{
	public static void Saxpy()
	{
		var list = new List<SaxpyModel>();
		const int length = 100_000;
		for (int i = 0; i < length; i++)
		{
			list.Add(new SaxpyModel
			{
				A = 13,
				X = i * MathF.PI,
				Y = MathF.Sqrt(i)
			});
		}

		var arrayN = list.ToArray();
		var arrayP = list.ToArray();
		var arrayG = list.ToArray();

		var stopwatch = Stopwatch.StartNew();
		for (int i = 0; i < length; i++)
		{
			arrayN[i].S = (arrayN[i].A * arrayN[i].X) + arrayN[i].Y;
		}
		Console.WriteLine($"-- Normal CPU Elapsed:   {stopwatch.Elapsed}");
		Console.WriteLine($"AFTER: {arrayN[99_999]}");

		stopwatch = Stopwatch.StartNew();
		Parallel.For(0, length, i => arrayP[i].S = (arrayP[i].A * arrayP[i].X) + arrayP[i].Y);
		Console.WriteLine($"-- Parallel CPU Elapsed: {stopwatch.Elapsed}");
		Console.WriteLine($"AFTER: {arrayP[99_999]}");

		stopwatch = Stopwatch.StartNew();
		using ReadWriteBuffer<SaxpyModel> gpuBuffer = GraphicsDevice.GetDefault().AllocateReadWriteBuffer(arrayG);
		var device = GraphicsDevice.GetDefault();
		device.For(length, new SaxpyCompute(13, gpuBuffer));
		Console.WriteLine($"-- GPU Elapsed:          {stopwatch.Elapsed}");
		gpuBuffer.CopyTo(arrayG);
		Console.WriteLine($"AFTER: {arrayG[99_999]}");
	}

	public static float[] Saxpy(int N, float a, float[] x, float[] y)
	{
		var device = GraphicsDevice.GetDefault();
		using ReadWriteBuffer<float> deviceX = device.AllocateReadWriteBuffer(x);
		using ReadWriteBuffer<float> deviceY = device.AllocateReadWriteBuffer(y);
		device.For(N, new SaxpyKernel(a, deviceX, deviceY));
		var hostBuffer = new float[N];
		deviceY.CopyTo(hostBuffer);
		return hostBuffer;
	}

	public static float[] SaxpyObj(int N, float a, float[] x, float[] y)
	{
		var list = new List<SaxpyModel>();
		for (int i = 0; i < N; i++)
		{
			list.Add(new SaxpyModel
			{
				A = a,
				X = x[i],
				Y = y[i]
			});
		}
		var device = GraphicsDevice.GetDefault();
		var hostBuffer = list.ToArray();
		using ReadWriteBuffer<SaxpyModel> deviceBuffer = device.AllocateReadWriteBuffer(hostBuffer);
		device.For(N, new SaxpyCompute(a, deviceBuffer));
		deviceBuffer.CopyTo(hostBuffer);
		var result = hostBuffer.Select(s => s.S).ToArray();
		return result;
	}
}

[AutoConstructor]
[System.Runtime.Versioning.SupportedOSPlatform("windows")]
internal readonly partial struct SaxpyKernel : IComputeShader
{
	private readonly float a;
	private readonly ReadWriteBuffer<float> x;
	private readonly ReadWriteBuffer<float> y;

	public void Execute()
	{
		var index = ThreadIds.X;
		y[index] = (a * x[index]) + y[index];
	}
}

[AutoConstructor]
[System.Runtime.Versioning.SupportedOSPlatform("windows")]
internal readonly partial struct SaxpyCompute : IComputeShader
{
	private readonly float a;
	private readonly ReadWriteBuffer<SaxpyModel> buffer;

	public void Execute()
	{
		var saxpy = this.buffer[ThreadIds.X];
		saxpy.A = a;
		saxpy.S = (saxpy.A * saxpy.X) + saxpy.Y;
		this.buffer[ThreadIds.X] = saxpy;
	}
}
internal struct SaxpyModel
{
	public float S;
	public float A;
	public float X;
	public float Y;

	public readonly override string ToString()
	{
		return $"{S:0.00} = {A:0.00} * {X:0.00} + {Y:0.00}";
	}
}