using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using ILGPU.Runtime.Cuda;

namespace Test_ILGPU;

public partial class ILGpuPuzzle : IDisposable
{
    private static Context? _context;
    private static Accelerator? _accelerator;
    private static string _description = string.Empty;
    private static int _gridSize;
    private static int _groupSize;
    private static int _warpSize;
    private bool _disposed = false;
    public string Description => _description;
    protected ILGpuPuzzle()
    {
        // intentionally left blank
    }
    public static ILGpuPuzzle Create(bool useCPU = false)
    {
        _context = Context.Create(builder => builder.Default().EnableAlgorithms());
        _accelerator = useCPU ? _context.CreateCPUAccelerator(0) : _context.CreateCudaAccelerator(0);
        _gridSize = 1;
        _groupSize = _accelerator.MaxNumThreadsPerGroup;
        _warpSize = _accelerator.WarpSize;
        var acceleratorType = _accelerator.AcceleratorType.ToString().ToUpper();
        _description = $"ILGpuPuzzle : {acceleratorType} : {_accelerator.Name} : GroupSize={_groupSize}, WarpSize={_warpSize}";
        return new ILGpuPuzzle();
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Free managed objects here.
                _accelerator?.Dispose();
                _context?.Dispose();
            }

            // Free unmanaged objects here.
            _disposed = true;
        }
    }
    ~ILGpuPuzzle()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
