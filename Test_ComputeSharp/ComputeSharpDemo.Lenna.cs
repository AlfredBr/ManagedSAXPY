using ComputeSharp;
using ComputeSharp.Interop;

using Throw;

namespace Test_ComputeSharp;

public static partial class ComputeSharpDemo
{
	public static void Lenna(string imagePath, bool debug = false)
	{
		imagePath.ThrowIfNull().IfWhiteSpace().IfEmpty();
		File.Exists(imagePath).Throw().IfFalse();

		// Get the default graphics device
		var graphicsDevice = debug ? /*cpu*/ GraphicsDevice.EnumerateDevices().ToArray()[2] : /*cuda*/ GraphicsDevice.GetDefault();
		const string groupSize = "XXXX";
		var warpSize = graphicsDevice.WavefrontSize;
		var coreSize = graphicsDevice.ComputeUnits;
		var acceleratorType = graphicsDevice.IsHardwareAccelerated && !debug ? "CUDA" : "CPU";
		var infoString = $"{acceleratorType} : {graphicsDevice.Name} : CoreSize={coreSize}, GroupSize={groupSize}, WarpSize={warpSize}";
		Console.WriteLine(infoString);
		// Load a texture from a specified image, and decode it in the BGRA32 format
		using var texture = GraphicsDevice.GetDefault().LoadReadWriteTexture2D<Bgra32, float4>(imagePath);
		// Run our shader on the texture we just loaded
		GraphicsDevice.GetDefault().For(texture.Width, texture.Height, new GrayscaleEffect(texture));
		// Save the processed image
		var filename = Path.GetFileNameWithoutExtension(imagePath);
		filename.ThrowIfNull().IfWhiteSpace().IfEmpty();
		var directory = Path.GetDirectoryName(imagePath);
		directory.ThrowIfNull().IfWhiteSpace().IfEmpty();
		var newImagePath = Path.Combine(directory, $"{filename}_bw.jpg");
		texture.Save(newImagePath);

		ShaderInfo shaderInfo = ReflectionServices.GetShaderInfo<GrayscaleEffect>();
		string hlslSource = shaderInfo.HlslSource;
		File.WriteAllText(Path.Combine(directory, $"{filename}_bw.hlsl"), hlslSource);
	}
}

[AutoConstructor]
internal readonly partial struct GrayscaleEffect : IComputeShader
{
	private readonly IReadWriteNormalizedTexture2D<float4> texture;
	public readonly void Execute()
	{
		// Our image processing logic here. In this example, we are just
		// applying a naive grayscale effect to all pixels in the image.
		var radius = texture.Height / 5;
		var midY = texture.Height / 2;
		var midX = texture.Width / 2;
		var x = ThreadIds.X;
		var y = ThreadIds.Y;
		var tx = Math.Pow(x - midX, 2);
		var ty = Math.Pow(y - midY, 2);
		var outCircle = (tx + ty) > Math.Pow(radius, 2);
		//if ((ThreadIds.X < midX - radius || ThreadIds.X > midX + radius) ||
		//	(ThreadIds.Y < midY - radius || ThreadIds.Y > midY + radius))
		if (outCircle)
		{
			float3 rgb = texture[ThreadIds.XY].RGB;
			float avg = Hlsl.Dot(rgb, new(0.0722f, 0.7152f, 0.2126f));
			texture[ThreadIds.XY].RGB = avg;
		}

	}
}
