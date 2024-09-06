using ComputeSharp;
using Throw;

// Puzzle 2 - Zip
// Implement a kernel that adds together each position of a and b and
// stores it in out. You have 1 thread per position.

namespace Test_ComputeSharp
{
	public partial class ComputeSharpPuzzle : IDisposable
	{
		public int[] Puzzle2()
		{
			_graphicsDevice.ThrowIfNull();
			// allocate the host buffer and fill with source data
			const int len = 4;
			var arrayA = Enumerable.Range(0, len).ToArray();
			var arrayB = Enumerable.Range(0, len).ToArray();
			var arrayOut = new int[len];
			// allocate the device buffer and copy the host buffer into it
			using var bufferA = _graphicsDevice.AllocateReadOnlyBuffer(arrayA);
			using var bufferB = _graphicsDevice.AllocateReadOnlyBuffer(arrayB);
			using var bufferOut = _graphicsDevice.AllocateReadWriteBuffer(arrayOut);
			// the shader will modify the items in-place, so we can define a single read-write buffer to work on
			var kernel = new Puzzle2.SumTwoArrays(bufferA, bufferB, bufferOut);
			// execute the kernel
			_graphicsDevice.For(len, kernel);
			// copy the result back to the host
			bufferOut.CopyTo(arrayOut);
			// return the modified array
			return arrayOut;
		}
	}
}

namespace Puzzle2
{
	[AutoConstructor]
	internal readonly partial struct SumTwoArrays : IComputeShader
	{
		private readonly ReadOnlyBuffer<int> bufferA;
		private readonly ReadOnlyBuffer<int> bufferB;
		private readonly ReadWriteBuffer<int> bufferOut;
		public readonly void Execute()
		{
			var index = ThreadIds.X;
			bufferOut[index] = bufferA[index] + bufferB[index];
		}
	}
}