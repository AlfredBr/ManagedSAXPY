using ILGPU;
using ILGPU.Runtime;
using Throw;

namespace Test_ILGPU;

// Puzzle 4 - Map 2D
// Implement a kernel that adds 10 to each position of a and stores it in out.
// Input a is 2D and square. You have more threads than positions.

public partial class ILGpuPuzzle : IDisposable
{
	public int[,] Puzzle4()
	{
		_context.ThrowIfNull();
		_accelerator.ThrowIfNull();
		// create an input array on the host
		var arrayA = new int[,] { { 0, 1 }, { 2, 3 } };
		// create an output array on the host based on the input array
		var arrayOut = new int[arrayA.GetLength(0), arrayA.GetLength(1)];
		// allocate a gpu buffer on the device and copy the host data to it
		using var bufferA = _accelerator.Allocate2DDenseY<int>(arrayA);
		// allocate a gpu buffer on the device and copy the host data to it
		using var bufferOut = _accelerator.Allocate2DDenseY<int>(arrayOut);
		// create the kernel configuration, this time with 2D extents
		var config = new KernelConfig(bufferA.IntExtent, bufferOut.IntExtent);
		// create the kernel
		var kernel = _accelerator.LoadStreamKernel<Index2D, ArrayView2D<int, Stride2D.DenseY>, ArrayView2D<int, Stride2D.DenseY>>(addTenAndStore);
		// execute the kernel, config required because the kernel is not auto grouped
		kernel(config, bufferOut.IntExtent, bufferA.View, bufferOut.View);
		// wait for the kernel to finish
		_accelerator.Synchronize();
		// copy the result back to the host
		bufferOut.CopyToCPU(arrayOut);
		// return the array
		return arrayOut;

		static void addTenAndStore(Index2D limit, ArrayView2D<int, Stride2D.DenseY> bufferA, ArrayView2D<int, Stride2D.DenseY> bufferOut)
		{
			var x = (Grid.IdxX * Group.DimX) + Group.IdxX;
			var y = (Grid.IdxY * Group.DimY) + Group.IdxY;
			if (x < limit.X && y < limit.Y)
			{
				bufferOut[x,y] = bufferA[x,y] + 10;
			}
		}
	}
}
