using ILGPU;
using ILGPU.Runtime;
using Throw;

namespace Test_ILGPU;

// Puzzle 3 - Guards
// Implement a kernel that adds 10 to each position of a and
// stores it in out. You have more threads than positions.

public partial class ILGpuPuzzle : IDisposable
{
	public int[] Puzzle3()
	{
		_context.ThrowIfNull();
		_accelerator.ThrowIfNull();
		// create an array on the host
		const int len = 4;
		var arrayA = Enumerable.Range(0, len).ToArray();
		var arrayOut = new int[len];
		// allocate a gpu buffer on the device and copy the host data to it
		using var bufferA = _accelerator.Allocate1D(arrayA);
		using var bufferOut = _accelerator.Allocate1D(arrayOut);
		// create the kernel configuration
		var config = new KernelConfig(_gridSize, Math.Clamp(len, _warpSize, _groupSize));
		// create the kernel
		var kernel = _accelerator.LoadStreamKernel<Index1D, ArrayView<int>, ArrayView<int>>(addTenAndStore);
		// execute the kernel, config required because the kernel is not auto grouped
		kernel(config, bufferOut.IntExtent, bufferA.View, bufferOut.View);
		// wait for the kernel to finish
		_accelerator.Synchronize();
		// copy the result back to the host
		bufferOut.CopyToCPU(arrayOut);
		// return the array
		return arrayOut;

		static void addTenAndStore(Index1D limit, ArrayView<int> bufferA, ArrayView<int> bufferOut)
		{
			var x = (Grid.IdxX * Group.DimX) + Group.IdxX;
			if (x < limit)
			{
				bufferOut[x] = bufferA[x] + 10;
			}
		}
	}
}
