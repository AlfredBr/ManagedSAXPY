using ComputeSharp;
using Throw;

// Puzzle 4 - Map 2D
// Implement a kernel that adds 10 to each position of a and stores it in out.
// Input a is 2D and square. You have more threads than positions.

namespace Test_ComputeSharp
{
	public partial class ComputeSharpPuzzle : IDisposable
	{
		public int[,] Puzzle4()
		{
			_graphicsDevice.ThrowIfNull();
			// allocate the host input buffer and fill with source data
			var arrayA = new int[,] { { 0, 1 }, { 2, 3 } };
			// allocate the host output buffer
			var arrayOut = new int[arrayA.GetLength(0), arrayA.GetLength(1)];
			// allocate the device input buffer and copy the host input buffer into it
			using var bufferA = _graphicsDevice.AllocateReadOnlyTexture2D(arrayA);
			// allocate the device output buffer
			using var bufferOut = _graphicsDevice.AllocateReadWriteTexture2D(arrayOut);
			// define the kernel function
			var kernel = new Puzzle4.Sum2DArray(bufferA, bufferOut);
			// execute the kernel
			_graphicsDevice.For(bufferOut.Width, bufferOut.Height, kernel);
			// copy the result back to the host
			bufferOut.CopyTo(arrayOut);
			// return the modified array
			return arrayOut;
		}
	}
}

namespace Puzzle4
{
	[AutoConstructor]
	internal readonly partial struct Sum2DArray : IComputeShader
	{
		private readonly ReadOnlyTexture2D<int> bufferA;
		private readonly ReadWriteTexture2D<int> bufferOut;
		public readonly void Execute()
		{
			var x = ThreadIds.X;
			var y = ThreadIds.Y;
			bufferOut[x, y] = bufferA[x, y] + 10;
		}
	}
}