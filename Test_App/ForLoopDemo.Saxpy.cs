namespace Test_App;

public static partial class ForLoopDemo
{
	internal static float[] Saxpy(int N, float a, float[] x, float[] y)
	{
		var result = new float[N];
		for (int i = 0; i < N; i++)
		{
			result[i] = (a * x[i]) + y[i];
		}
		return result;
	}
}
