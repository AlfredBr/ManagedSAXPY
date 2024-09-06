using ComputeSharp;

namespace Test_ComputeSharp;

public static partial class ComputeSharpDemo
{
	public static void MultiplyByTwoIntoSingleIntArray(bool debug = false)
	{
		// Get some sample data

		// Get the default graphics device
		var graphicsDevice = debug ? /*cpu*/ GraphicsDevice.EnumerateDevices().ToArray()[2] : /*cuda*/ GraphicsDevice.GetDefault();
		const string groupSize = "XXXX";
		var warpSize = graphicsDevice.WavefrontSize;
		var coreSize = graphicsDevice.ComputeUnits;
		var acceleratorType = graphicsDevice.IsHardwareAccelerated && !debug ? "CUDA" : "CPU";
		var infoString = $"{acceleratorType} : {graphicsDevice.Name} : CoreSize={coreSize}, GroupSize={groupSize}, WarpSize={warpSize}";
		Console.WriteLine(infoString);

		const int length = 10;
		// create a buffer of integers on the host
		var hostA = Enumerable.Range(1, length).ToArray();
		// allocate a gpu buffer on the device and copy the host data to it
		using ReadWriteBuffer<int> deviceA = graphicsDevice.AllocateReadWriteBuffer(hostA);
		// the shader will modify the items in-place, so we can define a single read-write buffer to work on
		var kernel = new MultiplyByTwo(deviceA);
		// execute the kernel
		graphicsDevice.For(length, kernel);

		Console.WriteLine("BEFORE");
		Console.WriteLine($"  a[] = {string.Join(", ", hostA.Select(f => f.ToString("D2")))}");
		deviceA.CopyTo(hostA);
		Console.WriteLine("AFTER");
		Console.WriteLine($"  a[] = {string.Join(", ", hostA.Select(f => f.ToString("D2")))}");
	}
}

[AutoConstructor]
internal readonly partial struct MultiplyByTwo : IComputeShader
{
	private readonly ReadWriteBuffer<int> buffer;
	public readonly void Execute()
	{
		buffer[ThreadIds.X] *= 2;
	}
}
