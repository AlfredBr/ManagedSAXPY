using ILGPU;
using ILGPU.Runtime;
using Throw;

namespace Test_ILGPU;

// Strides
// Demonstrate the difference between DenseX and DenseY strides
public partial class ILGpuPuzzle
{
	public int[,] DenseX()
	{
		_context.ThrowIfNull();
		_accelerator.ThrowIfNull();
		// create an input array on the host
		var arrayIn = new int[,] { { 0, 1, 2 }, { 3, 4, 5 } };
		// create an output array on the host based on the input array
		var arrayOut = new int[arrayIn.GetLength(0), arrayIn.GetLength(1)];
		// allocate a gpu buffer on the device and copy the host data to it
		using var bufferIn = _accelerator.Allocate2DDenseX<int>(arrayIn);
		// allocate a gpu buffer on the device and copy the host data to it
		using var bufferOut = _accelerator.Allocate2DDenseX<int>(arrayOut);
		// create the kernel
		var kernel = _accelerator.LoadAutoGroupedStreamKernel<Index2D, ArrayView2D<int, Stride2D.DenseX>, ArrayView2D<int, Stride2D.DenseX>>(stride);
		// execute the kernel
		kernel(bufferOut.IntExtent, bufferIn.View, bufferOut.View);
		// wait for the kernel to finish
		_accelerator.Synchronize();
		// copy the result back to the host
		bufferOut.CopyToCPU(arrayOut);
		// return the array
		return arrayOut;

		static void stride(Index2D limit, ArrayView2D<int, Stride2D.DenseX> bufferIn, ArrayView2D<int, Stride2D.DenseX> bufferOut)
		{
			var x = limit.X;
			var y = limit.Y;
			Interop.WriteLine("DenseX : {0},{1}", x, y);
			bufferOut[x, y] = bufferIn[x, y];
		}
	}
	public int[,] DenseY()
	{
		_context.ThrowIfNull();
		_accelerator.ThrowIfNull();
		// create an input array on the host
		var arrayIn = new int[,] { { 0, 1, 2 }, { 3, 4, 5 } };
		// create an output array on the host based on the input array
		var arrayOut = new int[arrayIn.GetLength(0), arrayIn.GetLength(1)];
		// allocate a gpu buffer on the device and copy the host data to it
		using var bufferIn = _accelerator.Allocate2DDenseY<int>(arrayIn);
		// allocate a gpu buffer on the device and copy the host data to it
		using var bufferOut = _accelerator.Allocate2DDenseY<int>(arrayOut);
		// create the kernel
		var kernel = _accelerator.LoadAutoGroupedStreamKernel<Index2D, ArrayView2D<int, Stride2D.DenseY>, ArrayView2D<int, Stride2D.DenseY>>(stride);
		// execute the kernel
		kernel(bufferOut.IntExtent, bufferIn.View, bufferOut.View);
		// wait for the kernel to finish
		_accelerator.Synchronize();
		// copy the result back to the host
		bufferOut.CopyToCPU(arrayOut);
		// return the array
		return arrayOut;

		static void stride(Index2D limit, ArrayView2D<int, Stride2D.DenseY> bufferIn, ArrayView2D<int, Stride2D.DenseY> bufferOut)
		{
			var x = limit.X;
			var y = limit.Y;
			Interop.WriteLine("DenseY : {0},{1}", x, y);
			bufferOut[x, y] = bufferIn[x, y];
		}
	}
}
