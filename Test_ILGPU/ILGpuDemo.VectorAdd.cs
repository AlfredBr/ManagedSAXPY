using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using ILGPU.Runtime.Cuda;

namespace Test_ILGPU;

public static partial class ILGpuDemo
{
	/// <summary>
	/// int VectorAdd()
	/// </summary>
	/// <param name="debug"></param>
	public static void VectorAdd(int N = 100_000, bool debug = false)
	{
		static void AddKernel(Index1D index, ArrayView<int> a, ArrayView<int> b, ArrayView<int> c)
		{
			var x = index.X;
			c[x] = a[x] + b[x];
		}

		using Context context = Context.Create(builder => builder.Default().EnableAlgorithms());
		using Accelerator accelerator = debug ? context.CreateCPUAccelerator(0) : context.CreateCudaAccelerator(0);
		var acceleratorType = accelerator.AcceleratorType.ToString().ToUpper();
		var infoString = $"ILGpuDemo : {acceleratorType} : {accelerator.Name}";
		Console.WriteLine(infoString);
		//Console.WriteLine(GetInfoString(accelerator));

		// create a buffer of integers on the host
		var hostA = Enumerable.Range(1, N).ToArray();
		var hostB = Enumerable.Range(2, N).ToArray();
		var hostC = Enumerable.Repeat(-1, N).ToArray();
		// allocate a gpu buffer on the device and copy the host data to it
		using var deviceA = accelerator.Allocate1D(hostA);
		using var deviceB = accelerator.Allocate1D(hostB);
		using var deviceC = accelerator.Allocate1D(hostC);
		// create the kernel
		var kernel = accelerator.LoadAutoGroupedStreamKernel<Index1D, ArrayView<int>, ArrayView<int>, ArrayView<int>>(AddKernel);
		// execute the kernel
		kernel(N, deviceA.View, deviceB.View, deviceC.View);
		// wait for the kernel to finish
		accelerator.Synchronize();

		const int limit = 5;
		Console.WriteLine("BEFORE");
		Console.WriteLine($"  a[] = {string.Join(", ", hostA.Skip(Math.Max(0, hostA.Length - limit)).Take(limit).Select(f => f.ToString("D2")))}");
		Console.WriteLine($"  b[] = {string.Join(", ", hostB.Skip(Math.Max(0, hostB.Length - limit)).Take(limit).Select(f => f.ToString("D2")))}");

		// copy the result back to the CPU
		deviceC.CopyToCPU(hostC);

		Console.WriteLine("AFTER");
		Console.WriteLine($"  c[] = {string.Join(", ", hostC.Skip(Math.Max(0, hostC.Length - limit)).Take(limit).Select(f => f.ToString("D2")))}");
	}
}