using ILGPU;
using ILGPU.Runtime;

using Throw;

namespace Test_ILGPU;

// TODO: Puzzle 5 - Broadcast
// Implement a kernel that adds a and b and stores it in out.
// Inputs a and b are vectors. a is a row vector, b is a column vector.
// out is a 2x2 matrix.  You have more threads than positions.
public partial class ILGpuPuzzle : IDisposable
{
	public int[,] Puzzle5()
	{
		_context.ThrowIfNull();
		_accelerator.ThrowIfNull();
		// create an input array on the host
		var arrayA = new int[,] { { 0 }, { 1 } };
		var arrayB = new int[,] { { 0, 1 } };
		// create an output array on the host based on the input array
		var arrayOut = new int[arrayA.GetLength(0), arrayB.GetLength(1)];
		// allocate a gpu buffer on the device and copy the host data to it
		using var bufferA = _accelerator.Allocate2DDenseX<int>(arrayA);
		using var bufferB = _accelerator.Allocate2DDenseY<int>(arrayB);
		// allocate a gpu buffer on the device and copy the host data to it
		using var bufferOut = _accelerator.Allocate2DDenseX<int>(arrayOut);
		// create the kernel configuration, this time with 2D extents
		var config = new KernelConfig(bufferA.IntExtent, bufferB.IntExtent);
		// create the kernel
		var kernel = _accelerator.LoadStreamKernel<Index2D, ArrayView2D<int, Stride2D.DenseX>, ArrayView2D<int, Stride2D.DenseY>, ArrayView2D<int, Stride2D.DenseX>>(sum2DArray);
		// execute the kernel, config required because the kernel is not auto grouped
		kernel(config, bufferOut.IntExtent, bufferA.View, bufferB.View, bufferOut.View);
		// wait for the kernel to finish
		_accelerator.Synchronize();
		// copy the result back to the host
		bufferOut.CopyToCPU(arrayOut);
		// return the array
		return arrayOut;

		static void sum2DArray(Index2D limit, ArrayView2D<int, Stride2D.DenseX> bufferA, ArrayView2D<int, Stride2D.DenseY> bufferB, ArrayView2D<int, Stride2D.DenseX> bufferOut)
		{
			var x = (Grid.IdxX * Group.DimX) + Group.IdxX;
			var y = (Grid.IdxY * Group.DimY) + Group.IdxY;
			if (x < limit.X && y < limit.Y)
			{
				bufferOut[x, y] = bufferA[x, y] + bufferB[x, y];
			}
		}
	}
}
