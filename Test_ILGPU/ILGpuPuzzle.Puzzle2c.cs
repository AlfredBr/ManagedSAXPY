using ILGPU;
using ILGPU.Runtime;
using Throw;

namespace Test_ILGPU;

// Puzzle 2 - Zip
// Implement a kernel that adds together each position of a and b and
// stores it in out. You have 1 thread per position.

public partial class ILGpuPuzzle : IDisposable
{
    public int[] Puzzle2c()
    {
        _context.ThrowIfNull();
        _accelerator.ThrowIfNull();
        // create an array on the host
        const int len = 4;
        var arrayA = Enumerable.Range(0, len).ToArray();
        var arrayB = Enumerable.Range(0, len).ToArray();
        var arrayOut = new int[len];
        // allocate a gpu buffer on the device and copy the host data to it
        using var bufferA = _accelerator.Allocate1D(arrayA);
        using var bufferB = _accelerator.Allocate1D(arrayB);
        using var bufferOut = _accelerator.Allocate1D(arrayOut);
        // create the kernel configuration
        var config = new KernelConfig(_gridSize, Math.Clamp(len, _warpSize, _groupSize));
        // launch the kernel, according to config settings
        _accelerator.Launch<Index1D, ArrayView<int>, ArrayView<int>, ArrayView<int>>(sumTwoArrays, config, bufferOut.IntExtent, bufferA.View, bufferB.View, bufferOut.View);
        // wait for the kernel to finish
        _accelerator.Synchronize();
        // copy the result back to the host
        bufferOut.CopyToCPU(arrayOut);
        // return the array
        return arrayOut;

        static void sumTwoArrays(Index1D limit, ArrayView<int> bufferA, ArrayView<int> bufferB, ArrayView<int> bufferOut)
        {
            var x = (Grid.IdxX * Group.DimX) + Group.IdxX;
            if (x < limit)
            {
                bufferOut[x] = bufferA[x] + bufferB[x];
            }
        }
    }
}
