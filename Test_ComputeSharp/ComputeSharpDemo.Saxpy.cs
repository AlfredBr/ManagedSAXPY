using ComputeSharp;

namespace Test_ComputeSharp;

public static class ComputeSharpDemo
{
	public static float[] Saxpy1(int N, float a, float[] x, float[] y)
	{
		var list = new List<SaxpyModel>();
		for (int i = 0; i < N; i++)
		{
			list.Add(new SaxpyModel
			{
				A = a,
				X = x[i],
				Y = y[i]
			});
		}
		var device = GraphicsDevice.GetDefault();
		var hostBuffer = list.ToArray();
		using ReadWriteBuffer<SaxpyModel> deviceBuffer = device.AllocateReadWriteBuffer(hostBuffer);
		device.For(N, new SaxpyCompute(a, deviceBuffer));
		deviceBuffer.CopyTo(hostBuffer);
		var result = hostBuffer.Select(s => s.S).ToArray();
		return result;
	}

	public static float[] Saxpy2(int N, float a, float[] x, float[] y)
	{
		var device = GraphicsDevice.GetDefault();
		using ReadWriteBuffer<float> deviceX = device.AllocateReadWriteBuffer(x);
		using ReadWriteBuffer<float> deviceY = device.AllocateReadWriteBuffer(y);
		device.For(N, new SaxpyKernel(a, deviceX, deviceY));
		var hostBuffer = new float[N];
		deviceY.CopyTo(hostBuffer);
		return hostBuffer;
	}
}

[ThreadGroupSize(DefaultThreadGroupSizes.X)]
[GeneratedComputeShaderDescriptor]
[System.Runtime.Versioning.SupportedOSPlatform("windows")]
internal readonly partial struct SaxpyKernel : IComputeShader
{
	private readonly float a;
	private readonly ReadWriteBuffer<float> x;
	private readonly ReadWriteBuffer<float> y;

	public SaxpyKernel(float a, ReadWriteBuffer<float> deviceX, ReadWriteBuffer<float> deviceY) : this()
	{
		this.a = a;
		this.x = deviceX;
		this.y = deviceY;
	}

	public void Execute()
	{
		var index = ThreadIds.X;
		y[index] = (a * x[index]) + y[index];
	}
}

[ThreadGroupSize(DefaultThreadGroupSizes.X)]
[GeneratedComputeShaderDescriptor]
[System.Runtime.Versioning.SupportedOSPlatform("windows")]
internal readonly partial struct SaxpyCompute : IComputeShader
{
	private readonly float a;
	private readonly ReadWriteBuffer<SaxpyModel> buffer;

	public SaxpyCompute(float a, ReadWriteBuffer<SaxpyModel> deviceBuffer) : this()
	{
		this.a = a;
		this.buffer = deviceBuffer;
	}

	public void Execute()
	{
		var saxpy = this.buffer[ThreadIds.X];
		saxpy.A = a;
		saxpy.S = (saxpy.A * saxpy.X) + saxpy.Y;
		this.buffer[ThreadIds.X] = saxpy;
	}
}
internal struct SaxpyModel
{
	public float S;
	public float A;
	public float X;
	public float Y;

	public readonly override string ToString()
	{
		return $"{S:0.00} = {A:0.00} * {X:0.00} + {Y:0.00}";
	}
}