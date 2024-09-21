using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Test_App;

public static class Intrinsics
{
	internal static float[] SaxpyVector(int N, float a, float[] x, float[] y)
	{
		// Ensure N is a multiple of the vector size for simplicity
		int vectorSize = Vector<float>.Count;
		if (N % vectorSize != 0)
		{
			throw new ArgumentException($"N must be a multiple of {vectorSize}");
		}

		// Initialize arrays
		var result = new float[N];
		var va = new Vector<float>(a);

		// Perform SIMD addition
		for (int i = 0; i < N; i += vectorSize)
		{
			var vx = new Vector<float>(x, i);
			var vy = new Vector<float>(y, i);
			var vc = (va * vx) + vy;
			// Store the result back into the array
			vc.CopyTo(result, i);
		}

		return result;
	}
	internal static float[] SaxpyMultiplyAdd(int N, float a, float[] x, float[] y)
	{
		// Ensure AVX2 is supported
		if (!Avx2.IsSupported)
		{
			throw new PlatformNotSupportedException("AVX2 is not supported on this platform.");
		}

		// Ensure N is a multiple of 8 for simplicity
		if (N % 8 != 0)
		{
			throw new ArgumentException("N must be a multiple of 8");
		}

		// Initialize arrays
		var result = new float[N];
		var va = Vector256.Create(a);

		// Perform SIMD Fused Multiply-Add
		for (int i = 0; i < N; i += 8)
		{
			var vx = Vector256.Create(x[i], x[i + 1], x[i + 2], x[i + 3], x[i + 4], x[i + 5], x[i + 6], x[i + 7]);
			var vy = Vector256.Create(y[i], y[i + 1], y[i + 2], y[i + 3], y[i + 4], y[i + 5], y[i + 6], y[i + 7]);
			// FMA operation: va * vx + vy
			var vc = Fma.MultiplyAdd(va, vx, vy);
			// Store the result back into the array
			vc.CopyTo(result, i);
		}

		return result;
	}
	internal static double[] SaxpyMultiplyAddUsingPointers(int N, double a, double[] x, double[] y)
	{
		// Ensure AVX is supported
		if (!Avx.IsSupported)
		{
			throw new PlatformNotSupportedException("AVX is not supported on this platform.");
		}
		// Ensure N is a multiple of the vector size for simplicity
		int vectorSize = Vector<double>.Count;
		if (N % vectorSize != 0)
		{
			throw new ArgumentException($"N must be a multiple of {vectorSize}");
		}
		// Initialize arrays
		var result = new double[N];
		var va = Vector256.Create(a);
		// Perform SIMD addition
		unsafe
		{
			fixed (double* px = x, py = y, presult = result)
			{
				for (int i = 0; i < N; i += vectorSize)
				{
					var vx = Avx.LoadVector256(px + i);
					var vy = Avx.LoadVector256(py + i);
					// FMA operation: va * vx + vy
					var vc = Fma.MultiplyAdd(va, vx, vy);
					// Store the result back into the array
					Avx.Store(presult + i, vc);
				}
			}
		}
		return result;
	}
	internal static float[] Saxpy128(int N, float a, float[] x, float[] y)
	{
		// Ensure SSE is supported
		if (!Sse.IsSupported)
		{
			throw new PlatformNotSupportedException("SSE is not supported on this platform.");
		}
		// Ensure N is a multiple of 4 for simplicity
		if (N % 4 != 0)
		{
			throw new ArgumentException("N must be a multiple of 4");
		}
		int vectorSize = Vector<double>.Count;
		if (N % vectorSize != 0)
		{
			throw new ArgumentException($"N must be a multiple of {vectorSize}");
		}
		// Initialize arrays
		var result = new float[N];
		var va = Vector128.Create(a);
		// Perform SIMD addition
		for (int i = 0; i < N; i += 4)
		{
			var vx = Vector128.Create(x[i], x[i + 1], x[i + 2], x[i + 3]);
			var vy = Vector128.Create(y[i], y[i + 1], y[i + 2], y[i + 3]);
			var vax = Sse.Multiply(va, vx);
			var vc = Sse.Add(vax, vy);
			// Store the result back into the array
			vc.CopyTo(result, i);
		}
		return result;
	}
	internal static float[] Saxpy256(int N, float a, float[] x, float[] y)
	{
		// Ensure AVX2 is supported
		if (!Avx2.IsSupported)
		{
			throw new PlatformNotSupportedException("AVX2 is not supported on this platform.");
		}
		// Ensure N is a multiple of 8 for simplicity
		if (N % 8 != 0)
		{
			throw new ArgumentException("N must be a multiple of 8");
		}
		// Initialize arrays
		var result = new float[N];
		var va = Vector256.Create(a);
		// Perform SIMD addition
		for (int i = 0; i < N; i += 8)
		{
			var vx = Vector256.Create(x[i], x[i + 1], x[i + 2], x[i + 3], x[i + 4], x[i + 5], x[i + 6], x[i + 7]);
			var vy = Vector256.Create(y[i], y[i + 1], y[i + 2], y[i + 3], y[i + 4], y[i + 5], y[i + 6], y[i + 7]);
			var vax = Avx2.Multiply(va, vx);
			var vc = Avx2.Add(vax, vy);
			// Store the result back into the array
			vc.CopyTo(result, i);
		}
		return result;
	}
	internal static float[] Saxpy512(int N, float a, float[] x, float[] y)
	{
		// Ensure AVX-512 is supported
		if (!Avx512F.IsSupported)
		{
			throw new PlatformNotSupportedException("AVX-512 is not supported on this platform.");
		}
		// Ensure N is a multiple of 16 for simplicity
		if (N % 16 != 0)
		{
			throw new ArgumentException("N must be a multiple of 16");
		}
		// Initialize arrays
		var result = new float[N];
		var va = Vector512.Create(a);
		// Perform SIMD addition
		for (int i = 0; i < N; i += 16)
		{
			var vx = Vector512.Create(
				x[i], x[i + 1], x[i + 2], x[i + 3],	x[i + 4], x[i + 5], x[i + 6], x[i + 7],
				x[i + 8], x[i + 9], x[i + 10], x[i + 11], x[i + 12], x[i + 13], x[i + 14], x[i + 15]
			);
			var vy = Vector512.Create(
				y[i], y[i + 1], y[i + 2], y[i + 3],	y[i + 4], y[i + 5], y[i + 6], y[i + 7],
				y[i + 8], y[i + 9], y[i + 10], y[i + 11], y[i + 12], y[i + 13], y[i + 14], y[i + 15]
			);
			var vax = Avx512F.Multiply(va, vx);
			var vc = Avx512F.Add(vax, vy);
			// Store the result back into the array
			vc.CopyTo(result, i);
		}
		return result;
	}
}
