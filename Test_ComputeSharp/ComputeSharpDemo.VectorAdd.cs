using ComputeSharp;

namespace Test_ComputeSharp;

public static partial class ComputeSharpDemo
{
	public static void VectorAdd(int N = 100_000, bool debug = false)
	{
		// Get the default graphics device
		var graphicsDevice = debug ? /*cpu*/ GraphicsDevice.EnumerateDevices().ToArray()[2] : /*cuda*/ GraphicsDevice.GetDefault();
		var acceleratorType = graphicsDevice.IsHardwareAccelerated && !debug ? "CUDA" : "CPU";
		var infoString = $"ComputeSharpDemo: {acceleratorType} : {graphicsDevice.Name}";
		Console.WriteLine(infoString);

		// create a buffer of integers on the host
		var hostA = Enumerable.Range(1, N).ToArray();
		var hostB = Enumerable.Range(2, N).ToArray();
		var hostC = Enumerable.Repeat(-1, N).ToArray();
		// allocate a gpu buffer on the device and copy the host data to it
		using ReadWriteBuffer<int> deviceA = graphicsDevice.AllocateReadWriteBuffer(hostA);
		using ReadWriteBuffer<int> deviceB = graphicsDevice.AllocateReadWriteBuffer(hostB);
		using ReadWriteBuffer<int> deviceC = graphicsDevice.AllocateReadWriteBuffer(hostC);
		// create the kernel
		var kernel = new VectorAdd(deviceA, deviceB, deviceC);
		// execute the kernel
		graphicsDevice.For(N, kernel);

		const int limit = 5;
		Console.WriteLine("BEFORE");
		Console.WriteLine($"  a[] = {string.Join(", ", hostA.Skip(Math.Max(0, hostA.Length - limit)).Take(limit).Select(f => f.ToString("D2")))}");
		Console.WriteLine($"  b[] = {string.Join(", ", hostB.Skip(Math.Max(0, hostB.Length - limit)).Take(limit).Select(f => f.ToString("D2")))}");

		// copy the result back to the CPU
		deviceC.CopyTo(hostC);

		Console.WriteLine("AFTER");
		Console.WriteLine($"  c[] = {string.Join(", ", hostC.Skip(Math.Max(0, hostC.Length - limit)).Take(limit).Select(f => f.ToString("D2")))}");
	}
}

[AutoConstructor]
internal readonly partial struct VectorAdd : IComputeShader
{
	private readonly ReadWriteBuffer<int> bufferA;
	private readonly ReadWriteBuffer<int> bufferB;
	private readonly ReadWriteBuffer<int> bufferC;
	public readonly void Execute()
	{
		bufferC[ThreadIds.X] = bufferA[ThreadIds.X] + bufferB[ThreadIds.X];
	}
}
