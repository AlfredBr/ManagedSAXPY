using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ManagedCuda;

using Throw;

namespace Test_ManagedCUDA;

public static partial class ManagedCUDADemo
{
	public static void VectorAdd(int N = 100_000, bool _ = false)
	{
		Console.WriteLine("ManagedCUDADemo");

		const int deviceID = 0;
		var context = new CudaContext(deviceID);
		const string ptxFile = @"E:\Test.GPU\Test_App\bin\Debug\net8.0-windows\vectorAdd.ptx";
		File.Exists(ptxFile).Throw(_ => throw new FileNotFoundException(ptxFile)).IfFalse();
		var kernel = context.LoadKernel(ptxFile, "VectorAdd");
		kernel.GridDimensions = (N + 255) / 256;
		kernel.BlockDimensions = 256;

		// allocate input vectors hostA and hostB in host memory
		int[] hostA = Enumerable.Range(1, N).ToArray();
		int[] hostB = Enumerable.Range(2, N).ToArray();

		// allocate vectors in device memory and copy vectors from host memory to device memory
		CudaDeviceVariable<int> deviceA = hostA;
		CudaDeviceVariable<int> deviceB = hostB;
		CudaDeviceVariable<int> deviceC = new(N);
		deviceC.Memset(0);

		// invoke kernel
		kernel.Run(deviceA.DevicePointer, deviceB.DevicePointer, deviceC.DevicePointer, N);

		const int limit = 5;
		Console.WriteLine("BEFORE");
		Console.WriteLine($"  a[] = {string.Join(", ", hostA.Skip(Math.Max(0, hostA.Length - limit)).Take(limit).Select(f => f.ToString("D2")))}");
		Console.WriteLine($"  b[] = {string.Join(", ", hostB.Skip(Math.Max(0, hostB.Length - limit)).Take(limit).Select(f => f.ToString("D2")))}");

		// copy result from device memory to hostC (contains the result in host memory)
		int[] hostC = deviceC;

		Console.WriteLine("AFTER");
		Console.WriteLine($"  c[] = {string.Join(", ", hostC.Skip(Math.Max(0, hostC.Length - limit)).Take(limit).Select(f => f.ToString("D2")))}");
	}
}
