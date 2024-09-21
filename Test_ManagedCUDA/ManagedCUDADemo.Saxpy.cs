using ManagedCuda;
using ManagedCuda.CudaBlas;

using Throw;

namespace Test_ManagedCUDA;

public static class ManagedCUDADemo
{
	public static float[] Saxpy(int N, float A, float[] x, float[] y)
	{
		// create a new context
		const int deviceID = 0;
		var context = new PrimaryContext(deviceID);
		context.SetCurrent();
		// load ptx file
		const string ptxFile = @"E:\Test.GPU\Test_App\bin\Debug\net8.0-windows\saxpy.ptx";
		File.Exists(ptxFile).Throw(_ => throw new FileNotFoundException(ptxFile)).IfFalse();
		// load kernel
		var kernel = context.LoadKernel(ptxFile, "Saxpy");
		kernel.GridDimensions = (N + 255) / 256;
		kernel.BlockDimensions = 256;
		// allocate vectors in device memory and copy vectors from host memory to device memory
		CudaDeviceVariable<float> deviceX = x;
		CudaDeviceVariable<float> deviceY = y;
		CudaDeviceVariable<float> deviceS = new(N);
		deviceS.Memset(0);
		// invoke kernel
		kernel.Run(N, A, deviceS.DevicePointer, deviceX.DevicePointer, deviceY.DevicePointer);
		// copy result from device memory to hostC (contains the result in host memory)
		float[] hostS = deviceS;
		return hostS;
	}

	public static float[] CuBlasSaxpy(int _, float A, float[] x, float[] y)
	{
		// create a new CudaBlas instance
		var blas = new CudaBlas();
		// allocate device memory and copy data from host to device
		CudaDeviceVariable<float> deviceX = x;
		CudaDeviceVariable<float> deviceY = y;
		// perform SAXPY operation
		blas.Axpy(A, deviceX, 1, deviceY, 1);
		// copy result from device memory to host memory
		float[] hostArray = deviceY;
		// free device memory
		blas.Dispose();
		// return the result
		return hostArray;
	}
}
