using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using ILGPU.Runtime.Cuda;

using System.Diagnostics;

namespace Test_ILGPU;

public static partial class ILGpuDemo
{
	public static void Saxpy()
	{
		const bool debug = false;
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

		using Context context = Context.Create(builder => builder.Default().EnableAlgorithms());
		using Accelerator accelerator = debug ? context.CreateCPUAccelerator(0) : context.CreateCudaAccelerator(0);
		const int gridSize = 1;
		var groupSize = accelerator.MaxNumThreadsPerGroup;
		var warpSize = accelerator.WarpSize;
		const string coreSize = "????";
		var acceleratorType = accelerator.AcceleratorType.ToString().ToUpper();
		var infoString = $"{acceleratorType} : {accelerator.Name} : CoreSize={coreSize}, GroupSize={groupSize}, WarpSize={warpSize}";
		Console.WriteLine(infoString);

		stopwatch = Stopwatch.StartNew();
		// allocate a gpu buffer on the device and copy the host data to it
		using var bufferG = accelerator.Allocate1D(arrayG);
		// create the kernel configuration
		var config = new KernelConfig(gridSize, Math.Clamp(length, warpSize, groupSize));
		// create the kernel
		var kernel = accelerator.LoadStreamKernel<int, ArrayView<SaxpyModel>, float>(SaxpyKernel);
		// execute the kernel
		kernel(config, length, bufferG.View, 13f);
		// wait for the kernel to finish
		accelerator.Synchronize();
		// copy the result back to the CPU
		bufferG.CopyToCPU(arrayG);

		Console.WriteLine($"-- GPU Elapsed:          {stopwatch.Elapsed}");
		Console.WriteLine($"AFTER: {arrayG[99_999]}");

		static void SaxpyKernel(int limit, ArrayView<SaxpyModel> buffer, float a)
		{
			var index = (Grid.IdxX * Group.DimX) + Group.IdxX;
			if (index < limit)
			{
				var item = buffer[index];
				item.S = (a * item.X) + item.Y;
				buffer[index] = item;
			}
		}
	}

	public static float[] Saxpy(int N, float a, float[] x, float[] y)
	{
		// create device context and accelerator
		using Context context = Context.Create(builder => builder.Default().EnableAlgorithms());
		using Accelerator accelerator = context.CreateCudaAccelerator(0);
		// allocate a gpu buffer on the device and copy the host data to it
		using var deviceX = accelerator.Allocate1D(x);
		using var deviceY = accelerator.Allocate1D(y);
		// create the kernel
		var kernel = accelerator.LoadAutoGroupedStreamKernel<Index1D, float, ArrayView<float>, ArrayView<float>>(SaxpyKernel);
		// execute the kernel
		kernel(N, a, deviceX.View, deviceY.View);
		// wait for the kernel to finish
		accelerator.Synchronize();
		// create a buffer on the host
		var hostArray = new float[N];
		// copy the result back to the CPU
		deviceY.CopyToCPU(hostArray);
		// return the result
		return hostArray;

		static void SaxpyKernel(Index1D index, float a, ArrayView<float> x, ArrayView<float> y)
		{
			var i = index.X;
			y[i] = (a * x[i]) + y[i];
		}
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

		using Context context = Context.Create(builder => builder.Default().EnableAlgorithms());
		using Accelerator accelerator = context.CreateCudaAccelerator(0);
		// create a buffer on the host
		var hostArray = list.ToArray();
		// allocate a gpu buffer on the device and copy the host data to it
		using var deviceArray = accelerator.Allocate1D(hostArray);
		// create the kernel
		var kernel = accelerator.LoadAutoGroupedStreamKernel<Index1D, ArrayView<SaxpyModel>>(SaxpyKernel);
		// execute the kernel
		kernel(N, deviceArray.View);
		// wait for the kernel to finish
		accelerator.Synchronize();
		// copy the result back to the CPU
		deviceArray.CopyToCPU(hostArray);
		// return the result
		var result = hostArray.Select(s => s.S).ToArray();
		return result;

		static void SaxpyKernel(Index1D index, ArrayView<SaxpyModel> buffer)
		{
			var i = index.X;
			var item = buffer[i];
			item.S = (item.A * item.X) + item.Y;
			buffer[i] = item;
		}
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