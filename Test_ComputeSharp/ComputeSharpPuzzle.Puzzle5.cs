using ComputeSharp;

using Throw;

// Puzzle 5 - Broadcast
// Implement a kernel that adds a and b and stores it in out.
// Inputs a and b are vectors. a is a row vector, b is a column vector.
// out is a 2x2 matrix.  You have more threads than positions.

namespace Test_ComputeSharp
{
	public partial class ComputeSharpPuzzle : IDisposable
	{
		public int[,] Puzzle5()
		{
			_graphicsDevice.ThrowIfNull();
			// allocate the host input buffer and fill with source data
			var arrayA = new int[,] { { 0 }, { 1 } };
			var arrayB = new int[,] { { 0, 1 } };
			// allocate the host output buffer
			var arrayOut = new int[arrayA.GetLength(0), arrayB.GetLength(1)];
			// allocate the device input buffer and copy the host input buffer into it
			using var bufferA = _graphicsDevice.AllocateReadOnlyTexture2D(arrayA);
			using var bufferB = _graphicsDevice.AllocateReadOnlyTexture2D(arrayB);
			// allocate the device output buffer
			using var bufferOut = _graphicsDevice.AllocateReadWriteTexture2D(arrayOut);
			// define the kernel function
			var kernel = new Puzzle5.Sum2DArray(bufferA, bufferB, bufferOut);
			// execute the kernel
			_graphicsDevice.For(bufferOut.Width, bufferOut.Height, kernel);
			// copy the result back to the host
			bufferOut.CopyTo(arrayOut);
			// return the modified array
			return arrayOut;
		}
	}
}

namespace Puzzle5
{
	[AutoConstructor]
	internal readonly partial struct Sum2DArray : IComputeShader
	{
		private readonly ReadOnlyTexture2D<int> bufferA;
		private readonly ReadOnlyTexture2D<int> bufferB;
		private readonly ReadWriteTexture2D<int> bufferOut;
		public readonly void Execute()
		{
			var x = ThreadIds.X;
			var y = ThreadIds.Y;
			bufferOut[x, y] = bufferA[x, y] + bufferB[x, y];
		}
	}
}