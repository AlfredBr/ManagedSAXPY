using ComputeSharp;

namespace Test_ComputeSharp;

public partial class ComputeSharpPuzzle : IDisposable
{
	private static GraphicsDevice? _graphicsDevice;
	private static string _description = string.Empty;
	private bool _disposed = false;
	public string Description => _description;
	protected ComputeSharpPuzzle()
	{
		// intentionally left blank
	}
	public static ComputeSharpPuzzle Create(bool useCpu = false)
	{
		_graphicsDevice = useCpu ? GraphicsDevice.EnumerateDevices().ToArray()[2] : GraphicsDevice.GetDefault();
		const string groupSize = "????";
		var warpSize = _graphicsDevice.WavefrontSize;
		var acceleratorType = _graphicsDevice.IsHardwareAccelerated && !useCpu ? "CUDA" : "CPU";
		_description = $"ComputeSharpPuzzle : {acceleratorType} : {_graphicsDevice.Name} : GroupSize={groupSize}, WarpSize={warpSize}";
		return new ComputeSharpPuzzle();
	}
	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			if (disposing)
			{
				// Free managed objects here.
				_graphicsDevice?.Dispose();
			}

			// Free unmanaged objects here.
			_disposed = true;
		}
	}
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
	~ComputeSharpPuzzle()
	{
		Dispose(disposing: false);
	}
}
