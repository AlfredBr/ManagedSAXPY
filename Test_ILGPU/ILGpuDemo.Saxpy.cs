using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.Cuda;

namespace Test_ILGPU;

public static class ILGpuDemo
{
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
}
