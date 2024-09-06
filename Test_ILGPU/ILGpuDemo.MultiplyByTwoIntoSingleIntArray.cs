using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using ILGPU.Runtime.Cuda;

namespace Test_ILGPU;

public static partial class ILGpuDemo
{
	public static void MultiplyByTwoIntoSingleIntArray(bool debug = false)
	{
		using Context context = Context.Create(builder => builder.Default().EnableAlgorithms());
		using Accelerator accelerator = debug ? context.CreateCPUAccelerator(0) : context.CreateCudaAccelerator(0);
		const int gridSize = 1;
		var groupSize = accelerator.MaxNumThreadsPerGroup;
		var warpSize = accelerator.WarpSize;
		const string coreSize = "????";
		var acceleratorType = accelerator.AcceleratorType.ToString().ToUpper();
		var infoString = $"{acceleratorType} : {accelerator.Name} : CoreSize={coreSize}, GroupSize={groupSize}, WarpSize={warpSize}";
		Console.WriteLine(infoString);

		const int length = 10;
		// create a buffer of integers on the host
		var hostA = Enumerable.Range(1, length).ToArray();
		// allocate a gpu buffer on the device and copy the host data to it
		using var deviceA = accelerator.Allocate1D(hostA);
		// create the kernel configuration
		var config = new KernelConfig(gridSize, Math.Clamp(length, warpSize, groupSize));
		// the kernel will modify the items in-place, so we can define a single read-write buffer to work on.
		var kernel = accelerator.LoadStreamKernel<int, ArrayView<int>>(MultiplyByTwo);
		// execute the kernel
		kernel(config, length, deviceA.View);
		// wait for the kernel to finish
		accelerator.Synchronize();

		Console.WriteLine("BEFORE");
		Console.WriteLine($"  a[] = {string.Join(", ", hostA.Select(f => f.ToString("D2")))}");

		// copy the result back to the CPU
		deviceA.CopyToCPU(hostA);
		Console.WriteLine("AFTER");
		Console.WriteLine($"  a[] = {string.Join(", ", hostA.Select(f => f.ToString("D2")))}");

		static void MultiplyByTwo(int limit, ArrayView<int> buffer)
		{
			var index = (Grid.IdxX * Group.DimX) + Group.IdxX;
			if (index < limit)
			{
				buffer[index] *= 2;
			}
		}
	}
}
