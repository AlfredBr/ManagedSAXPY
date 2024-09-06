using ComputeSharp;
using Throw;

// Puzzle 3 - Guards
// Implement a kernel that adds 10 to each position of a and
// stores it in out. You have more threads than positions.

namespace Test_ComputeSharp
{
	public partial class ComputeSharpPuzzle : IDisposable
	{
		public int[] Puzzle3()
		{
			_graphicsDevice.ThrowIfNull();
			// allocate the host buffer and fill with source data
			const int len = 4;
			var arrayA = Enumerable.Range(0, len).ToArray();
			var arrayOut = new int[len];
			// allocate the device buffer and copy the host buffer into it
			using var bufferA = _graphicsDevice.AllocateReadOnlyBuffer(arrayA);
			using var bufferOut = _graphicsDevice.AllocateReadWriteBuffer(arrayOut);
			// the shader will modify the items in-place, so we can define a single read-write buffer to work on
			var kernel = new Puzzle3.AddTenAndStore(bufferA, bufferOut);
			// execute the kernel
			_graphicsDevice.For(len, kernel);
			// copy the result back to the host
			bufferOut.CopyTo(arrayOut);
			// return the modified array
			return arrayOut;
		}
	}
}

namespace Puzzle3
{
	[AutoConstructor]
	internal readonly partial struct AddTenAndStore : IComputeShader
	{
		private readonly ReadOnlyBuffer<int> bufferA;
		private readonly ReadWriteBuffer<int> bufferOut;
		public readonly void Execute()
		{
			var index = ThreadIds.X;
			bufferOut[index] = bufferA[index] + 10;
		}
	}
}
