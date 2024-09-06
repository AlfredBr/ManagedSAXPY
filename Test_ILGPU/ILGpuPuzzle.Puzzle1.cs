using ILGPU;
using ILGPU.Runtime;
using Throw;

namespace Test_ILGPU;

// Puzzle 1 - Map
// Implement a "kernel" (GPU function) that adds 10 to each position
// of vector a and stores it in vector out. You have 1 thread per position.

public partial class ILGpuPuzzle : IDisposable
{
    public int[] Puzzle1()
    {
        _context.ThrowIfNull();
        _accelerator.ThrowIfNull();
        // create an array on the host
        var array = Enumerable.Range(0, 4).ToArray();
        // allocate a gpu buffer on the device and copy the host data to it
        using var buffer = _accelerator.Allocate1D(array);
        // create the kernel configuration
        var config = new KernelConfig(_gridSize, Math.Clamp(array.Length, _warpSize, _groupSize));
        // create the kernel
        var kernel = _accelerator.LoadStreamKernel<int, ArrayView<int>>(kernelFunc);
        // execute the kernel
        kernel(config, array.Length, buffer.View);
        // wait for the kernel to finish
        _accelerator.Synchronize();
        // copy the result back to the host
        buffer.CopyToCPU(array);
        // return the array
        return array;

        static void kernelFunc(int limit, ArrayView<int> buffer)
        {
            var x = (Grid.IdxX * Group.DimX) + Group.IdxX;
            if (x < limit)
            {
                buffer[x] += 10;
            }
        }
    }
}
