using ComputeSharp;
using Throw;

// Puzzle 1 - Map
// Implement a "kernel" (GPU function) that adds 10 to each position
// of vector a and stores it in vector out. You have 1 thread per position.

namespace Test_ComputeSharp
{
	public partial class ComputeSharpPuzzle : IDisposable
	{
		public int[] Puzzle1()
		{
			_graphicsDevice.ThrowIfNull();
			// allocate the host buffer and fill with source data
			var array = Enumerable.Range(0, 4).ToArray();
			// allocate the device buffer and copy the host buffer into it
			using var buffer = _graphicsDevice.AllocateReadWriteBuffer(array);
			// the shader will modify the items in-place, so we can define a single read-write buffer to work on
			var kernel = new Puzzle1.AddTen(buffer);
			// execute the kernel
			_graphicsDevice.For(array.Length, kernel);
			// copy the result back to the host
			buffer.CopyTo(array);
			// return the modified array
			return array;
		}
	}
}

namespace Puzzle1
{
	[AutoConstructor]
	internal readonly partial struct AddTen : IComputeShader
	{
		private readonly ReadWriteBuffer<int> buffer;
		public readonly void Execute()
		{
			var index = ThreadIds.X;
			buffer[index] += 10;
		}
	}
}